USE [msdb]
GO

/****** Object:  Job [Enterprise Data Migration]    Script Date: 3/13/2019 4:58:51 PM ******/
BEGIN TRANSACTION
DECLARE @DataFilesFolder NVarchar(1024) = N'P:\'
DECLARE @OutputFile NVARCHAR(2048) = @DataFilesFolder + '\EnterpriseDataMigrationJob.out' 
DECLARE @stepCommand nVarchar(MAX)
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 3/13/2019 4:58:51 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

SET @stepCommand = N'-- use master
	DECLARE @path nvarchar(max) = ''' + @DataFilesFolder + '''
	DECLARE @MSG nvarchar(512) = N''Please specify folder containing the data files dsttimes.txt, CrossWalkData.txt, and CrossWalkFileFormat.fmt''
	IF (@Path = '''') 
	BEGIN
		raiserror(@msg, 15,-1)
	END
	--Verify files accessable
	IF @Path IS NULL OR RTRIM(LTRIM(@Path)) = ''''
	BEGIN
		RAISERROR(@MSG, 15, -1) 
	END	

	IF @@ERROR = 0
	BEGIN
		EXEC(''
			CREATE TABLE #tmp (
		[TimeZone] [nvarchar](50) NOT NULL,
		[UTCOffset] [int] NOT NULL,
		[DSTBegin] [datetimeoffset](7) NOT NULL,
		[DSTEnd] [datetimeoffset](7) NOT NULL,
		[DSTOffset] [int] NOT NULL
	) 

    BULK
	INSERT #tmp 
	FROM ''''''+@path+''\dsttimes.txt''''
	WITH
	(
		FIELDTERMINATOR = ''''\t'''',
		ROWTERMINATOR = ''''\n''''
	); 
	DROP TABLE #tmp
			'')

	END
IF @@ERROR = 0
	PRINT ''Successfully completed TAS UPG 00005''
GO
'
DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Enterprise Data Migration', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Migrates v7.5.x TAS Terminal database to v10.x TAS Terminal database', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00005 Prepare for Migration]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00005 Prepare for Migration', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=@stepCommand, 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00006 CHECK Source Database]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00006 CHECK Source Database', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
--		SELECT distinct q.[EquipmentIndex]
--      ,q.[EquipmentID]
--      ,q.[EquipmentType]
--      ,q.[SiteIndex]
--	 into #tmp
--  FROM [ConsolidatedDB].[dbo].[tblEquipmentQualityTagLog] q left join [ConsolidatedDB].[dbo].[tblEquipment] e on q.EquipmentIndex=e.[index] 
--  WHERE e.[index] is NULL AND (q.SiteIndex=0 or (q.SiteIndex = (SELECT MIN(siteIndex) FROM [ConsolidatedDB].[dbo].[tblEquipmentQualityTagLog] WHERE q.EquipmentIndex = EquipmentIndex)))

--versions 8 and above do not allow same userids for different sites.  they are 
-- entity assigned to the sites
UPDATE tblUsers Set userid = Userid + convert(varchar(10), siteindex) where userid = ''administrator'' and siteindex <> -1

-- to stop the Siteadmin from been deleted in Step 6
UPDATE tblsites SET timezone = ''UTC'' WHERE siteindex = -1 AND timezone IS NULL

-- DELETE ANY SAVED QUERIES THAT BELONG TO USERS THAT NO LONGER EXIST IN THE DATABASE
DELETE FROM dbo.tblSavedQueryItems WHERE QueryIndex IN (SELECT QueryIndex FROM dbo.tblSavedQueries WHERE UserIndex NOT IN (SELECT UserIndex FROM dbo.tblUsers))
DELETE FROM dbo.tblSavedQueries WHERE UserIndex NOT IN (SELECT UserIndex FROM dbo.tblUsers)


IF EXISTS(select a.name, t.* from sys.triggers t join  sys.tables a on t.parent_id=a.object_id where a.name=''tblusers'' and t.name=''TR_tblUser_InsUpd'')
	DROP TRIGGER [dbo].[TR_tblUser_InsUpd]
GO
IF EXISTS(select a.name, t.* from sys.triggers t join  sys.tables a on t.parent_id=a.object_id where a.name=''tblusers'' and t.name=''TR_tblUsers_IU_UpdateInactivityLockoutDate'')
	DROP TRIGGER [dbo].[TR_tblUsers_IU_UpdateInactivityLockoutDate]
GO
UPDATE [ConsolidatedDB].[dbo].[tblUsers] SET siteindex=-1 WHERE siteindex=0


--SELECT ID, SiteIndex, COUNT(*) FROM dbo.tblFuelCards  GROUP BY ID, SiteIndex HAVING COUNT(*) > 1

--SELECT * FROM tblEquipmentQualityTagLog WHERE EquipmentIndex NOT IN (SELECT [Index] FROM tblEquipment)
--SELECT * FROM tblTankQualityTagLog WHERE TankIndex NOT IN (SELECT [TankIndex] FROM tblTanks)

--SELECT * FROM tblTestSetEquipmentResults WHERE TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)
--SELECT * FROM tblTestSetTankResults WHERE TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)
--SELECT * FROM tblTestSetEquipmentResults WHERE EquipmentIndex NOT IN (SELECT [index] FROM tblEquipment)

--SELECT * FROM dbo.tblProducts p WHERE massunitindex NOT IN (SELECT engineeringunitindex FROM FuelsManagerDB_Template.[lookup].[tblEngineeringUnit])

--DELETE e FROM tblEquipmentQualityTagLog e WHERE EquipmentIndex NOT IN (SELECT [Index] FROM tblEquipment)

--DELETE t FROM tblTestEquipmentResults t JOIN tblTestSetEquipmentResults e ON t.TestSetEquipmentResultIndex=e.TestSetEquipmentResultIndex WHERE  e.TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)
--DELETE e FROM tblTestSetEquipmentResults e WHERE TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)

--DELETE t FROM tblTestEquipmentResults t JOIN tblTestSetEquipmentResults e ON t.TestSetEquipmentResultIndex=e.TestSetEquipmentResultIndex WHERE  e.EquipmentIndex NOT IN (SELECT [index] FROM tblEquipment)
--DELETE FROM tblTestSetEquipmentResults WHERE EquipmentIndex NOT IN (SELECT [index] FROM tblEquipment)

--DELETE t FROM tblTestTankResults t JOIN tblTestSetTankResults e ON t.TestSetTankResultIndex=e.TestSetTankResultIndex WHERE  e.TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)
--DELETE e FROM tblTestSetTankResults e WHERE TestSetName NOT IN (SELECT TestSetName FROM tblTestSetDefinitions)

--DELETE t FROM tblTestTankResults t JOIN tblTestSetTankResults e ON t.TestSetTankResultIndex=e.TestSetTankResultIndex WHERE  e.TankIndex NOT IN (SELECT [Tankindex] FROM tblTanks)
--DELETE FROM tblTestSetTankResults WHERE TankIndex NOT IN (SELECT [Tankindex] FROM tblTanks)

--UPDATE p SET massunitindex=NULL FROM dbo.tblProducts p WHERE massunitindex NOT IN (SELECT engineeringunitindex FROM FuelsManagerDB_Template.[lookup].[tblEngineeringUnit])

--remove duplicate fuelcard entry
--select * FROM dbo.tblFuelCards tf JOIN   
--	(SELECT fuelcardindex, siteindex, id, id + ROW_NUMBER() OVER( ORDER BY id) as new_id FROM dbo.tblFuelCards f WHERE EXISTS(SELECT TOP 1 1 FROM dbo.tblFuelcards WHERE id=f.id AND siteindex=f.siteindex GROUP BY id HAVING COUNT(*) > 1)) a
--	ON tf.fuelcardindex=a.fuelcardindex
--BEGIN TRANSACTION

--UPDATE tf SET id =  a.new_id FROM dbo.tblFuelCards tf JOIN   
--	(SELECT fuelcardindex, siteindex, id, id + CONVERT(nvarchar(6),ROW_NUMBER() OVER( ORDER BY id)) as new_id FROM dbo.tblFuelCards f  WITH(NOLOCK)  WHERE EXISTS(SELECT TOP 1 1 FROM dbo.tblFuelcards  WITH(NOLOCK)  WHERE id=f.id AND siteindex=f.siteindex GROUP BY id HAVING COUNT(*) > 1)) a
--	ON tf.fuelcardindex=a.fuelcardindex
--UPDATE dbo.tblTransactions SET fuelcardid = f.id FROM dbo.tblfuelcards f  WITH(NOLOCK)  WHERE f.fuelcardindex=dbo.tblTransactions.fuelcardindex  and ISNULL(dbo.tblTransactions.fuelcardid,'''') <> f.id

--COMMIT TRANSACTION

--UPDATE dbo.tblTransactions SET fuelcardindex=(select fuelcardindex from dbo.tblfuelcards where fuelcardguid=''6F72F334-1034-452E-B890-499F4899EEC7'') 
--WHERE fuelcardindex=(select fuelcardindex from dbo.tblfuelcards where fuelcardguid=''64570974-1320-46EF-A7C9-5556C6A52402'')
--DELETE FROM dbo.tblFuelCards WHERE fuelcardguid=''64570974-1320-46EF-A7C9-5556C6A52402''

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00010 Drop Database Objects]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00010 Drop Database Objects', 
		@step_id=3, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	TAS Database Migration To v10.x
	SCRIPT TO DROP: Idexes, Primary Keys, Foreign Keys, Defaults, Stored Procedures, Functions
	Author: Aloisio(Al) dos Santos
*/

SET NOCOUNT ON; 

DECLARE @ErrorCount int=0
DECLARE @MSG NVARCHAR(MAX)

exec sp_configure ''clr enabled'',1
reconfigure
GO

ALTER DATABASE ConsolidatedDB
SET TRUSTWORTHY ON
GO

ALTER DATABASE ConsolidatedDB
SET RECOVERY SIMPLE;
GO

EXEC sp_changedbowner @loginame = ''sa''
GO

truncate table dbo.tblChangesQueue
GO

truncate table dbo.tblSessions
GO

IF  EXISTS (SELECT * FROM sys.triggers WHERE parent_class_desc = ''DATABASE'' AND name = N''TRDDL_DROP_TABLE'') DROP TRIGGER [TRDDL_DROP_TABLE] ON DATABASE

DECLARE @SchemaName NVARCHAR(300)
	,	@ObjectName NVARCHAR(400)
	,	@TableName NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@ColumnName NVARCHAR(500)
	,	@FromType NVARCHAR(500)
	,	@ToType NVARCHAR(500)
	,	@SiteIndexLevel TINYINT

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as IndexName
	,	o.name as TableName
	FROM sys.indexes i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	and i.Is_Unique=0
	and i.index_id <> 1
	and o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP INDEX [''+@ObjectName+'']''+'' ON [''+@SchemaName+''].[''+@TableName+''] ''
	PRINT @Sql

	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------------------	FOREIGN KEYS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP FOREIGN KEYS''
PRINT ''************************************''
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as IndexName
	,	o.name as TableName
	FROM sys.foreign_keys i
	INNER JOIN sys.objects o on o.object_id=i.parent_object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] DROP CONSTRAINT [''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

----------------------- DEFAULT CONSTRAINTS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP DEFAULT CONSTRAINTS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as IndexName
	,	o.name as TableName
	FROM sys.default_constraints i
	INNER JOIN sys.objects o on o.object_id=i.parent_object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	and o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] DROP CONSTRAINT [''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


------------------	INDEX/UNIQUE CONSTRAINTS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP INDEX/UNIQUE CONSTRAINTS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName
	FROM sys.indexes i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	and i.Is_Unique_Constraint=1
	and i.index_id <> 1
	and  o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] DROP CONSTRAINT [''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName
	FROM sys.indexes i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type =''U''
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	AND i.is_primary_key=0
	and i.index_id <> 1
	and  o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP INDEX [''+@ObjectName+''] ON [''+@SchemaName+''].[''+@TableName+''];''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------------------	CHECK CONSTRAINTS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP CHECK CONSTRAINTS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName

	FROM sys.check_constraints i
	INNER JOIN sys.objects o on o.object_id=i.parent_object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	and  o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] DROP CONSTRAINT [''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------- VIEWS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP VIEWS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName
	FROM sys.views i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP VIEW [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------------- TRIGGERS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP TRIGGERS''
PRINT ''************************************''
	
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as TriggerName
	,	o.name as TableName
	FROM sys.triggers i
	inner join sys.objects o on o.object_id=i.object_id
	inner join sys.schemas s on s.schema_id=o.schema_id
	WHERE s.name=''dbo''
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP TRIGGER [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


------------------- PRIMARY KEYS
------------------- GCP UPDATED 1/10/2019: ADDED additional filter to ONLY include objects where p.type NOT IN (''TF'').  
-------------------                Reason: The output table from dbo.ufn_ProductList declares a CLUSTERED PRIMARY KEY on the ProductIndex column.  These are not associated with a REAL TABLE and therefore you CANNOT generate an ALTER statement to remove it.
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP PRIMARY KEYS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	o.name as FkName
	,	p.name as TableName
	FROM sys.sysconstraints i
	inner join sys.objects o on o.object_id=i.constid
	inner join sys.schemas s on s.schema_id=o.schema_id
	inner join sys.objects p on p.object_id=i.id
	WHERE o.type NOT IN (''S'',''IT'')
	AND p.type NOT IN (''TF'')
	AND o.name IS NOT NULL
	AND s.name=''dbo'' and o.name not in (''PK_tblExportResults'',''PK_tblSites'',''PK_tblTransactions'',''PK_tblTransactionLineItems'')
	and p.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,o.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] DROP CONSTRAINT [''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


PRINT ''''
PRINT ''************************************''
PRINT ''** DROP CLUSERTED INDEXES ''
PRINT ''************************************''
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName
	FROM sys.indexes i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type =''U''
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	and i.index_id = 1
	and i.name not in (''PK_tblExportResults'',''PK_tblSites'',''PK_tblTransactions'',''PK_tblTransactionLineItems'')
	and o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP INDEX [''+@ObjectName+''] ON [''+@SchemaName+''].[''+@TableName+''];''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------------- STATISTICS
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP STATISTICS''
PRINT ''************************************''

DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as StatName
	,	o.name as TableName
	FROM sys.stats i
	inner join sys.objects o on o.object_id=i.object_id
	inner join sys.schemas s on s.schema_id=o.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND o.name IS NOT NULL
	and o.name not in (''tblExportResultDetails'', ''tblExportResults'', ''tblTransactions'', ''tbltransactionLineitems'')
	AND s.name=''dbo''
	AND i.user_created=1
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP STATISTICS [''+@SchemaName+''].[''+@TableName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

	------------- Referenced Functions
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP USER FUNCTIONS''
PRINT ''************************************''	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[GetLocalTime]'') AND type in (N''FN'', N''IF'', N''TF'', N''FS'', N''FT''))
	DROP FUNCTION [dbo].GetLocalTime


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[GetUTCTime]'') AND type in (N''FN'', N''IF'', N''TF'', N''FS'', N''FT''))
	DROP FUNCTION [dbo].[GetUTCTime]

/****** Object:  UserDefinedFunction [dbo].[GetLocalOffset]    Script Date: 03/06/2013 12:31:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[GetLocalOffset]'') AND type in (N''FN'', N''IF'', N''TF'', N''FS'', N''FT''))
	DROP FUNCTION [dbo].[GetLocalOffset]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[GetLocalOffset]'') AND type in (N''FN'', N''IF'', N''TF'', N''FS'', N''FT''))
	DROP FUNCTION [dbo].[GetLocalOffset]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[GetUnitAbbrev]'') AND type in (N''FN'', N''IF'', N''TF'', N''FS'', N''FT''))
	DROP FUNCTION [dbo].GetUnitAbbrev

------- FN (sql Scalar) FUNCTIONS
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	o.name as FkName
	FROM sys.objects o
	INNER JOIN sys.schemas s on s.schema_id=s.schema_id
	WHERE o.type IN (''FN'')
	AND o.name IS NOT NULL
	AND s.name=''dbo''

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP FUNCTION [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

------- FN (sql Scalar) FUNCTIONS
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	o.name as FkName
	FROM sys.objects o
	INNER JOIN sys.schemas s on s.schema_id=s.schema_id
	WHERE o.type IN (''FS'',''FT'',''IF'',''TF'')
	AND o.name IS NOT NULL
	AND s.name=''dbo''

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP FUNCTION [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


------- FN (sql Aggreaget) FUNCTIONS
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	o.name as FkName
	FROM sys.objects o
	INNER JOIN sys.schemas s on s.schema_id=s.schema_id
	WHERE o.type IN (''AF'')
	AND o.name IS NOT NULL
	AND s.name=''dbo''

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP AGGREGATE [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor





------- PROCEDURES
PRINT ''''
PRINT ''************************************''
PRINT ''** DROP STORED PROCEDURES''
PRINT ''************************************''	
	
DECLARE ObjCursor CURSOR FOR
	SELECT	DISTINCT	
		s.name as SchemaName
	,	i.name as FkName
	,	o.name as TableName
	FROM sys.procedures i
	INNER JOIN sys.objects o on o.object_id=i.object_id
	INNER JOIN sys.schemas s on s.schema_id=s.schema_id
	WHERE o.type NOT IN (''S'',''IT'')
	AND i.name IS NOT NULL
	AND s.name=''dbo''
	AND i.NAME NOT LIKE ''%NotificationStoredProcedure%''
	AND i.name NOT IN (''usp_RenameColumn'')
	ORDER BY s.name,i.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP PROCEDURE [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName,@TableName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor



PRINT ''''
PRINT ''************************************''
PRINT ''** DROP ASSEMBLIES''
PRINT ''************************************''	

/****** Object:  SqlAssembly [FMCLRAssembly]    Script Date: 03/06/2013 12:29:19 ******/
IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N''FMCLRAssembly'')
DROP ASSEMBLY [FMCLRAssembly]

/****** Object:  SqlAssembly [EngineeringUnitsLibrary]    Script Date: 03/06/2013 12:29:02 ******/
IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N''EngineeringUnitsLibrary'')
DROP ASSEMBLY [EngineeringUnitsLibrary]

/****** Object:  SqlAssembly [FMCLRStoredProcedureAssembly]    Script Date: 03/06/2013 12:29:29 ******/
IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N''FMCLRStoredProcedureAssembly'')
DROP ASSEMBLY [FMCLRStoredProcedureAssembly]

/****** Object:  SqlAssembly [Interop.ConvertEngUnits]    Script Date: 03/06/2013 12:29:44 ******/
IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N''Interop.ConvertEngUnits'')
DROP ASSEMBLY [Interop.ConvertEngUnits]


--DECLARE @ObjectName NVARCHAR(500)
--	,	@SchemaName NVARCHAR(500)
--	,	@Sql NVARCHAR(max)

DECLARE ObjCursor CURSOR FOR
	SELECT	s.name as SchemaName
		,	o.name as ProcName
	FROM sys.procedures O
	inner join sys.schemas s on s.schema_id=o.schema_id
	order by s.name,o.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP PROCEDURE [''+@SchemaName+''].[''+@ObjectName+''] ''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @SchemaName,@ObjectName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor



PRINT ''''
PRINT ''************************************''
PRINT ''** DROP USERS/LOGINS''
PRINT ''************************************''	
DECLARE @UserName nvarchar(255)
DECLARE ObjCursor CURSOR FOR
	SELECT du.name FROM sys.database_principals du
	--INNER JOIN tblUsers u on du.name = u.UserID
	WHERE [Type] = ''S'' and default_schema_name = ''SQLDependency'' --sqluser

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @UserName
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''DROP USER ['' + @UserName + '']''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	IF EXISTS(SELECT 1 FROM master.sys.database_principals WHERE name = @UserName)
	BEGIN
		SET @Sql=''USE MASTER; DROP USER ['' + @UserName + '']''
		PRINT @Sql
		EXEC sp_executesql @statement=@Sql
	END

	IF EXISTS(SELECT 1 FROM master..syslogins WHERE name = @UserName)
	BEGIN
		SET @Sql=''DROP LOGIN @UserName ['' + @UserName + '']''
		PRINT @Sql
		EXEC sp_executesql @statement=@Sql

	END

	FETCH NEXT FROM ObjCursor INTO @UserName
END
CLOSE ObjCursor
DEALLOCATE ObjCursor
	

PRINT ''''
PRINT ''************************************''
PRINT ''** PROCESS COMPLETE''
PRINT ''************************************''	
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00012 Adjust DB Compatibility level]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00012 Adjust DB Compatibility level', 
		@step_id=4, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'ALTER DATABASE [ConsolidatedDB] SET COMPATIBILITY_LEVEL = 110 --sql 2012
', 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

SET @stepCommand = N'--creates timezone tables used later on
DECLARE @path nvarchar(max) = '''+@DataFilesFolder+N'''
EXEC (
''IF NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = ''''dbo'''' 
                 AND  TABLE_NAME = ''''tblTimeZoneDSTOffSets'''')
BEGIN

	CREATE TABLE tblTimeZoneDSTOffSetsLoad (
		[TimeZone] [nvarchar](50) NOT NULL,
		[UTCOffset] [int] NOT NULL,
		[DSTBegin] [datetimeoffset](7) NOT NULL,
		[DSTEnd] [datetimeoffset](7) NOT NULL,
		[DSTOffset] [int] NOT NULL
	) 

    BULK
	INSERT tblTimeZoneDSTOffSetsLoad 
	FROM ''''''+@path+''\dsttimes.txt''''
	WITH
	(
		FIELDTERMINATOR = ''''\t'''',
		ROWTERMINATOR = ''''\n''''
	); 

	CREATE TABLE  tblTimeZones
	(
		[Index] int NOT NULL PRIMARY KEY,
		Name nvarchar(50) NOT NULL,
		UTCOffSet int NOT NULL
	)

	CREATE TABLE tblSiteTimeZones 
	(	SiteIndex int NOT NULL PRIMARY KEY,
		[Index] int NOT NULL,
		[SiteGuid] uniqueidentifier,
	--	Name nvarchar(50) NOT NULL,
		AdjustForDayLightSavings bit,
		UTCOffSet int NOT NULL
	)
	--drop table tblTimeZoneDSTOffSetsLoad

	CREATE TABLE tblTimeZoneDSTOffSets 
	(
		TimeZoneIndex int,
		StartTime datetimeoffset(7),
		EndTime datetimeoffset(7),
		Offset int,
		UNIQUE CLUSTERED (TimeZoneIndex, StartTime, EndTime)
	)
END


INSERT INTO tblTimeZones
SELECT DISTINCT row_number() over (order by tz.timezone) , tz.TimeZone, UTCOFFset 
FROM tblTimeZoneDSTOffSetsLoad tz 
INNER JOIN tblsites s ON tz.TimeZone = s.TimeZone
GROUP BY tz.timezone, utcoffset
UNION 
SELECT 20, ''''UTC'''',0

INSERT INTO tblSiteTimeZones (SiteIndex ,
		[Index] ,
		AdjustForDayLightSavings ,
		UTCOffSet )
SELECT s.SiteIndex, tz.[Index] , s.AdjustForDaylightSavings, tz.UTCOFFset 
FROM tblTimeZones tz 
INNER JOIN tblsites s ON tz.Name = s.TimeZone

INSERT INTO tblTimeZoneDSTOffSets
SELECT tz.[index], tz2.DSTBegin, tz2.DSTEnd, tz2.DSTOffset
FROM tblTimeZones tz
INNER JOIN tblTimeZoneDSTOffSetsLoad tz2 
	on tz.Name = tz2.TimeZone'')

PRINT ''Completed successfully''

'
/****** Object:  Step [TAS UPG 00013 Create TimeZone Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00013 Create TimeZone Tables', 
		@step_id=5, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=@stepCommand, 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00014 DateTimeConvert non special tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00014 DateTimeConvert non special tables', 
		@step_id=6, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	DATATIME CONVERSION
	drop table  #tblTimeZones
*/


SET NOCOUNT ON;

DECLARE @ErrorCount int=0
DECLARE @MSG NVARCHAR(MAX)
--DECLARE @SchemaName NVARCHAR(300)
--	,	@ObjectName NVARCHAR(400)
--	,	@TableName NVARCHAR(500)
--	,	@Sql NVARCHAR(max)
--	,	@ColumnName NVARCHAR(500)
--	,	@FromType NVARCHAR(500)
--	,	@ToType NVARCHAR(500)
--	,	@SiteIndexLevel TINYINT

PRINT ''''
PRINT ''************************************''
PRINT ''** MODIFY DATETIME COLUMNS TO DATETIMEOFFSET''
PRINT ''************************************''


DECLARE @SchemaName NVARCHAR(300)
	,	@ObjectName NVARCHAR(400)
	,	@TableName NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@ColumnName NVARCHAR(500)
	,	@FromType NVARCHAR(500)
	,	@ToType NVARCHAR(500)
	,	@SiteIndexLevel TINYINT

--DECLARE DateCursor SCROLL CURSOR FOR

DECLARE @ConvertDate TABLE(
		RowNumber INT IDENTITY
	,	TableSchema NVARCHAR(200)
	,	TableName	NVARCHAR(500)
	,	ColumnName	NVARCHAR(500)
	,	FromDataType NVARCHAR(500)
	,	ToDataType NVARCHAR(500)
	,	SiteIndexLevel TINYINT DEFAULT 0
	)


	INSERT INTO @ConvertDate(TableSchema,TableName,ColumnName,FromDataType,ToDataType)
	SELECT	c1.table_schema
		,	c1.table_name
		,	c1.column_name
		,	c2.data_type as FromDataType
		,	c1.data_type as ToDataType
	FROM FuelsManagerDB_Template.information_schema.columns c1
	INNER JOIN ConsolidatedDB.information_schema.columns c2 ON
		(c2.table_schema=c1.table_schema AND c2.table_name=c1.table_name and c2.column_name=c1.column_name)
	WHERE c2.data_type<>c1.data_type AND c1.DATA_TYPE = ''datetimeoffset''
	AND  NOT (c1.table_schema = ''dbo'' and c1.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
								''tblTransactionLineItems'',
								''tblTransactions'',
								''tblTransactionUserData'',
								''tblTransactionLineItemUserData'',
								''tblTransactionNotes'',
								''tblTransactionSignature'',
								''tblTransactionTransportLineItems'',
								''tblTransactionWeightReadings'',
								''tblTransactionLinks'',
								''tblTransactionPIDX'',
								''tblTransactionSubLineItems''))
	ORDER BY c1.table_schema,c1.table_name,c1.column_name


	DECLARE DateCursor SCROLL CURSOR  FOR
		SELECT DISTINCT TableSchema,TableName--,ColumnName
		FROM @ConvertDate
		WHERE ToDataType=''datetimeoffset''
	
		ORDER BY TableSchema,TableName
	OPEN DateCursor
	FETCH NEXT FROM DateCursor INTO @SchemaName,@TableName
	WHILE @@FETCH_STATUS=0
	BEGIN
		IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema=@SchemaName AND table_name=@TableName AND Column_Name=''SiteIndex'')
		BEGIN
				UPDATE @ConvertDate
				SET SiteIndexLevel = 1
				WHERE TableSchema=@SchemaName
				AND TableName=@TableName

		END
	
		-- LEVEL 2: Any transaction derived table as they all link with transaction table to find the SiteIndex

		UPDATE @ConvertDate
		SET SiteIndexLevel = 2
		WHERE LEFT(TableName,14) = ''tblTransaction''
		AND TableName NOT IN(''tblTransactionLineItemUserData'')
		AND SiteIndexLevel = 0

		--LEVEL 3: Remaining Tables
		UPDATE @ConvertDate
		SET SiteIndexLevel = 3
		WHERE TableName NOT IN(''tblExportResultDetails'',''tblAllocationLineItems'',''tblArchivedUsers'',''tblChangesQueue'')
		AND SiteIndexLevel = 0

		FETCH NEXT FROM DateCursor INTO @SchemaName,@TableName

	END
	CLOSE DateCursor
	DEALLOCATE DateCursor


DECLARE TableCursor SCROLL CURSOR FOR
	SELECT DISTINCT TableSchema,TableName,SiteIndexLevel
	FROM @ConvertDate
	ORDER BY TableSchema,TableName
OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel
WHILE @@FETCH_STATUS=0
BEGIN

	DECLARE @isnewdateoffsetcol bit
		,	@SqlInsertColumns NVARCHAR(max)
		,	@SqlInsertVals NVARCHAR(max)
		,	@SqlTdzJoins NVARCHAR(max)
		,	@tzdCount INT
		,	@SqlCreateEmpty nvarchar(10)

		SET @SqlInsertColumns = '''';
		SET @SqlInsertVals = '''';
		SET @SqlTdzJoins = '''';
		SET @isnewdateoffsetcol = 0;
		SET @tzdCount = 0;
		SET @SqlCreateEmpty = '''';

		IF (@TableName <> ''tblExportResultDetails'' AND @TableName <> ''tblAllocationLineItems'' AND @TableName <> ''tblArchivedUsers'' AND @TableName <> ''tblChangesQueue'')
		BEGIN
			SET @SqlCreateEmpty =  ''WHERE 1=0''
		END

		-- create new table structure to match
		SET @Sql = ''SELECT *  INTO [''+@SchemaName+''].[''+@TableName+''_DateMigrate]  FROM [''+@SchemaName+''].[''+@TableName+''] '' + @SqlCreateEmpty
		PRINT @Sql
		EXEC sp_executesql @statment=@Sql

		--- MODIFY COLUMNS
		DECLARE ColumnCursor SCROLL CURSOR FOR
		SELECT	c1.column_name
			,	case when c2.data_type = ''datetimeoffset'' and c2.DATA_TYPE <> c1.DATA_TYPE then 1 else 0 end
		FROM ConsolidatedDB.information_schema.columns c1
			LEFT JOIN FuelsManagerDB_Template.information_schema.columns c2
				ON c1.TABLE_SCHEMA = c2.TABLE_SCHEMA AND c1.TABLE_NAME = c2.TABLE_NAME and c2.column_name=c1.column_name
		WHERE c1.TABLE_SCHEMA = @SchemaName and c1.TABLE_NAME = @TableName
		ORDER BY c1.column_name
		OPEN ColumnCursor
		FETCH NEXT FROM ColumnCursor INTO @ColumnName,@isnewdateoffsetcol
		WHILE @@FETCH_STATUS=0
		BEGIN

			IF (@TableName = ''tblExportResultDetails'' OR @TableName = ''tblAllocationLineItems'' OR @TableName = ''tblArchivedUsers'' OR @TableName = ''tblChangesQueue'')
			BEGIN
				IF (@isnewdateoffsetcol = 1)
				BEGIN
					SET @Sql = ''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''] ALTER COLUMN [''+@ColumnName+''] datetimeoffset(7);''
					PRINT @Sql
					EXEC sp_executesql @statment=@Sql
				END
			END
			ELSE
			BEGIN

				SET @SqlInsertColumns = @SqlInsertColumns + ''['' + @ColumnName + ''],'';

				IF @isnewdateoffsetcol = 1
				BEGIN

					-- alter new table 
					SET @Sql = ''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''_DateMigrate] ALTER COLUMN [''+@ColumnName+''] datetimeoffset(7);''
					PRINT @Sql
					EXEC sp_executesql @statment=@Sql

					declare @tdzAlias nvarchar(10)
					SET @tzdCount = @tzdCount + 1;
					SET @tdzAlias = ''tdz'' + cast (@tzdCount as nvarchar(5));

					-- Learning from BSM-E, older versions of FuelsManager stored everything in UTC/GMT so just leave the offset at +0.00 and DO NOT adjust the DateTime to the Site timezone.  It will happen
					-- automatically.
					--SET @SqlInsertVals = @SqlInsertVals + '' TODATETIMEOFFSET(tb1.[''+@ColumnName+''],case when '' + @tdzAlias + '' .Offset is null or s.AdjustForDaylightSavings = 0 then tz.UTCOffSet else tz.utcoffset + '' + @tdzAlias + ''.Offset end), ''
					
					-- Keep the original UTC offset of 0
					SET @SqlInsertVals = @SqlInsertVals + '' TODATETIMEOFFSET(tb1.[''+@ColumnName+''], 0),''

					SET @SqlTdzJoins = @SqlTdzJoins + '' left join tbltimezonedstoffsets '' +@tdzAlias + '' on tz.[Index] = '' +@tdzAlias + ''.[TimeZoneIndex] AND tb1.[''+ @ColumnName +''] between '' +@tdzAlias + ''.StartTime AND '' +@tdzAlias + ''.EndTime ''
				END
				ELSE
				BEGIN
					SET @SqlInsertVals = @SqlInsertVals + ''tb1.['' + @ColumnName + ''],''
				END	
			END

			FETCH NEXT FROM ColumnCursor INTO @ColumnName,@isnewdateoffsetcol
		END
			
		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor

				
		DECLARE @siteQualifiedColumn nvarchar(50), @caseJoin nvarchar(max)
		IF @SiteIndexLevel > 0
		BEGIN

			IF @SiteIndexLevel=1
			BEGIN
			
				SET @siteQualifiedColumn = ''tb1.SiteIndex ''
				SET @caseJoin  = ''''
			END
		
			IF @SiteIndexLevel=2
			BEGIN
				SET @siteQualifiedColumn = ''tb2.SiteIndex ''

				IF @TableName=''tblTransactionAliasFields''
				BEGIN
				
					SET @caseJoin = '' INNER JOIN dbo.tblTransactionAliases tb2 ON tb2.AliasID=tb1.AliasID ''

				END
				ELSE
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblTransactions tb2 ON tb2.TransIndex=tb1.TransIndex ''
	
				END
			END
		
			IF @SiteIndexlevel = 3
			BEGIN

				SET @siteQualifiedColumn = ''tb2.SiteIndex ''

				IF @TableName=''tblAllocationLineItems''
				BEGIN
					SET @caseJoin ='' INNER JOIN dbo.tblAllocations tb2 ON tb2.[Index]=tb1.[Index] ''
				END

				ELSE IF @TableName = ''tblBulkPayments''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblSites tb2 ON tb2.[ID]=tb1.[Site] ''
				END

				ELSE IF @TableName = ''tblBulkPaymentLinks''
				BEGIN
					SET @siteQualifiedColumn = ''tb3.SiteIndex ''
					SET @caseJoin= '' INNER JOIN dbo.tblBulkPayments tb2 ON tb2.[BulkPaymentID]=tb1.[BulkPaymentID] INNER JOIN [dbo].[tblSites] tb3 ON tb3.[ID]=tb2.[Site] ''
				END

				ELSE IF @TableName = ''tblCurrencyLineItems''
				BEGIN
					SET @caseJoin ='' INNER JOIN dbo.tblCurrencies tb2 ON tb2.[CurrencyIndex]=tb1.[CurrencyIndex] ''
				END

				ELSE IF @TableName = ''tblExcise''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblProducts tb2 ON tb2.[ProductIndex]=tb1.[ProductIndex] ''
				END
			
				-- tblExportResultDetails
				ELSE IF @TableName = ''tblExportResultDetails''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblExportResults tb2 ON tb2.[Index]=tb1.[ExportResultIndex] ''
				END
			
				-- tblFilterViews
				ELSE IF @TableName = ''tblFilterViews''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblTransactionAliases tb2 ON tb2.[TransTypeID]=tb1.[TransTypeID] ''
				END
			
				-- tblGeneralConfigurationAliases
				ELSE IF @TableName = ''tblGeneralConfigurationAliases''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblGeneralConfiguration tb2 ON tb2.[GCIndex]=tb1.[GCIndex] ''
				END
			
				-- tblLoadArms
				ELSE IF @TableName = ''tblLoadArms''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblStations tb2 ON tb2.[Index]=tb1.[BayAStationIndex] ''
				END
			
				-- tblMessageLog
				ELSE IF @TableName = ''tblMessageLog''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblMessages tb2 ON tb2.[Index]=tb1.[MessageIndex] ''
				END
			
				-- tblSavedQueryItems
				ELSE IF @TableName = ''tblSavedQueryItems''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblSavedQueries tb2 ON tb2.[QueryIndex]=tb1.[QueryIndex] ''
				END
			
				-- tblTestDefinitions
				ELSE IF @TableName = ''tblTestDefinitions''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblSites tb2 ON tb2.[SiteIndex]=tb1.[OwnerSiteIndex] ''
				END
			
				-- tblTestEquipmentResults
				ELSE IF @TableName = ''tblTestEquipmentResults''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblTestSetEquipmentResults tb2 ON tb2.[TestSetEquipmentResultIndex]=tb1.[TestSetEquipmentResultIndex] ''
				END
			
				-- tblTestSetDefinitions
				ELSE IF @TableName = ''tblTestSetDefinitions''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblSites tb2 ON tb2.[SiteIndex]=tb1.[OwnerSiteIndex] ''
				END
			
				-- tblTestTankResults
				-- [tblTestSetTankResults] ([TestSetTankResultIndex]
				ELSE IF @TableName = ''tblTestTankResults''
				BEGIN
					SET @caseJoin = '' INNER JOIN dbo.tblTestSetTankResults tb2 ON tb2.[TestSetTankResultIndex]=tb1.[TestSetTankResultIndex] ''
				END
			
				-- ALL TABLES IN WHICH SITE INDEX CANNOT BE RESOLVED: USE SITEADMIN
				ELSE
				BEGIN
					SET @siteQualifiedColumn = ''-1''
					SET @caseJoin = ''''
				END

			END
			IF @TableName <> ''tblExportResultDetails'' AND @TableName <> ''tblAllocationLineItems'' AND @TableName <> ''tblArchivedUsers'' AND @TableName <>''tblChangesQueue''
			BEGIN
				--remove trailing commas
				SET @SqlInsertColumns = LEFT(@SqlInsertColumns, len(@SqlInsertColumns) -1);
				SET @SqlInsertVals = LEFT(@SqlInsertVals, len(@SqlInsertVals) - 1);
				SET @Sql = ''''
				DECLARE @hasIdentity bit
				SET @hasIdentity = 0

				IF EXISTS( select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = @SchemaName and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, ''IsIdentity'') = 1 AND TABLE_NAME = @TableName +''_DateMigrate'')
				BEGIN
				SET @hasIdentity = 1
				END

				IF (@hasIdentity = 1)
				BEGIN
					SET @Sql = ''SET IDENTITY_INSERT [''+@SchemaName+''].[''+@TableName+''_DateMigrate] ON ''
				END

				SET @Sql = @Sql + ''
					INSERT INTO [''+@SchemaName+''].[''+@TableName+''_DateMigrate] WITH (TABLOCK) ('' + @SqlInsertColumns + '')
					SELECT '' + @SqlInsertVals + '' FROM [''+@SchemaName+''].[''+@TableName+''] tb1 '' + @caseJoin + ''
					INNER JOIN [dbo].tblSites s on s.SiteIndex = ''+@siteQualifiedColumn + ''
						inner join tblTimeZones tz on  s.TimeZone = tz.Name '' +  @SqlTdzJoins 
						
				IF (@hasIdentity = 1)
				BEGIN
					SET @Sql = @Sql + ''
						SET IDENTITY_INSERT [''+@SchemaName+''].[''+@TableName+''_DateMigrate] OFF ''
				END
			
				PRINT cast(@Sql as ntext)
				EXEC sp_executesql @statment=@Sql
	
				SET @Sql = ''
					TRUNCATE TABLE [''+@SchemaName+''].[''+@TableName+'']
					DROP TABLE [''+@SchemaName+''].[''+@TableName+'']
					EXEC sp_rename ''''[''+@SchemaName+''].[''+@TableName+''_DateMigrate]'''', ''''''+@TableName+''''''''
				print @sql
				EXEC sp_executesql @statment=@Sql
			END
		END
	FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel
END
CLOSE TableCursor
DEALLOCATE TableCursor
PRINT ''Completed successfully''

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00015 Deploy User Defined Data Types]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00015 Deploy User Defined Data Types', 
		@step_id=7, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DECLARE @ErrorCount int=0
DECLARE @MSG NVARCHAR(MAX)

DECLARE @Sql NVARCHAR(max)


SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''lookup'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [lookup] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql


SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''sync'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [sync] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql



SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''track'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [track] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql


SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''erv'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [erv] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql

SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''map'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [map] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql

SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''rpt'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [rpt] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql

SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''fmaudit'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [fmaudit] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql

SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''maint'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [maint] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql

SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N''''fmcdc'''') ''
SET	@Sql+= ''   EXEC(''''CREATE SCHEMA [fmcdc] AUTHORIZATION [dbo]'''');''
PRINT ''''

PRINT @Sql
EXEC sp_executesql @statement=@Sql
	
/****** Object:  UserDefinedDataType [dbo].[udtUserID]    Script Date: 05/23/2013 09:06:57 ******/
IF  NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''udtUserID'' AND ss.name = N''dbo'')
	CREATE TYPE [dbo].[udtUserID] FROM [nvarchar](100) NULL


IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AlarmAndEventLogType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[AlarmAndEventLogType] AS TABLE(
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[Source] [nvarchar](120) NOT NULL,
	[Alarm] [bit] NOT NULL,
	[ID] [nvarchar](120) NOT NULL,
	[AssociatedData] [nvarchar](max) NOT NULL,
	[Acknowledged] [bit] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AlarmDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[AlarmDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmDataType] AS TABLE(
	[AlarmGuid] [uniqueidentifier] NOT NULL,
	[InputTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[Enabled] [Bit] Not NULL,
	[AlarmCategoryApplicationStringGuid] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[NotAlarmState] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](256) NULL,
	[ShelvedStartTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedEndTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedOneShot] [Bit]NOT NULL,
	[ShelvedBy] [dbo].[udtUserID]  NULL,
	[Suppressed] [Bit]NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[AlarmStateTagGuid] [uniqueidentifier] NOT NULL,
	[ExclusiveAlarm] BIT NOT NULL,
	[AlarmTemplateGuid]  [uniqueidentifier] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AlarmTemplateDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[AlarmTemplateDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmTemplateDataType] AS TABLE(
	[AlarmTemplateGuid] [uniqueidentifier] NOT NULL,
	[InputTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[Enabled] [Bit] Not NULL,
	[AlarmCategoryApplicationStringGuid] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[NotAlarmState] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](256) NULL,
	[ShelvedStartTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedEndTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedOneShot] [Bit]NOT NULL,
	[ShelvedBy] [dbo].[udtUserID]  NULL,
	[Suppressed] [Bit]NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[AlarmStateTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[ExclusiveAlarm] BIT NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AlarmTestDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[AlarmTestDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmTestDataType] AS TABLE(
	[AlarmTestGuid] [uniqueidentifier] NOT NULL,
	[AlarmGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[LimitTagGuid] [uniqueidentifier] NOT NULL,
	[TagField] INT NOT NULL,
	[AlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[NormalUnacknowledgedAlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[TestType] [int] NOT NULL,  -- this is an enum for the different comparison types. See slide 5
	[BitMask] BIGINT NOT NULL, 
	[Enabled] [Bit] Not NULL,
	[Order] [int] NOT NULL,
	[AlarmState] [nvarchar](100) NOT NULL,
	[Holdoff] [float] NOT NULL,  -- between 0 and 1 a percentage of the delta between the tag Max and Min.
	[AlarmText] [nvarchar](256) NULL,
	[HelpFile] [nvarchar](Max) NULL, 	
	[DrawingGuid] [uniqueidentifier] NULL,	
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[BitwiseOperator] [int] NOT NULL,  
	[TimedHoldOffInSeconds] [int] NOT NULL,
	[AlarmTestTemplateGuid] [uniqueidentifier] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AlarmTestTemplateDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[AlarmTestTemplateDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmTestTemplateDataType] AS TABLE(
	[AlarmTestTemplateGuid] [uniqueidentifier] NOT NULL,
	[AlarmTemplateGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[LimitTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[TagField] INT NOT NULL,
	[AlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[NormalUnacknowledgedAlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[TestType] [int] NOT NULL,  -- this is an enum for the different comparison types. See slide 5
	[BitMask] BIGINT NOT NULL, 
	[Enabled] [Bit] Not NULL,
	[Order] [int] NOT NULL,
	[AlarmState] [nvarchar](100) NOT NULL,
	[Holdoff] [float] NOT NULL,  -- between 0 and 1 a percentage of the delta between the tag Max and Min.
	[AlarmText] [nvarchar](256) NULL,
	[HelpFile] [nvarchar](Max) NULL, 	
	[DrawingGuid] [uniqueidentifier] NULL,	
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[BitwiseOperator] [int] NOT NULL,  
	[TimedHoldOffInSeconds] [int] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''AnimationDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[AnimationDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AnimationDataType] AS TABLE(
	[AnimationGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](50) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[AnimationTestGroupList] [nvarchar](max) Not NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''DataDictionaryDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[DataDictionaryDataType] AS TABLE(
	[Key] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
	[Value] [nvarchar](100) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''ExternalStationConnectionInformationType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[ExternalStationConnectionInformationType] AS TABLE(
	[ExternalStationGuid] [uniqueidentifier] NOT NULL,
	[LookupExternalStationStatusIndex] [int] NOT NULL,
	[LastSuccessfulConnection] [datetimeoffset](7) NULL,
	[LastConnectionAttempt] [datetimeoffset](7) NULL,
	[LastTransactionID] [bigint] NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''ExternalStationLogType]'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[ExternalStationLogType] AS TABLE(
	[ExternalStationLogGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[ExternalStationGuid] [uniqueidentifier] NOT NULL,
	[LogText] [nvarchar](max) NOT NULL,
	[LookupExternalStationLogTypeIndex] [int] NOT NULL,
	[LogDate] [datetimeoffset](7) NOT NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''ExternalStationTransactionErrorType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[ExternalStationTransactionErrorType] AS TABLE(
	[ExternalStationTransactionErrorGuid] [uniqueidentifier] NOT NULL,
	[ExternalStationTransactionGuid] [uniqueidentifier] NOT NULL,
	[Error] [nvarchar](1000) NOT NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''ExternalStationTransactionFailedStatusType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[ExternalStationTransactionFailedStatusType] AS TABLE(
	[ExternalStationTransactionGuid] [uniqueidentifier] NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NULL,
	[LookupExternalStationTransactionFailedStatusIndex] [int] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''ExternalStationTransactionType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[ExternalStationTransactionType] AS TABLE(
	[ExternalStationTransactionGuid] [uniqueidentifier] NULL,
	[ExternalStationGuid] [uniqueidentifier] NULL,
	[SiteGuid] [uniqueidentifier] NULL,
	[StationTransactionID] [nvarchar](20) NULL,
	[RawTransactionData] [nvarchar](max) NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NULL,
	[LookupExternalStationTransactionStatusIndex] [int] NULL,
	[LookupExternalStationTransactionFailedStatusIndex] [int] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''FindDuplicateExternalStationTransactionType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[FindDuplicateExternalStationTransactionType] AS TABLE(
	[StationTransactionID] [nvarchar](20) NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''GasboyDeviceToProduct'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[GasboyDeviceToProduct] AS TABLE(
	[GasboyDeviceGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[CreatedUpdatedDate] [datetimeoffset](7) NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''GasboyStationEventType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[GasboyStationEventType] AS TABLE(
	[GasboyStationEventGuid] [uniqueidentifier] NOT NULL,
	[ExternalStationLogGuid] [uniqueidentifier] NOT NULL,
	[EventID] [int] NULL,
	[LookupGasboyEventErrorClassCodeIndex] [int] NULL,
	[ErrorCode] [int] NULL,
	[FleetID] [int] NULL,
	[ObjectID] [int] NULL,
	[LookupGasboyEventObjectTypeIndex] [int] NULL,
	[DeviceName] [nvarchar](100) NULL,
	[Field1] [nvarchar](100) NULL,
	[Field2] [nvarchar](100) NULL,
	[Field3] [nvarchar](100) NULL,
	[Field4] [nvarchar](100) NULL,
	[Field5] [nvarchar](100) NULL,
	[Field6] [nvarchar](100) NULL,
	[Field7] [nvarchar](100) NULL,
	[Field8] [nvarchar](100) NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''GuidListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[GuidListType] AS TABLE(
	[Guid] [uniqueidentifier] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''IntegerListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[IntegerListType] AS TABLE(
	[value] [int] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''OwnerCloseoutType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[OwnerCloseoutType] AS TABLE
(
    [Site]               NVARCHAR (30)      NOT NULL,
    [ManagerName]        NVARCHAR (100)     NOT NULL,
	[OwnerName]          NVARCHAR (100)     NOT NULL,
    [ProductName]        NVARCHAR (30)      NOT NULL,
    [CloseoutDate]       DATE               NOT NULL,
	[SiteGuid]           UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid] UNIQUEIDENTIFIER   NULL,
    [OwnerCompanyGuid]   UNIQUEIDENTIFIER   NULL,
    [ProductGuid]        UNIQUEIDENTIFIER   NULL,
    [GrossBookInventory] FLOAT (53)         NULL,
    [NetBookInventory]   FLOAT (53)         NULL,
	[MassBookInventory]  FLOAT (53)         NULL,
    [GrossBookPrice]     FLOAT (53)         NULL,
    [NetBookPrice]       FLOAT (53)         NULL,
    [MassBookPrice]      FLOAT (53)         NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToAlarmTestDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToAlarmTestDataType] AS TABLE(
	PointAccessGroupToAlarmTestGuid [uniqueidentifier] NOT NULL,
	AlarmTestTemplateGuid [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[View] bit NOT NULL,
	[Acknowledge] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToExposedSettingDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToExposedSettingDataType] AS TABLE(
	[PointAccessGroupToExposedSettingGuid] [uniqueidentifier] NOT NULL,
	[ExposedSettingGuid] [uniqueidentifier] NOT NULL,
	[PropertyID] nvarchar( 60 ) NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[ValueType] int NOT NULL,
	[View] bit NOT NULL,
	[Modify] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToPointDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToPointDataType] AS TABLE(
	PointAccessGroupToPointGuid [uniqueidentifier] NOT NULL,
	PointAccessGroupGuid [uniqueidentifier] NOT NULL,
	PointGuid [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToPointTemplateDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToPointTemplateDataType] AS TABLE(
	PointAccessGroupToPointTemplateGuid [uniqueidentifier] NOT NULL,
	PointAccessGroupGuid [uniqueidentifier] NOT NULL,
	PointTemplateGuid [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToTagDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToTagDataType] AS TABLE(
	[PointAccessGroupToTagGuid] [uniqueidentifier] NOT NULL,
	TagGuid [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[View] bit NOT NULL,
	[Modify] bit NOT NULL,
	[ExceedRange] bit NOT NULL,
	[Override] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointAccessGroupToUserGroupDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointAccessGroupToUserGroupDataType] AS TABLE(
	[PointAccessGroupToUserGroupGuid] [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[UserGroupGuid] [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointTagAlarmStatusDataType'' AND ss.name = N''dbo'')
/****** Object:  UserDefinedTableType [dbo].[PointTagAlarmStatusDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[PointTagAlarmStatusDataType] AS TABLE(
	[PointTagAlarmStatusGuid] [uniqueidentifier] NOT NULL,
	[AlarmTestGuid] [uniqueidentifier] NOT NULL,
	[Acknowledged] [Bit] NOT NULL,
	[AcknowledgedTimestamp] [datetimeoffset](7) NULL,
	[AcknowledgedBy] [dbo].[udtUserID] NULL,
	[AcknowledgedComment] [nvarchar](MAX) NULL,
	[Silenced] [Bit] NOT NULL DEFAULT (0),
	[SilencedTimestamp] [DATETIMEOFFSET](7) NULL,
	[SilencedBy] [dbo].[udtUserID] NULL,
	[AlarmTestFailed] [BIT] NOT NULL,
	[AlarmTestFailedTimestamp] [DATETIMEOFFSET](7) NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointTagDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointTagDataType] AS TABLE
(
	[PointTagGuid] [uniqueidentifier] NOT NULL,
	[EngineeringUnitsType] [INT]	NULL,
	[EngineeringUnitsIndex]	[INT] NULL,
	[DecimalPlaces] [TINYINT]	NULL,
	[Maximum] [FLOAT]	NULL,
	[Minimum] [FLOAT]	NULL,
	[Value] [xml] NULL,
	[Status] [bigint] NULL,
	[ServerTimeStamp] [datetimeoffset](7) NULL,
	[SourceTimeStamp] [datetimeoffset](7) NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointTagIDListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointTagIDListType] AS TABLE(
	[ID] nvarchar( 50 )  NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointTagType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointTagType] AS TABLE
(
	[PointTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [NVarChar](50)	NULL,
	[EngineeringUnitsType]	[INT] NULL,
	[EngineeringUnitsIndex] [INT]	NULL,
	[DecimalPlaces] [TINYINT]	NULL,
	[ServerEngineeringUnitsIndex] [INT]	NULL,
	[ValueType] [NVarChar] (max) NULL,
	[Status] [bigint] NULL,
	[Value] [xml] NULL,
	[ServerTimeStamp] [datetimeoffset] NULL,
	[SourceTimeStamp] [datetimeoffset] NULL,
	[Maximum] [FLOAT] NULL,
	[Minimum] [FLOAT] NULL,
	[PointTagInputOutputTypeIndex] [INT] NULL,
	[Input] [BIT] NULL,
	[AlarmStatus] [BIT] NULL,
	[ApplyPointEngineeringUnits] [BIT] NULL,
	[ApplyPointDecimalPlaces] [BIT] NULL,
	[ApplyPointMaximum] [BIT] NULL,
	[ApplyPointMinimum] [BIT] NULL,
	[OpcUaServerGuid] [uniqueidentifier] NULL,
	[OpcUaBrowsePath] [NVARCHAR](250) NULL,
	[OpcUaNamespaceUri] [NVARCHAR](250) NULL,
	[OpcUaPublishingInterval] [INT] NULL,
	[OpcUaNodeId] [NVARCHAR](250) NULL,
	[OpcUaIsReadable] [BIT] NULL,
	[OpcUaServerDataType] [INT] NULL,
	[OpcUaWriteHoldoffTime] [INT] NULL,
	[OpcUaWritePeriodicUpdateInterval] [INT] NULL,
	[AlarmsEnabled] [BIT] NULL,
	[InhibitInputOutputTypeConfiguration] [BIT] NULL,
	[InhibitOverride] [BIT] NULL,
	[CreatedDate] [datetimeoffset] NULL,
	[CreatedBy] [udtUserID] NULL,
	[UpdatedDate] [datetimeoffset] NULL,
	[UpdatedBy] [udtUserID] NULL,
	[PointGuid] [uniqueidentifier] NULL,
	[PointTemplateTagGuid] [uniqueidentifier] NULL
)

/****** Object:  UserDefinedTableType [dbo].[PointTemplateTagAlarmStatusDataType]    Script Date: 12/22/2016 07:25:27 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''PointTemplateTagAlarmStatusDataType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[PointTemplateTagAlarmStatusDataType] AS TABLE(
	[PointTemplateTagAlarmStatusGuid] [uniqueidentifier] NOT NULL,
	[AlarmTestTemplateGuid] [uniqueidentifier] NOT NULL,
	[Acknowledged] [Bit] Not NULL,
	[AcknowledgedTimestamp] [datetimeoffset](7) NULL,
	[AcknowledgedBy] [dbo].[udtUserID] NULL,
	[AcknowledgedComment] [nvarchar](MAX) NULL,
	[Silenced] [Bit] Not NULL DEFAULT (0),
	[SilencedTimestamp] [datetimeoffset](7) NULL,
	[SilencedBy] [dbo].[udtUserID] NULL,
	[AlarmTestFailed] [Bit] Not NULL,
	[AlarmTestFailedTimestamp] [datetimeoffset](7) NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''StringListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[StringListType] AS TABLE(
	[value] nvarchar(MAX) NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransactionGuidAndLineItemSequenceListType]    Script Date: 10/26/2013 11:55:15 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionGuidAndLineItemSequenceListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionGuidAndLineItemSequenceListType] AS TABLE(
	[TransactionGuid] [uniqueidentifier] NOT NULL,
	[SequenceID] [smallint] NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransactionGuidAndSubLineItemSequenceListType]    Script Date: 10/26/2013 11:55:41 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionGuidAndSubLineItemSequenceListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionGuidAndSubLineItemSequenceListType] AS TABLE(
	[TransactionLineItemGuid] [uniqueidentifier] NOT NULL,
	[SequenceID] [smallint] NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransactionGuidAndTransportOrderNumberListType]    Script Date: 10/26/2013 11:56:03 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionGuidAndTransportOrderNumberListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionGuidAndTransportOrderNumberListType] AS TABLE(
	[TransactionGuid] [uniqueidentifier] NOT NULL,
	[TransportOrderNumber] [nvarchar](50) NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionGuidAndTransVersionListType]    Script Date: 1/14/2015 2:29:01 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionGuidAndTransVersionListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionGuidAndTransVersionListType] AS TABLE(
	[TransactionGuid] [uniqueidentifier] NOT NULL,
	[TransVersion] [bigint] NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionGuidListType]    Script Date: 1/14/2015 2:30:39 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionGuidListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionGuidListType] AS TABLE(
	[TransactionGuid] [uniqueidentifier] NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionHeadersType]    Script Date: 1/14/2015 2:32:19 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionHeadersType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionHeadersType] AS TABLE
(
	TransactionGuid UNIQUEIDENTIFIER NOT NULL,
	-- Transaction Header fields
	TransID NVARCHAR(64) NOT NULL,
	AliasName NVARCHAR(32) NULL,
	SubType NVARCHAR(20) NULL,
	Site NVARCHAR(30) NULL,
	TransReferenceID NVARCHAR(64) NULL,
	InventoryDate DATETIME NULL,
	ShipToID NVARCHAR(100) NULL,
	ShipToCode NVARCHAR(10) NULL,
	SupplierID NVARCHAR(100) NULL,
	SupplierCode NVARCHAR(10) NULL,
	RequestedDeliveryDate DATETIMEOFFSET(7) NULL,
	TransDateTime DATETIMEOFFSET(7) NULL,
	TransVersion BIGINT NULL,
	SCACCode NVARCHAR(4) NULL,
	CardNumber NVARCHAR(30) NULL,
	ShipmentNumber NVARCHAR(30) NULL,
	ShipperID NVARCHAR(100) NULL,
	ShipperCode NVARCHAR(10) NULL,
	OwnerID NVARCHAR(100) NULL,
	OwnerCode NVARCHAR(10) NULL,
	ManagerID NVARCHAR(100) NULL,
	ManagerCode NVARCHAR(10) NULL,
	CarrierID NVARCHAR(100) NULL,
	CarrierCode NVARCHAR(10) NULL,
	ConjoinTransID NVARCHAR(64) NULL,
	ReversedTransID NVARCHAR(64) NULL,
	LinkedDocumentNumber NVARCHAR(64) NULL,
	ReversalType NVARCHAR(2) NULL,
	PONumber NVARCHAR(14) NULL,
	TimeIn DATETIMEOFFSET(7) NULL,
	TimeOut DATETIMEOFFSET(7) NULL,
	TimeEnd DATETIMEOFFSET(7) NULL,
	RoutingID NVARCHAR(30) NULL,
	TicketSource NVARCHAR(20) NULL,
	LoadID NVARCHAR(50) NULL,
	BillToID NVARCHAR(100) NULL,
	BillToCode NVARCHAR(10) NULL,
	DriverIdentificationNumber NVARCHAR(50) NULL,
	CreditAmount FLOAT NULL,
	CardExpiration DATETIMEOFFSET(7) NULL,
	CardName NVARCHAR(30) NULL,
	CardType NVARCHAR(30) NULL,
	CashAmount FLOAT NULL,
	RouteOriginationDate DATETIMEOFFSET(7) NULL,
	InternationalRouteIndicator BIT NULL,
	PreviousRoutingID NVARCHAR(30) NULL,
	ShippingDocumentNumber NVARCHAR(30) NULL,
	DocumentNumber NVARCHAR(30) NULL,
	STD DATETIMEOFFSET(7) NULL,
	ETD DATETIMEOFFSET(7) NULL,
	STA DATETIMEOFFSET(7) NULL,
	ETA DATETIMEOFFSET(7) NULL,
	SFT DATETIMEOFFSET(7) NULL,
	FST DATETIMEOFFSET(7) NULL,
	EstimatedFuelingDuration INT NULL,
	DeleteFlag BIT NULL,
	TicketMode NVARCHAR(15) NULL,
	DestinationRegistrationID1 NVARCHAR(30) NULL,
	DestinationSerialNumber1 NVARCHAR(10) NULL,
	DestinationEquipmentType1 NVARCHAR(50) NULL,
	DestinationEquipmentModel1 NVARCHAR(20) NULL,
	DestinationCompanyEquipmentID1 NVARCHAR(30) NULL,
	DestinationRegistrationID2 NVARCHAR(30) NULL,
	DestinationSerialNumber2 NVARCHAR(10) NULL,
	DestinationEquipmentType2 NVARCHAR(50) NULL,
	DestinationEquipmentModel2 NVARCHAR(20) NULL,
	DestinationCompanyEquipmentID2 NVARCHAR(30) NULL,
	DestinationRegistrationID3 NVARCHAR(30) NULL,
	DestinationSerialNumber3 NVARCHAR(10) NULL,
	DestinationEquipmentType3 NVARCHAR(50) NULL,
	DestinationEquipmentModel3 NVARCHAR(20) NULL,
	DestinationCompanyEquipmentID3 NVARCHAR(30) NULL,
	SourceRegistrationID1 NVARCHAR(30) NULL,
	SourceSerialNumber1 NVARCHAR(10) NULL,
	SourceEquipmentType1 NVARCHAR(50) NULL,
	SourceEquipmentModel1 NVARCHAR(20) NULL,
	SourceCompanyEquipmentID1 NVARCHAR(30) NULL,
	SourceRegistrationID2 NVARCHAR(30) NULL,
	SourceSerialNumber2 NVARCHAR(10) NULL,
	SourceEquipmentType2 NVARCHAR(50) NULL,
	SourceEquipmentModel2 NVARCHAR(20) NULL,
	SourceCompanyEquipmentID2 NVARCHAR(30) NULL,
	SourceRegistrationID3 NVARCHAR(30) NULL,
	SourceSerialNumber3 NVARCHAR(10) NULL,
	SourceEquipmentType3 NVARCHAR(50) NULL,
	SourceEquipmentModel3 NVARCHAR(20) NULL,
	SourceCompanyEquipmentID3 NVARCHAR(30) NULL,
	OperatorID NVARCHAR(50) NULL,
	EffectiveDate DATETIMEOFFSET(7) NULL,
	ExpirationDate DATETIMEOFFSET(7) NULL,
	ScheduledDate DATETIMEOFFSET(7) NULL,
	AutoComplete BIT NULL,
	Flag01 BIT NULL,
	Flag02 BIT NULL,
	Flag03 BIT NULL,
	Flag04 BIT NULL,
	Flag05 BIT NULL,
	Flag06 BIT NULL,
	Number01 FLOAT NULL,
	Number02 FLOAT NULL,
	Number03 FLOAT NULL,
	Number04 FLOAT NULL,
	Number05 FLOAT NULL,
	Number06 FLOAT NULL,
	ContactFirstName NVARCHAR(50) NULL,
	ContactSurname NVARCHAR(50) NULL,
	Date01 DATETIMEOFFSET(7) NULL,
	Date02 DATETIMEOFFSET(7) NULL,
	Date03 DATETIMEOFFSET(7) NULL,
	Date04 DATETIMEOFFSET(7) NULL,
	LegacyNumber NVARCHAR(50) NULL,
	Country NVARCHAR(50) NULL,
	ContactInfo NVARCHAR(50) NULL,
	AssociatedDocNumber NVARCHAR(30) NULL,
	AssociatedCLIN NVARCHAR(10) NULL,
	SubmittedToAccounting BIT NULL,
	FuelCardID NVARCHAR(50) NULL,
	AssociatedTransportOrderNumber NVARCHAR(30) NULL,
	RequestedDateTime DATETIMEOFFSET(7) NULL,
	DispatchedDateTime DATETIMEOFFSET(7) NULL,
	ErrorFlag BIT NULL,
	SiteGuid UNIQUEIDENTIFIER NULL,
	LookupTransTypeIndex SMALLINT NULL,
	LookupTransactionStatusIndex INT NULL,
	LookupOriginApplicationIndex INT NULL,
	TransactionAliasGuid UNIQUEIDENTIFIER NULL,
	BillToCompanyGuid UNIQUEIDENTIFIER NULL,
	Destination1EquipmentGuid UNIQUEIDENTIFIER NULL,
	Destination2EquipmentGuid UNIQUEIDENTIFIER NULL,
	Destination3EquipmentGuid UNIQUEIDENTIFIER NULL,
	FinalStationIATAGuid UNIQUEIDENTIFIER NULL,
	FuelCardGuid UNIQUEIDENTIFIER NULL,
	ManagerCompanyGuid UNIQUEIDENTIFIER NULL,
	NextStationIATAGuid UNIQUEIDENTIFIER NULL,
	OperatorPersonnelGuid UNIQUEIDENTIFIER NULL,
	OriginStationIATAGuid UNIQUEIDENTIFIER NULL,
	OwnerCompanyGuid UNIQUEIDENTIFIER NULL,
	PreviousStationIATAGuid UNIQUEIDENTIFIER NULL,
	ShipperCompanyGuid UNIQUEIDENTIFIER NULL,
	ShipToCompanyGuid UNIQUEIDENTIFIER NULL,
	Source1EquipmentGuid UNIQUEIDENTIFIER NULL,
	Source2EquipmentGuid UNIQUEIDENTIFIER NULL,
	Source3EquipmentGuid UNIQUEIDENTIFIER NULL,
	SupplierCompanyGuid UNIQUEIDENTIFIER NULL,
	CarrierCompanyGuid UNIQUEIDENTIFIER NULL,
	ReasonCodeGuid UNIQUEIDENTIFIER NULL,
	OriginStationIATAID NVARCHAR(50) NULL,
	PreviousStationIATAID NVARCHAR(50) NULL,
	NextStationIATAID NVARCHAR(50) NULL,
	FinalStationIATAID NVARCHAR(50) NULL,
	OperatorName NVARCHAR(150) NULL,
	FuelAdditiveFlag BIT NULL,
	IssuePoint NVARCHAR(MAX) NULL,
	IssuePointNumber NVARCHAR(MAX) NULL,
	RadioNumber NVARCHAR(MAX) NULL,
	GateID NVARCHAR(10) NULL,
	GateGuid UNIQUEIDENTIFIER NULL,
	ShippingMethod NVARCHAR(150) NULL,
	-- Transaction User data fields
	TransactionUserDataGuid UNIQUEIDENTIFIER NULL,
	UserData1 NVARCHAR(MAX) NULL,
	UserData2 NVARCHAR(MAX) NULL,
	UserData3 NVARCHAR(MAX) NULL,
	UserData4 NVARCHAR(MAX) NULL,
	UserData5 NVARCHAR(MAX) NULL,
	UserData6 NVARCHAR(MAX) NULL,
	UserData7 NVARCHAR(MAX) NULL,
	UserData8 NVARCHAR(MAX) NULL,
	UserData9 NVARCHAR(MAX) NULL,
	UserData10 NVARCHAR(MAX) NULL,
	UserData11 NVARCHAR(MAX) NULL,
	UserData12 NVARCHAR(MAX) NULL,
	UserData13 NVARCHAR(MAX) NULL,
	UserData14 NVARCHAR(MAX) NULL,
	UserData15 NVARCHAR(MAX) NULL,
	UserData16 NVARCHAR(MAX) NULL,
	UserData17 NVARCHAR(MAX) NULL,
	UserData18 NVARCHAR(MAX) NULL,
	UserData19 NVARCHAR(MAX) NULL,
	UserData20 NVARCHAR(MAX) NULL,
	UserData21 NVARCHAR(MAX) NULL,
	UserData22 NVARCHAR(MAX) NULL,
	UserData23 NVARCHAR(MAX) NULL,
	UserData24 NVARCHAR(MAX) NULL,
	-- Transaction Notes Fields
	TransactionNoteGuid UNIQUEIDENTIFIER NULL,
	Notes NVARCHAR(1000) NULL,
	AdditionalInformation NVARCHAR(1000) NULL,
	-- Transaction Signature Fields
	TransactionSignatureGuid UNIQUEIDENTIFIER NULL,
	Signature VARBINARY(MAX) NULL,
	-- Fields commmon to all records
	CreatedUpdatedBy udtUserID NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionLineItemsType]    Script Date: 1/14/2015 2:33:53 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionLineItemsType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionLineItemsType] AS TABLE
(
	TransactionLineItemGuid UNIQUEIDENTIFIER NOT NULL,
	TransactionGuid UNIQUEIDENTIFIER NOT NULL,
	-- Transaction Line Item fields
	SequenceID SMALLINT NULL,
	MeterStart FLOAT NULL,
	MeterStop FLOAT NULL,
	GrossQuantity FLOAT NULL,
	Temperature FLOAT NULL,
	Vcf FLOAT NULL,
	Density FLOAT NULL,
	Product NVARCHAR(30) NULL,
	ProductCode NVARCHAR(30) NULL,
	ProductType NVARCHAR(20) NULL,
	ProductPrice FLOAT NULL,
	CLIN NVARCHAR(10) NULL,
	NetQuantity FLOAT NULL,
	ContractNumber NVARCHAR(30) NULL,
	DestinationRegistrationID NVARCHAR(30) NULL,
	DestinationSerialNumber NVARCHAR(10) NULL,
	DestinationEquipmentType NVARCHAR(50) NULL,
	DestinationEquipmentModel NVARCHAR(20) NULL,
	DestinationCompanyEquipmentID NVARCHAR(30) NULL,
	DestinationCompartmentID NVARCHAR(50) NULL,
	SourceRegistrationID NVARCHAR(30) NULL,
	SourceSerialNumber NVARCHAR(10) NULL,
	SourceEquipmentType NVARCHAR(50) NULL,
	SourceEquipmentModel NVARCHAR(20) NULL,
	SourceCompanyEquipmentID NVARCHAR(30) NULL,
	SourceCompartmentID NVARCHAR(50) NULL,
	MeterFactor FLOAT NULL,
	BatchNumber NVARCHAR(20) NULL,
	DocumentNumber NVARCHAR(30) NULL,
	LineFill FLOAT NULL,
	BottomVolume FLOAT NULL,
	NetCapacity FLOAT NULL,
	Customs NVARCHAR(20) NULL,
	ArmNumber INT NULL,
	LineNumber INT NULL,
	OperatorID NVARCHAR(50) NULL,
	TankStatus NVARCHAR(30) NULL,
	MeterStartDateTime DATETIMEOFFSET(7) NULL,
	MeterStopDateTime DATETIMEOFFSET(7) NULL,
	Pit NVARCHAR(10) NULL,
	RequestedDateTime DATETIMEOFFSET(7) NULL,
	DispatchedDateTime DATETIMEOFFSET(7) NULL,
	AcknowledgedDateTime DATETIMEOFFSET(7) NULL,
	OnLocationTime DATETIMEOFFSET(7) NULL,
	ValidationDateTime DATETIMEOFFSET(7) NULL,
	CompletionDateTime DATETIMEOFFSET(7) NULL,
	ReceiptVariance FLOAT NULL,
	DifferentialPressure FLOAT NULL,
	LoadRackVariance FLOAT NULL,
	RequestedBy NVARCHAR(50) NULL,
	FreezePoint FLOAT NULL,
	DeleteFlag BIT NULL,
	StorageLocationID NVARCHAR(50) NULL,
	MeterID NVARCHAR(50) NULL,
	AdditiveProfileID NVARCHAR(50) NULL,
	PresetAmount FLOAT NULL,
	EngineeringUnitsIndex INT NULL,
	CustomerProductName NVARCHAR(50) NULL,
	CustomerProductCode NVARCHAR(20) NULL,
	TransactionInventoryDate DATETIME NULL,
	COAWaiver BIT NULL,
	COANote NVARCHAR(50) NULL,
	COAID NVARCHAR(40) NULL,
	Tax1 FLOAT NULL,
	Tax2 FLOAT NULL,
	Tax3 FLOAT NULL,
	Tax4 FLOAT NULL,
	Tax5 FLOAT NULL,
	TransVersion BIGINT NULL,
	LoadingLocationID NVARCHAR(30) NULL,
	ImproperAdditization BIT NULL,
	BrokenBlend BIT NULL,
	ContaminatePrompt BIT NULL,
	CompartmentsPreviouslyLoaded BIT NULL,
	CompartmentsEmpty BIT NULL,
	Flag01 BIT NULL,
	Flag02 BIT NULL,
	Flag03 BIT NULL,
	Flag04 BIT NULL,
	Flag05 BIT NULL,
	Flag06 BIT NULL,
	Number01 FLOAT NULL,
	Number02 FLOAT NULL,
	Number03 FLOAT NULL,
	Number04 FLOAT NULL,
	Number05 FLOAT NULL,
	Number06 FLOAT NULL,
	OdometerHours FLOAT NULL,
	EndDeliveryDate DATETIMEOFFSET(7) NULL,
	RequestedDeliveryDate DATETIMEOFFSET(7) NULL,
	InvoiceNumber NVARCHAR(50) NULL,
	InvoiceLineNumber NVARCHAR(50) NULL,
	AlternativeGrossVolume FLOAT NULL,
	AlternativeNetVolume FLOAT NULL,
	AlternativeUnits INT NULL,
	TankLevel FLOAT NULL,
	TankLevelUnits INT NULL,
	Date01 DATETIMEOFFSET(7) NULL,
	Date02 DATETIMEOFFSET(7) NULL,
	Date03 DATETIMEOFFSET(7) NULL,
	Date04 DATETIMEOFFSET(7) NULL,
	NonDomesticPrice FLOAT NULL,
	ExchangeRate FLOAT NULL,
	QualityTestNumber NVARCHAR(50) NULL,
	Odometer FLOAT NULL,
	DeliveryLocation NVARCHAR(50) NULL,
	Variance FLOAT NULL,
	PartialFill BIT NULL,
	MassQuantity FLOAT NULL,
	NetManualValueFlag BIT NULL,
	MassManualValueFlag BIT NULL,
	GrossManualValueFlag BIT NULL,
	VcfManualValueFlag BIT NULL,
	LookupTransactionStatusIndex INT NULL,
	LookupQualityIndex INT NULL,
	StorageLocationTankGuid UNIQUEIDENTIFIER NULL,
	AdditiveProfileGuid UNIQUEIDENTIFIER NULL,
	DestinationCompartmentEquipmentGuid UNIQUEIDENTIFIER NULL,
	DestinationEquipmentGuid UNIQUEIDENTIFIER NULL,
	OperatorPersonnelGuid UNIQUEIDENTIFIER NULL,
	ProductGuid UNIQUEIDENTIFIER NULL,
	SourceCompartmentEquipmentGuid UNIQUEIDENTIFIER NULL,
	SourceEquipmentGuid UNIQUEIDENTIFIER NULL,
	CurrencyGuid UNIQUEIDENTIFIER NULL,
	OrderReferenceTransactionLineItemGuid UNIQUEIDENTIFIER NULL,
	LoadingLocationStationGuid UNIQUEIDENTIFIER NULL,
	MeterGuid UNIQUEIDENTIFIER NULL,
	PackageManualValueFlag BIT NULL,
	CleanLineItem BIT NULL,
	CleanLineDeductItem BIT NULL,
	CleanLineDeductQuantity FLOAT NULL,
	CleanLinePackQuantity FLOAT NULL,
	DualFuelingModeFlag BIT NULL,
	DualFuelingPrimaryFlag BIT NULL,
	EngineRunTime FLOAT NULL,
	FlowRate FLOAT NULL,
	FuelCompressionFactor FLOAT NULL,
	HydrantPressure FLOAT NULL,
	MobileDeviceID NVARCHAR(50) NULL,
	MobileDeviceGuid UNIQUEIDENTIFIER NULL,
	TemperatureQualityStatus NVARCHAR(50) NULL,
	MeterStartObtainedAutomaticallyFlag BIT NULL,
	MeterStopObtainedAutomaticallyFlag BIT NULL,
	-- Transaction Line Item User data fields
	TransactionLineItemUserDataGuid UNIQUEIDENTIFIER NULL,
	UserData1 NVARCHAR(60) NULL,
	UserData2 NVARCHAR(60) NULL,
	UserData3 NVARCHAR(60) NULL,
	UserData4 NVARCHAR(60) NULL,
	UserData5 NVARCHAR(60) NULL,
	UserData6 NVARCHAR(60) NULL,
	UserData7 NVARCHAR(60) NULL,
	UserData8 NVARCHAR(60) NULL,
	UserData9 NVARCHAR(60) NULL,
	UserData10 NVARCHAR(60) NULL,
	UserData11 NVARCHAR(60) NULL,
	UserData12 NVARCHAR(60) NULL,
	UserData13 NVARCHAR(60) NULL,
	UserData14 NVARCHAR(60) NULL,
	UserData15 NVARCHAR(60) NULL,
	UserData16 NVARCHAR(60) NULL,
	UserData17 NVARCHAR(60) NULL,
	UserData18 NVARCHAR(60) NULL,
	UserData19 NVARCHAR(60) NULL,
	UserData20 NVARCHAR(60) NULL,
	UserData21 NVARCHAR(60) NULL,
	UserData22 NVARCHAR(60) NULL,
	UserData23 NVARCHAR(60) NULL,
	UserData24 NVARCHAR(60) NULL,
	-- Fields commmon to all records
	CreatedUpdatedBy udtUserID NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionLinksDeleteType]    Script Date: 1/14/2015 2:37:46 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionLinksDeleteType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionLinksDeleteType] AS TABLE(
	[OriginalTransID] [nvarchar](64) NULL,
	[LinkedTransID] [nvarchar](64) NOT NULL,
	[TransactionLineItemGuid] [uniqueidentifier] NULL,
	[LinkedTransactionLineItemGuid] [uniqueidentifier] NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionLinksType]    Script Date: 1/14/2015 2:38:48 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionLinksType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionLinksType] AS TABLE(
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[OriginalTransID] [nvarchar](64) NOT NULL,
	[LinkedTransID] [nvarchar](64) NOT NULL,
	[Level] [int] NOT NULL,
	[TransactionLineItemGuid] [uniqueidentifier] NOT NULL,
	[LinkedTransactionLineItemGuid] [uniqueidentifier] NOT NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransactionPIDXsType]    Script Date: 1/14/2015 2:39:45 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionPIDXsType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionPIDXsType] AS TABLE(
	[TransactionPIDXGuid] [uniqueidentifier] NULL,
	[TransactionGuid] [uniqueidentifier] NULL,
	[AuthorizationNumber] [nvarchar](8) NULL,
	[SentFlag] [bit] NULL,
	[DateSent] [datetimeoffset](7) NULL,
	[BrokenBlend] [bit] NULL,
	[PIDXProfileGuid] [uniqueidentifier] NULL,
	[CompanyPersonnelToShipToBillToGuid] [uniqueidentifier] NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionSubLineItemsType]    Script Date: 1/14/2015 2:41:11 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionSubLineItemsType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionSubLineItemsType] AS TABLE
(
	TransactionSubLineItemGuid UNIQUEIDENTIFIER NOT NULL,
	TransactionLineItemGuid UNIQUEIDENTIFIER NULL,
	TransactionGuid UNIQUEIDENTIFIER NULL,
	SequenceID INT NOT NULL,
	Product NVARCHAR(30) NULL,
	ProductCode NVARCHAR(50) NULL,
	ProductType NVARCHAR(20) NULL,
	GrossQuantity FLOAT NULL,
	NetQuantity FLOAT NULL,
	Vcf FLOAT NULL,
	Density FLOAT NULL,
	Temperature FLOAT NULL,
	Customs NVARCHAR(20) NULL,
	ArmNumber INT NULL,
	LineNumber INT NULL,
	BatchNumber NVARCHAR(20) NULL,
	LineFill FLOAT NULL,
	BottomVolume FLOAT NULL,
	NetCapacity FLOAT NULL,
	TankStatus NVARCHAR(30) NULL,
	MeterFactor FLOAT NULL,
	MeterStart FLOAT NULL,
	MeterStop FLOAT NULL,
	MeterStopDateTime DATETIMEOFFSET(7) NULL,
	MeterStartDateTime DATETIMEOFFSET(7) NULL,
	FreezePoint FLOAT NULL,
	DifferentialPressure FLOAT NULL,
	DosageRate FLOAT NULL,
	DeleteFlag BIT NULL,
	PresetAmount FLOAT NULL,
	StorageLocationID NVARCHAR(50) NULL,
	MeterID NVARCHAR(50) NULL,
	COAID NVARCHAR(40) NULL,
	TransactionInventoryDate DATETIME NULL,
	Tax1 FLOAT NULL,
	Tax2 FLOAT NULL,
	Tax3 FLOAT NULL,
	Tax4 FLOAT NULL,
	Tax5 FLOAT NULL,
	TransVersion BIGINT NULL,
	ImproperAdditization BIT NULL,
	BrokenBlend BIT NULL,
	Flag01 BIT NULL,
	Flag02 BIT NULL,
	Flag03 BIT NULL,
	Flag04 BIT NULL,
	Flag05 BIT NULL,
	Flag06 BIT NULL,
	Number01 FLOAT NULL,
	Number02 FLOAT NULL,
	Number03 FLOAT NULL,
	Number04 FLOAT NULL,
	Number05 FLOAT NULL,
	Number06 FLOAT NULL,
	Date01 DATETIMEOFFSET(7) NULL,
	Date02 DATETIMEOFFSET(7) NULL,
	Date03 DATETIMEOFFSET(7) NULL,
	Date04 DATETIMEOFFSET(7) NULL,
	MassQuantity FLOAT NULL,
	NetManualValueFlag BIT NULL,
	MassManualValueFlag BIT NULL,
	GrossManualValueFlag BIT NULL,
	VcfManualValueFlag BIT NULL,
	LookupTransactionStatusIndex INT NULL,
	LookupQualityIndex INT NULL,
	ProductGuid UNIQUEIDENTIFIER NULL,
	StorageLocationTankGuid UNIQUEIDENTIFIER NULL,
	MeterGuid UNIQUEIDENTIFIER NULL,
	PackageManualValueFlag BIT NULL,
	CleanLineItem BIT NULL,
	CleanLineDeductItem BIT NULL,
	CleanLineDeductQuantity FLOAT NULL,
	CleanLinePackQuantity FLOAT NULL,
	CreatedUpdatedBy udtUserID NOT NULL
)


/****** Object:  UserDefinedTableType [dbo].[TransactionTransportLineItemsType]    Script Date: 1/14/2015 2:42:15 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionTransportLineItemsType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionTransportLineItemsType] AS TABLE(
	[TransactionTransportLineItemGuid] [uniqueidentifier] NULL,
	[TransactionGuid] [uniqueidentifier] NULL,
	[TransportOrderNumber] [nvarchar](50) NULL,
	[TransVersion] [bigint] NULL,
	[LocationName] [nvarchar](30) NULL,
	[Address1] [nvarchar](60) NULL,
	[Address2] [nvarchar](60) NULL,
	[City] [nvarchar](20) NULL,
	[State] [nvarchar](20) NULL,
	[Zip] [nvarchar](11) NULL,
	[POCName] [nvarchar](50) NULL,
	[POCPhone] [nvarchar](20) NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransactionWeightReadingsType]    Script Date: 1/14/2015 2:43:28 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransactionWeightReadingsType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransactionWeightReadingsType] AS TABLE(
	[TransactionGuid] [uniqueidentifier] NOT NULL,
	[CompartmentID] [nvarchar](30) NOT NULL,
	[BeginQuantityValue] [float] NULL,
	[RequestedQuantityValue] [float] NULL,
	[FinalQuantityValue] [float] NULL,
	[SourceVersionNumber] [int] NULL,
	[HistoricalFlag] [bit] NOT NULL,
	[TransVersion] [bigint] NULL,
	[VolumetricTopOffFlag] [bit] NULL,
	[CreatedUpdatedBy] [dbo].[udtUserID] NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[TransIDListType]    Script Date: 10/26/2013 11:56:27 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''TransIDListType'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[TransIDListType] AS TABLE(
	[TransID] [nvarchar](64) NOT NULL
)

/****** Object:  UserDefinedTableType [dbo].[utt_EquipmentType]    Script Date: 05/23/2013 09:07:45 ******/
IF  NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_EquipmentType'' AND ss.name = N''dbo'')
	CREATE TYPE [dbo].[utt_EquipmentType] AS TABLE(
	[EquipmentType] [int] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_PointValueIdentifier'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[utt_PointValueIdentifier] AS TABLE
(
	[Guid] UNIQUEIDENTIFIER NOT NULL,
	[PropertyId] NVARCHAR(50),
	[ValueType] TINYINT
)

/****** Object:  UserDefinedTableType [dbo].[utt_RelatedGuidParameters]    Script Date: 10/26/2013 11:56:51 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_RelatedGuidParameters'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[utt_RelatedGuidParameters] AS TABLE(
	[Section] [int] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[TransId] [nvarchar](100) NOT NULL,
	[EntityId] [nvarchar](100) NULL,
	[EntityType] [nvarchar](100) NULL,
	[EntityGuid] [uniqueidentifier] NULL,
	[Identifier] [nvarchar](100) NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_UserGroupADMapping'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[utt_UserGroupADMapping] AS TABLE(
	[UserId] [nvarchar](100) NULL,
	[UserGroupGuid] [uniqueidentifier] NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_UserADMapping'' AND ss.name = N''dbo'')
CREATE TYPE [dbo].[utt_UserADMapping] AS TABLE(
	[UserId] [nvarchar](100) NULL,
	[SiteGuid] [uniqueidentifier] NULL
)

/****** Object:  UserDefinedTableType [erv].[utt_EntityRecordVersions]    Script Date: 10/26/2013 11:57:13 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_EntityRecordVersions'' AND ss.name = N''erv'')
CREATE TYPE [erv].[utt_EntityRecordVersions] AS TABLE(
	[EntityTypeId] [nvarchar](100) NULL,
	[SiteGuid] [uniqueidentifier] NULL,
	[MasterRecordGuid] [uniqueidentifier] NULL,
	[EntityGuid] [uniqueidentifier] NULL,
	[EntitySegmentTemplateGuid] [uniqueidentifier] NULL,
	[FilterFieldName] [nvarchar](100) NULL,
	[FilterValueGuid] [uniqueidentifier] NULL,
	[FilterValueName] [nvarchar](100) NULL
)

/****** Object:  UserDefinedTableType [erv].[utt_FieldLevelConfig]    Script Date: 05/23/2013 09:08:11 ******/
IF  NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_FieldLevelConfig'' AND ss.name = N''erv'')

CREATE TYPE [erv].[utt_FieldLevelConfig] AS TABLE(
	[FieldConfigGuid] [uniqueidentifier] NULL,
	[EntitySegmentTemplateGuid] [uniqueidentifier] NULL,
	[EntityTypeId] [nvarchar](100) NULL,
	[SiteGroupGuid] [uniqueidentifier] NULL,
	[FilterFieldName] [nvarchar](100) NULL,
	[FilterValueGuid] [uniqueidentifier] NULL,
	[FilterValueName] [nvarchar](100) NULL,
	[TargetField] [nvarchar](100) NULL,
	[IsExternalAttribute] [bit] NULL,
	[InternalFieldName] [nvarchar](100) NULL,
	[InheritedControlMode] [nvarchar](100) NULL,
	[ForwardControlMode] [nvarchar](100) NULL,
	[HierarchyLevel] [int] NULL
)

/****** Object:  UserDefinedTableType [erv].[utt_SiteList]    Script Date: 05/23/2013 09:08:38 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''utt_SiteList'' AND ss.name = N''erv'')
CREATE TYPE [erv].[utt_SiteList] AS TABLE(
	[SiteGuid] [uniqueidentifier] NULL
)

/****** Object:  UserDefinedTableType [map].[FMAECompanyIDType]    Script Date: 1/14/2015 2:44:46 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''FMAECompanyIDType'' AND ss.name = N''map'')
CREATE TYPE [map].[FMAECompanyIDType] AS TABLE(
	[FMAECompanyID] [nvarchar](100) NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[UserID] [dbo].[udtUserID] NOT NULL
)

/****** Object:  UserDefinedTableType [map].[FMAEProductIDType]    Script Date: 1/14/2015 2:45:41 PM ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''FMAEProductIDType'' AND ss.name = N''map'')
CREATE TYPE [map].[FMAEProductIDType] AS TABLE(
	[FMAEProductID] [nvarchar](30) NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[UserID] [dbo].[udtUserID] NOT NULL
)

/****** Object:  UserDefinedTableType [map].[MapAnimtionatToDrawingDataType]    Script Date: 12/22/2016 07:25:27 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''MapAnimationToDrawingDataType'' AND ss.name = N''map'')
CREATE TYPE [map].[MapAnimationToDrawingDataType] AS TABLE(
	[AnimationToDrawingGuid] [uniqueidentifier] NOT NULL,
	[AnimationGuid] [uniqueidentifier] NOT NULL,
	[DrawingGuid] [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)

IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N''UploadedReportsType'' AND ss.name = N''rpt'')
CREATE TYPE [rpt].[UploadedReportsType] AS TABLE(
	[ReportName] [nvarchar](256) NULL,
	[CommandText] [nvarchar](256) NULL,
	[CommandType] [nvarchar](256) NULL
)

GO


PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00018 Merge Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00018 Merge Tables', 
		@step_id=8, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	TAS Database Migration To v10.x
	SCRIPT TO Create Lookup tables
	Author: Aloisio(Al) dos Santos
*/

/*
PRINT ''Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
EXECUTE sp_rename N''dbo.tblExportResultDetails'', N''tblExportInterfaceResult'', ''OBJECT'' 

PRINT ''p1 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''

SELECT * INTO dbo.tblExportResultDetails FROM dbo.tblExportInterfaceResult WHERE 0=1

PRINT ''p2 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
EXECUTE sp_rename N''dbo.tblExportInterfaceResult.[Index]'', N''ExportInterfaceResultIndex'', ''COLUMN'' 

PRINT ''p3 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''

ALTER TABLE dbo.tblExportInterfaceResult ADD SiteIndex int null --NOT NULL
ALTER TABLE dbo.tblExportInterfaceResult ADD TransDateTime datetime null --NOT NULL
ALTER TABLE dbo.tblExportInterfaceResult ADD InterfaceName nvarchar(150) null --NOT NULL
ALTER TABLE dbo.tblExportInterfaceResult ADD ExportResultTypeIndex int null --NOT NULL
ALTER TABLE dbo.tblExportInterfaceResult ADD ArchiveFileName nvarchar(150)  NULL
ALTER TABLE dbo.tblExportInterfaceResult ADD BatchID nvarchar(64)  NULL

PRINT ''p4 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
UPDATE dbo.tblExportInterfaceResult SET TransDateTime=e.TransDateTime, 
										InterfaceName=e.InterfaceName, 
										ExportResultTypeIndex= e.[Type], 
										ArchiveFileName=e.ArchiveFileName,
										BatchID=e.BatchID ,
										SiteIndex=e.SiteIndex
										FROM dbo.tblExportResults e WHERE e.[index]=dbo.tblExportInterfaceResult.ExportResultIndex;

PRINT ''p5 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
IF (EXISTS(select TOP 1 1 from sys.indexes where object_name(object_id)=''tblExportInterfaceResult'' and name=''IX_tblExportResultDetails_RecordID_InterfaceData'')) 
	DROP INDEX [IX_tblExportResultDetails_RecordID_InterfaceData] ON [dbo].[tblExportInterfaceResult]

PRINT ''p6 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''

IF (EXISTS(select * from sys.all_objects where type = ''F'' and name=''FK_tblExportResultDetails_tblExportResults'')) 
	ALTER TABLE [dbo].[tblExportInterfaceResult] DROP CONSTRAINT [FK_tblExportResultDetails_tblExportResults]


PRINT ''p7 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
DROP STATISTICS dbo.tblExportInterfaceResult.ExportResultIndex


PRINT ''p8 Merging tblExportResults and tblExportResultDetails into tblExportInterfaceResult table''
ALTER TABLE dbo.tblExportInterfaceResult DROP COLUMN ExportResultIndex 
*/

PRINT ''Completed successfully''

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00020 Create New Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00020 Create New Tables', 
		@step_id=9, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	TAS Database Migration To v10.x
	SCRIPT TO Create Lookup tables
	Author: Aloisio(Al) dos Santos
*/

/*
		ADJUST TEMPLATE DATABASE WITH CRITICAL SCHEMA CHANGES INTRODUCED ATFER THIS SCRIPT GOT CRATES
*/

USE FuelsManagerDB_Template
GO

	ALTER TABLE tblArchivedUsers
	ALTER COLUMN UserGuid UNIQUEIDENTIFIER NULL

	ALTER TABLE dbo.tblEquipmentQualityTagLog
	ALTER COLUMN QualityTagGuid UNIQUEIDENTIFIER NULL

	ALTER TABLE dbo.tblTankQualityTagLog
	ALTER COLUMN QualityTagGuid UNIQUEIDENTIFIER NULL


USE ConsolidatedDB
GO

	/*
		END OF TEMPLATE DATABASE ADJUSTMENT
	*/
	SET NOCOUNT ON; 

	DECLARE @Schema NVARCHAR(300)
		,	@Table NVARCHAR(400)
		,	@Sql NVARCHAR(max)
		,	@LineFeed CHAR(2)
		,	@DataType NVARCHAR(500)
		,	@MaxLen INT 
		,	@Column NVARCHAR(500)

	SET @LineFeed = CHAR(13)+CHAR(10)
	
	DECLARE TableCur CURSOR FOR
		SELECT	s.name as SchemaName
			,	t.name as TableName
		FROM	[FuelsManagerDB_Template].sys.tables t
		INNER JOIN [FuelsManagerDB_Template].sys.schemas s on s.schema_id=t.schema_id
		WHERE NOT EXISTS(
			SELECT 1
			FROM [ConsolidatedDB].sys.tables t2
			INNER JOIN [ConsolidatedDB].sys.schemas s2 on s2.schema_id=t2.schema_id
			AND s2.name=s.name
			AND t2.name=t.name)
		ORDER BY s.name,t.name
	OPEN TableCur
	FETCH NEXT FROM TableCur INTO @Schema,@Table
	WHILE @@FETCH_STATUS=0
	BEGIN
		SET @Sql = ''IF NOT EXISTS (SELECT 1 FROM ConsolidatedDB.INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=''''''+@Schema+'''''' AND TABLE_NAME = ''''''+@Table+'''''')'' + @LineFeed
		SET @Sql+= ''	SELECT *  INTO [''+@Schema+''].[''+@Table+'']  FROM [FuelsManagerDB_Template].[''+@Schema+''].[''+@Table+'']'' + @LineFeed
		PRINT @Sql
		PRINT ''GO''
		EXEC sp_executesql @statement=@Sql
		FETCH NEXT FROM TableCur INTO @Schema,@Table
	END
	CLOSE TableCur
	DEALLOCATE TableCur


PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00030 Modify TAS Schema to Add New Cirrus Columns]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00030 Modify TAS Schema to Add New Cirrus Columns', 
		@step_id=10, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	TAS Database Migration To v10.x
	SCRIPT TO Add New columns
	Author: Aloisio(Al) dos Santos
*/

DECLARE @Schema NVARCHAR(300)
	,	@Table NVARCHAR(500)
	,	@Column NVARCHAR(500)
	,	@Type NVARCHAR(500)
	,	@Default NVARCHAR(2000)
	,	@Nullable VARCHAR(50)
	,	@MaxLength INT
	,	@Precision INT
	,	@PrecisionRadix INT
	,	@Sql NVARCHAR(max)

DECLARE ColCursor SCROLL CURSOR FOR
	SELECT	x.TABLE_SCHEMA
		,	x.TABLE_NAME
		,	x.COLUMN_NAME
		,	x.DATA_TYPE
		,	x.COLUMN_DEFAULT
		,	x.IS_NULLABLE
		,	x.CHARACTER_MAXIMUM_LENGTH
		,	x.NUMERIC_PRECISION
		,	x.NUMERIC_PRECISION_RADIX
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS x

	WHERE EXISTS
	(
		SELECT 1
		FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS s 
		WHERE	s.TABLE_SCHEMA=x.TABLE_SCHEMA
		AND	s.TABLE_NAME=x.TABLE_NAME
	)
	AND NOT EXISTS
	(	SELECT 1 
		FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS c
		WHERE	c.TABLE_SCHEMA=x.TABLE_SCHEMA
		AND		c.TABLE_NAME=x.TABLE_NAME
		AND		c.COLUMN_NAME=x.COLUMN_NAME
	)
	AND  NOT ( x.TABLE_SCHEMA = ''dbo'' and (x.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems'')))
	ORDER BY x.TABLE_SCHEMA,x.TABLE_NAME,x.ORDINAL_POSITION

OPEN ColCursor
FETCH NEXT FROM ColCursor
INTO @Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix

WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql = ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ''
	SET @Sql+= ''ADD [''+@Column+''] ''+@Type
	SET @Sql+= CASE	
					WHEN (@MaxLength IS NOT NULL AND @MaxLength > -1 AND @MaxLength < 2147483647) 
						THEN ''(''+CAST(@MaxLength AS NVARCHAR(100))+'') '' 
					WHEN @MaxLength = -1 AND @Type = ''xml''
						THEN '' ''
					WHEN @MaxLength = -1 AND @Type <> ''xml''
						THEN ''(MAX)''
					WHEN @MaxLength = 2147483647
						THEN '' ''
					ELSE '' '' 
				END
	
	PRINT @Sql
	EXEC sp_executesql @statement=@sql

	FETCH NEXT FROM ColCursor
	INTO @Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix
END
CLOSE ColCursor
DEALLOCATE ColCursor


-- ADJUST DATA TYPES WHCIH DID NOT GET CREATED BASED ON USER DEFINED DATA TYPE

DECLARE ObjCursor CURSOR FOR
	SELECT sc1.name as SchemaName,tb1.name as TableName,cl1.name as ColumnName,tp1.name as UserType
	FROM FuelsManagerDB_Template.sys.tables tb1
	INNER JOIN FuelsManagerDB_Template.sys.columns cl1 on cl1.object_id=tb1.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc1 on sc1.schema_id=tb1.schema_id
	INNER JOIN FuelsManagerDB_Template.sys.types tp1 on cl1.user_type_id=tp1.user_type_id
	WHERE cl1.system_type_id<> cl1.user_type_id
		AND  NOT( sc1.name = ''dbo'' and tb1.name in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY sc1.name,tb1.name,cl1.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@Type
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@schema+''].[''+@table+''] ALTER COLUMN [''+@Column+''] ''+@Type+'' ;''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@Type
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

/*
----------------------------------
ADJUST COLUMN''S PROPERTIES
----------------------------------
*/
-- Was tinyint in old schema
ALTER TABLE dbo.tblAlarmPriorities
ALTER COLUMN BackgroundSteady NVARCHAR(8)

-- Was tinyint in old schema
ALTER TABLE dbo.tblAlarmPriorities
ALTER COLUMN BackgroundAlternate NVARCHAR(8)

-- Was tinyint in old schema
ALTER TABLE dbo.tblAlarmPriorities
ALTER COLUMN TextSteady NVARCHAR(8)

-- Was tinyint in old schema
ALTER TABLE dbo.tblAlarmPriorities
ALTER COLUMN TextAlternate NVARCHAR(8)

ALTER TABLE dbo.tblAlarmAndEventLog
ALTER COLUMN AssociatedData NVARCHAR(MAX)

ALTER TABLE dbo.tblArchivedUsers
ALTER COLUMN UserID udtUserID

ALTER TABLE dbo.tblCompanies
ALTER COLUMN [ID] NVARCHAR(100)

ALTER TABLE dbo.tblIATA
ALTER COLUMN CountryID NVARCHAR(50)

ALTER TABLE tblQueryDefaultFields
ALTER COLUMN Topic NVARCHAR(100)

ALTER TABLE tblUsers
ALTER COLUMN UserID udtUserID
GO

IF EXISTS(SELECT TOP 1 1 FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS 
				WHERE TABLE_NAME=''tblUsers'' AND COLUMN_NAME=''AccountExpirationDate'' AND TABLE_SCHEMA=''dbo'' AND IS_NULLABLE=''no'')
	AND 
	EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS 
				WHERE TABLE_NAME=''tblUsers'' AND COLUMN_NAME=''AccountExpirationDate'' AND TABLE_SCHEMA=''dbo'')

	BEGIN
		UPDATE ConsolidatedDB.dbo.tblUsers SET AccountExpirationDate=(CONVERT([date],dateadd(year,(1),getdate()))) WHERE AccountExpirationDate IS NULL
	END

IF EXISTS(SELECT TOP 1 1 FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS 
				WHERE TABLE_NAME=''tblGroups'' AND COLUMN_NAME=''SessionTimeout'' AND TABLE_SCHEMA=''dbo'' AND IS_NULLABLE=''no'')
	AND 
	EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS 
				WHERE TABLE_NAME=''tblGroups'' AND COLUMN_NAME=''SessionTimeout'' AND TABLE_SCHEMA=''dbo'')

	BEGIN
		UPDATE ConsolidatedDB.dbo.tblGroups SET SessionTimeout=20 WHERE SessionTimeout IS NULL
	END

IF (SELECT COUNT(*)
	FROM sys.columns cl
	INNER JOIN sys.tables tb on tb.object_id=cl.object_id
	INNER JOIN sys.schemas sc on sc.schema_id=tb.schema_id
	WHERE cl.name=''QueryStorageGuid''
	AND tb.name=''tblQueryStorage''
	AND sc.name=''dbo''
	AND cl.is_rowguidcol=1)>0
BEGIN
	ALTER TABLE dbo.tblQueryStorage ALTER COLUMN QueryStorageGuid
	DROP ROWGUIDCOL
END

PRINT ''Completed successfully''
GO


', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00031 Implement Default Constraints - Copy]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00031 Implement Default Constraints - Copy', 
		@step_id=11, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
THIS FILE MUST BE RECREATED AFTER ANY DATABASE UPDATES OR PRIOR TO DEPLOYMENT

*/

DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(500)
	,	@DefaultName NVARCHAR(1000)
	,	@Definition NVARCHAR(max)
	,	@Column NVARCHAR(1000)
	,	@Sql NVARCHAR(max)

DECLARE ObjCursor CURSOR FOR
	SELECT	sch.name as SchemaName
		,	obj.name as TableName
		,	col.name as ColumnName
		,	tb1.name as DefaultName
		,	tb1.[Definition]
	FROM FuelsManagerDB_Template.sys.default_constraints tb1
	INNER JOIN FuelsManagerDB_Template.sys.objects obj ON obj.object_id=tb1.parent_object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sch ON obj.schema_id=sch.schema_id
	INNER JOIN FuelsManagerDB_Template.sys.columns col ON (col.object_id=tb1.parent_object_id AND col.column_id=tb1.parent_column_id)
	where not exists(
		SELECT 1 
		FROM sys.default_constraints tb2
		--INNER JOIN sys.objects obj2 ON obj2.object_id=tb2.object_id
		--INNER JOIN FuelsManagerDB_Template.sys.schemas sch2 ON obj2.schema_id=sch2.schema_id
		--INNER JOIN FuelsManagerDB_Template.sys.columns col2 ON (col2.object_id=tb2.parent_object_id AND col.column_id=tb2.parent_column_id)
		WHERE tb2.name=tb1.name)
		--WHERE obj2.name=obj.name
		--AND sch2.name=sch.name
		--AND col2.name=col.name
		--AND tb1.definition=tb2.definition)
--vivian added
	AND NOT  (tb1.name LIKE  ''DF__TT_PointT__Silen%'')
	AND NOT ( sch.name = ''dbo''  and obj.name in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY sch.name,obj.name,col.name

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@DefaultName,@Definition
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql= ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD CONSTRAINT [''+@DefaultName+''] DEFAULT ''+@Definition+'' FOR [''+@Column+''] ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@DefaultName,@Definition
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

PRINT ''Completed successfully''


', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00040 Update GUIDs PKs]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00040 Update GUIDs PKs', 
		@step_id=12, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	TAS Database Migration To v10.x
	SCRIPT TO Update New GUID PK with a generated values - NEWID()
	Author: Aloisio(Al) dos Santos
*/
	SET NOCOUNT ON;
	
-- COLLECT ALL PRIMARY KEY COLUMNS WITH WICH DATA TYPE IS GUID
-- TO ADD RANDON NEW ID
DECLARE @Schema NVARCHAR(200)
	,	@Table NVARCHAR(200)
	,	@Column NVARCHAR(300)
	,	@Sql NVARCHAR(MAX)

DECLARE GUID_CURSOR CURSOR FOR
	SELECT	DISTINCT
			tab.name as TableName
		,	sch.name as SchemaName
		,	col.name as ColumnName
	
	FROM	FuelsManagerDB_Template.sys.tables tab
	INNER JOIN FuelsManagerDB_Template.sys.schemas sch on sch.schema_id=tab.schema_id
	INNER JOIN FuelsManagerDB_Template.sys.columns col on col.object_id=tab.object_id
	INNER JOIN FuelsManagerDB_Template.sys.types typ on typ.user_type_id=col.user_type_id
	LEFT JOIN FuelsManagerDB_Template.sys.indexes idx on idx.object_id=tab.object_id
	LEFT JOIN FuelsManagerDB_Template.sys.index_columns icl on (icl.object_id=idx.object_id AND icl.index_id=idx.index_id and icl.column_id=col.column_id)
	WHERE icl.object_id IS NOT NULL
	AND typ.name = ''uniqueidentifier''
	AND idx.is_primary_key=1
	AND NOT (sch.name = ''dbo''and tab.name in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY sch.name,tab.name,col.name
OPEN GUID_CURSOR
FETCH NEXT FROM GUID_CURSOR INTO @Table,@Schema,@Column
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Sql = ''UPDATE [ConsolidatedDB].[''+@Schema+''].[''+@Table+''] SET [''+@Column+'']=NEWID() WHERE [''+@Column+''] IS NULL;''
	PRINT @Sql
	EXEC sys.sp_executesql @statement=@Sql
	FETCH NEXT FROM GUID_CURSOR INTO @Table,@Schema,@Column
END
CLOSE GUID_CURSOR
DEALLOCATE GUID_CURSOR


-- UPDATE WELL KNOW TABLES
UPDATE ConsolidatedDB.dbo.tblSites
SET SiteGUID =''00000000-0000-0000-0000-000000000001''
WHERE SiteIndex=-1


--Update administrator
UPDATE ConsolidatedDB.dbo.tblUsers
SET UserGuid = ''00000000-0000-0000-0000-000000000002''
WHERE UserID = ''Administrator''

-- To avoid the FMService from being locked out due to inactivity, set the LastLoginDate to now and clear the Inactivity Lockout attributes
UPDATE [dbo].[tblUsers] SET LastLoginDate = SYSDATETIMEOFFSET(), LastLogoffDate = SYSDATETIMEOFFSET(), InactivityLockout = 0, InactivityLockoutDate = NULL, UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = ''V9 Upgrade. AAC'' WHERE UserID = ''FMService''

UPDATE g
SET g.GroupGuid = gt.GroupGuid
FROM ConsolidatedDB.dbo.tblGroups g
INNER JOIN FuelsManagerDB_Template.dbo.tblGroups gt ON g.GroupID = gt.GroupID


;MERGE INTO ConsolidatedDB.dbo.tblApplicationString AS cas
USING FuelsManagerDB_Template.dbo.tblApplicationString AS fas
ON (cas.ID = fas.ID AND cas.LookupApplicationStringTypeIndex = fas.LookupApplicationStringTypeIndex)
WHEN NOT MATCHED
	THEN INSERT ([Type], ID, SiteIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, StartDate, EndDate, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
		VALUES (fas.LookupApplicationStringTypeIndex, fas.ID, -1, fas.CreatedDate, fas.CreatedBy, fas.UpdatedDate, fas.UpdatedBy, fas.StartDate, fas.EndDate, fas.ApplicationStringGuid, fas.SiteGuid, fas.LookupApplicationStringTypeIndex)
;

-- For some reason, "All Companies" was not part of the default 
IF NOT EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.dbo.tblApplicationString WHERE [ID] = ''All Companies'' AND [Type] = 8 )
BEGIN
	INSERT INTO ConsolidatedDB.[dbo].[tblApplicationString] ([siteindex],[Type],[ApplicationStringGuid], [ID], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SiteGuid], [LookupApplicationStringTypeIndex]) 
	VALUES (-1, 8,N''b2d8440b-0629-4de9-8cbb-f8dad5943116'', N''All Companies'', ''2015-05-27'', N''Administrator'', ''2015-05-29'', N''Administrator'', N''00000000-0000-0000-0000-000000000001'', 8)
END

IF NOT EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.[dbo].[tblAlarmPriorities] WHERE ID = N''Normal Unacknowledged'')
BEGIN
	INSERT INTO ConsolidatedDB.[dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[SiteIndex],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES (N''Normal Unacknowledged'',''00FF00'',''000000'',''000000'',''00FF00'',''Silence.mp3'', -1, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', N''5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f'', N''00000000-0000-0000-0000-000000000001'', null)
END
ELSE
BEGIN 
	UPDATE ConsolidatedDB.[dbo].[tblAlarmPriorities] 
	SET [BackgroundSteady] = ''00FF00'',
	[BackgroundAlternate] = ''000000'',
	[TextSteady] = ''000000'',
	[TextAlternate] =  ''00FF00'',
	[SoundFile] = ''Silence.mp3'',
	[SiteIndex] = -1,
	[AlarmPriorityGuid] = N''5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f'',
	[Priority] = null
	WHERE ID = N''Normal Unacknowledged''
END

IF NOT EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.[dbo].[tblAlarmPriorities] WHERE ID = ''HiHi/LoLo'')
BEGIN
	INSERT INTO ConsolidatedDB.[dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[SiteIndex],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES (''HiHi/LoLo'', ''FF0000'', ''000000'', ''000000'', ''FF0000'', ''fmsound01.mp3'', -1, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', N''aa9e557c-a652-4caf-9bca-2bcb9ab5b104'', N''00000000-0000-0000-0000-000000000001'', 1)
END
ELSE
BEGIN 
	UPDATE ConsolidatedDB.[dbo].[tblAlarmPriorities] 
	SET [BackgroundSteady] = ''FF0000'',
	[BackgroundAlternate] = ''000000'',
	[TextSteady] = ''000000'',
	[TextAlternate] =  ''FF0000'',
	[SoundFile] = ''fmsound01.mp3'',
	[SiteIndex] = -1,
	[AlarmPriorityGuid] = N''aa9e557c-a652-4caf-9bca-2bcb9ab5b104'',
	[Priority] = 1
	WHERE ID = ''HiHi/LoLo''
END

IF NOT EXISTS(SELECT TOP 1 1 FROM tblAlarmPriorities WHERE ID = ''High/Low'')
BEGIN
	INSERT INTO ConsolidatedDB.[dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[SiteIndex],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES (''High/Low'', ''FFFF00'', ''000000'', ''000000'', ''FFFF00'', ''fmsound02.mp3'', -1, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', N''BA35E686-5CCE-402D-982B-18D45958CCB6'', N''00000000-0000-0000-0000-000000000001'', 2)
END
ELSE
BEGIN 
	UPDATE ConsolidatedDB.[dbo].[tblAlarmPriorities] 
	SET [BackgroundSteady] = ''FFFF00'',
	[BackgroundAlternate] = ''000000'',
	[TextSteady] = ''000000'',
	[TextAlternate] =  ''FFFF00'',
	[SoundFile] = ''fmsound02.mp3'',
	[SiteIndex] = -1,
	[AlarmPriorityGuid] = N''BA35E686-5CCE-402D-982B-18D45958CCB6'',
	[Priority] = 2
	WHERE ID = ''High/Low''
END


IF NOT EXISTS(SELECT TOP 1 1 FROM ConsolidatedDB.[dbo].[tblAlarmPriorities] WHERE ID = ''Min/Max Operating'')
BEGIN
	INSERT INTO ConsolidatedDB.[dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[SiteIndex],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES (''Min/Max Operating'', ''FF00FF'', ''000000'', ''000000'', ''FF00FF'', ''fmsound00.mp3'', -1, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', N''402A7722-062B-42F6-B6A5-E6180E2BA2B8'', N''00000000-0000-0000-0000-000000000001'', 3)
END
ELSE
BEGIN 
	UPDATE ConsolidatedDB.[dbo].[tblAlarmPriorities] 
	SET [BackgroundSteady] = ''FF00FF'',
	[BackgroundAlternate] = ''000000'',
	[TextSteady] = ''000000'',
	[TextAlternate] =  ''FF00FF'',
	[SoundFile] = ''fmsound00.mp3'',
	[SiteIndex] = -1,
	[AlarmPriorityGuid] = N''402A7722-062B-42F6-B6A5-E6180E2BA2B8'',
	[Priority] = 3
	WHERE ID = ''Min/Max Operating''
END


PRINT ''Completed successfully''
GO
',
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00041 Apply Enterprise GUIDs]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00041 Apply Enterprise GUIDs', 
		@step_id=13, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'PRINT ''Replacing Generated GUIDs with Enterprise GUIDs''
/*
	TAS Database Migration To v10.x
	SCRIPT TO Update TAS database to replace GUID with the ones known by FM Enterprise based on the ID
	Author: George Peters
*/

--
-- Merge Enterprise tblSite Records and SiteGuids
--
        ;   WITH existingData AS (
                SELECT [dbo].[tblSites].[ID],[dbo].[tblSites].[Number],[dbo].[tblSites].[SPLCCode],[dbo].[tblSites].[Address1],[dbo].[tblSites].[Address2],[dbo].[tblSites].[City],[dbo].[tblSites].[State],[dbo].[tblSites].[Zip],[dbo].[tblSites].[Country],[dbo].[tblSites].[Phone],[dbo].[tblSites].[FAX],[dbo].[tblSites].[EmailAddress],[dbo].[tblSites].[EmergencyContact],[dbo].[tblSites].[EmergencyPhone],[dbo].[tblSites].[Enabled],[dbo].[tblSites].[SiteGroupFlag],[dbo].[tblSites].[TimeZone],[dbo].[tblSites].[LevelUnitIndex],[dbo].[tblSites].[TemperatureUnitIndex],[dbo].[tblSites].[DensityUnitIndex],[dbo].[tblSites].[PressureUnitIndex],[dbo].[tblSites].[FlowUnitIndex],[dbo].[tblSites].[VolumeUnitIndex],[dbo].[tblSites].[MassUnitIndex],[dbo].[tblSites].[AdditiveVolumeUnitIndex],[dbo].[tblSites].[AdditiveProfileCycleAmountUnitIndex],[dbo].[tblSites].[AdditiveProfileRateUnitIndex],[dbo].[tblSites].[LevelDecimalPlaces],[dbo].[tblSites].[TemperatureDecimalPlaces],[dbo].[tblSites].[DensityDecimalPlaces],[dbo].[tblSites].[PressureDecimalPlaces],[dbo].[tblSites].[FlowDecimalPlaces],[dbo].[tblSites].[VolumeDecimalPlaces],[dbo].[tblSites].[MassDecimalPlaces],[dbo].[tblSites].[AdditiveVolumeDecimalPlaces],[dbo].[tblSites].[AdditiveProfileCycleAmountDecimalPlaces],[dbo].[tblSites].[AdditiveProfileRateDecimalPlaces],[dbo].[tblSites].[InhibitAccessAfterHours],[dbo].[tblSites].[InhibitMultipleCardIns],[dbo].[tblSites].[AccessCardInRequired],[dbo].[tblSites].[CheckSiteNumber],[dbo].[tblSites].[PromptForCustomerCard],[dbo].[tblSites].[PromptForTractorOrTanker],[dbo].[tblSites].[PromptForFirstTrailer],[dbo].[tblSites].[PromptForSecondTrailer],[dbo].[tblSites].[PromptForCompartment],[dbo].[tblSites].[EnforceDriverEquipmentMatch],[dbo].[tblSites].[EnableAdditiveAccounting],[dbo].[tblSites].[UseCompanyEquipmentIdentifiers],[dbo].[tblSites].[UseLastKnownGoodTankData],[dbo].[tblSites].[MaximumLoadAmount],[dbo].[tblSites].[MaximumLoadTime],[dbo].[tblSites].[MaximumIdleTime],[dbo].[tblSites].[MaximumFlushAmount],[dbo].[tblSites].[MaximumMeterProvingAmount],[dbo].[tblSites].[MaximumReturnsAmount],[dbo].[tblSites].[MaximumNumberOfActiveArms],[dbo].[tblSites].[DriverTimeoutPeriod],[dbo].[tblSites].[DriverWarningPeriod],[dbo].[tblSites].[MaximumPrompts],[dbo].[tblSites].[MaximumVehicleWeight],[dbo].[tblSites].[LoadByNet],[dbo].[tblSites].[PromptForShipmentNumber],[dbo].[tblSites].[MaximumProductTemperature],[dbo].[tblSites].[ListEquipment],[dbo].[tblSites].[DeferStationChanges],[dbo].[tblSites].[InhibitBOLWithBrokenBlends],[dbo].[tblSites].[InhibitBOLWithImproperAdditization],[dbo].[tblSites].[InhibitOverweightBOL],[dbo].[tblSites].[ExceptionBOLPrinter],[dbo].[tblSites].[EnableAutomaticBOLPrinting],[dbo].[tblSites].[AutomaticBOLStartNumber],[dbo].[tblSites].[AutomaticBOLEndNumber],[dbo].[tblSites].[SeparateManualBOLNumbering],[dbo].[tblSites].[ManualBOLStartNumber],[dbo].[tblSites].[ManualBOLEndNumber],[dbo].[tblSites].[TransactionStartNumber],[dbo].[tblSites].[TransactionEndNumber],[dbo].[tblSites].[OrderStartNumber],[dbo].[tblSites].[OrderEndNumber],[dbo].[tblSites].[OpenTransactionWindow],[dbo].[tblSites].[AdministrativeLockDate],[dbo].[tblSites].[OperationalLockDate],[dbo].[tblSites].[MaximumDaysToRetainLogs],[dbo].[tblSites].[EnableDebugLogging],[dbo].[tblSites].[EnableAuditLogging],[dbo].[tblSites].[AutomaticallyPrintAlarmsAndEvents],[dbo].[tblSites].[AlarmAndEventPrinter],[dbo].[tblSites].[MailServer],[dbo].[tblSites].[MailFrom],[dbo].[tblSites].[MailUserName],[dbo].[tblSites].[MailPassword],[dbo].[tblSites].[DialupName],[dbo].[tblSites].[SCADASystem],[dbo].[tblSites].[InhibitTemplateGraphics],[dbo].[tblSites].[RefreshInterval],[dbo].[tblSites].[InhibitEndOfDayOperations],[dbo].[tblSites].[InhibitEndOfMonthOperations],[dbo].[tblSites].[EndOfDayWarningPeriod],[dbo].[tblSites].[InhibitAutomaticPhysicalInventory],[dbo].[tblSites].[InhibitAutomaticMeterCloseout],[dbo].[tblSites].[InhibitAutomaticReportGeneration],[dbo].[tblSites].[InhibitAutomaticAdjustmentDistribution],[dbo].[tblSites].[InhibitAutomaticCloseout],[dbo].[tblSites].[InhibitTankScan],[dbo].[tblSites].[ReportDirectory],[dbo].[tblSites].[ManageReports],[dbo].[tblSites].[ManagedReportDirectory],[dbo].[tblSites].[VRURateLimit],[dbo].[tblSites].[VRUHourlyLimit],[dbo].[tblSites].[VRUDailyLimit],[dbo].[tblSites].[VRUYearlyLimit],[dbo].[tblSites].[VRUCurrentYearLimit],[dbo].[tblSites].[VRURateActual],[dbo].[tblSites].[VRUHourlyActual],[dbo].[tblSites].[VRUDailyActual],[dbo].[tblSites].[VRUYearlyActual],[dbo].[tblSites].[VRUCurrentYearActual],[dbo].[tblSites].[VRURateLimitEnabled],[dbo].[tblSites].[VRUHourlyLimitEnabled],[dbo].[tblSites].[VRUDailyLimitEnabled],[dbo].[tblSites].[VRUYearlyLimitEnabled],[dbo].[tblSites].[VRUCurrentYearLimitEnabled],[dbo].[tblSites].[WatchdogPeriod],[dbo].[tblSites].[WatchdogCounterStart],[dbo].[tblSites].[WatchdogCounterEnd],[dbo].[tblSites].[NumberDecimalSeparator],[dbo].[tblSites].[NumberGroupSeparator],[dbo].[tblSites].[ListSeparator],[dbo].[tblSites].[TimePattern],[dbo].[tblSites].[TimeSeparator],[dbo].[tblSites].[AMSymbol],[dbo].[tblSites].[PMSymbol],[dbo].[tblSites].[ShortDatePattern],[dbo].[tblSites].[DateSeparator],[dbo].[tblSites].[LongDatePattern],[dbo].[tblSites].[TwoDigitCalendarEndYear],[dbo].[tblSites].[UserData1],[dbo].[tblSites].[UserData2],[dbo].[tblSites].[UserData3],[dbo].[tblSites].[UserData4],[dbo].[tblSites].[UserData5],[dbo].[tblSites].[UserData6],[dbo].[tblSites].[UserData7],[dbo].[tblSites].[UserData8],[dbo].[tblSites].[CreatedDate],[dbo].[tblSites].[CreatedBy],[dbo].[tblSites].[UpdatedDate],[dbo].[tblSites].[UpdatedBy],[dbo].[tblSites].[MinTimeAllowedToChangePwd],[dbo].[tblSites].[MinPwdCharacterLength],[dbo].[tblSites].[PwdExpirationInDays],[dbo].[tblSites].[PwdLockoutThreshold],[dbo].[tblSites].[CheckForPreviousPwd],[dbo].[tblSites].[StrongPwdUse],[dbo].[tblSites].[PwdHistoryCount],[dbo].[tblSites].[ApplyToAllSiteMembers],[dbo].[tblSites].[InactivityDisablePeriod],[dbo].[tblSites].[EnforceSingleOwner],[dbo].[tblSites].[InhibitBOLSummaryAutoPopulate],[dbo].[tblSites].[InhibitOrderSummaryAutoPopulate],[dbo].[tblSites].[InhibitSupplyOrderSummaryAutoPopulate],[dbo].[tblSites].[InvoiceStartNumber],[dbo].[tblSites].[InvoiceEndNumber],[dbo].[tblSites].[PromptForReturns],[dbo].[tblSites].[PromptForTruckCard],[dbo].[tblSites].[StartingShortCardNumber],[dbo].[tblSites].[UseShortCardNumber],[dbo].[tblSites].[ExcessVarianceCount],[dbo].[tblSites].[ExcessVarianceTolerance],[dbo].[tblSites].[DisableArchivePeriod],[dbo].[tblSites].[ExportArchiveDir],[dbo].[tblSites].[ImportArchiveDir],[dbo].[tblSites].[GroupLedgerByID],[dbo].[tblSites].[InhibitSiteLedgerRollup],[dbo].[tblSites].[UseTankReconciliation],[dbo].[tblSites].[SiteGuid],[dbo].[tblSites].[LookupNumberGroupSizesTypeIndex],[dbo].[tblSites].[LookupQuantityDisplayDefaultIndex],[dbo].[tblSites].[LookupSecondaryStorageFillMethodIndex],[dbo].[tblSites].[LookupMailConnectModeIndex],[dbo].[tblSites].[LookupWatchdogModeIndex],[dbo].[tblSites].[Contact1Name],[dbo].[tblSites].[Contact1Address1],[dbo].[tblSites].[Contact1Address2],[dbo].[tblSites].[Contact1City],[dbo].[tblSites].[Contact1State],[dbo].[tblSites].[Contact1Zip],[dbo].[tblSites].[Contact1Country],[dbo].[tblSites].[Contact1PhoneOffice],[dbo].[tblSites].[Contact1Fax],[dbo].[tblSites].[Contact1EmailAddress],[dbo].[tblSites].[Contact2Name],[dbo].[tblSites].[Contact2Address1],[dbo].[tblSites].[Contact2Address2],[dbo].[tblSites].[Contact2City],[dbo].[tblSites].[Contact2State],[dbo].[tblSites].[Contact2Zip],[dbo].[tblSites].[Contact2Country],[dbo].[tblSites].[Contact2PhoneOffice],[dbo].[tblSites].[Contact2Fax],[dbo].[tblSites].[Contact2EmailAddress],[dbo].[tblSites].[Contact1PhoneMobile],[dbo].[tblSites].[Contact2PhoneMobile],[dbo].[tblSites].[EnablePasswordHint],[dbo].[tblSites].[EnablePasswordReset],[dbo].[tblSites].[MeterReconciliationToleranceIsPercent],[dbo].[tblSites].[MeterReconciliationReportName],[dbo].[tblSites].[TranslatedHelpURL],[dbo].[tblSites].[AllowUseOfSpecialChars],[dbo].[tblSites].[EnablePeriodicSyncFlag],[dbo].[tblSites].[PeriodicSyncIntervalMinutes],[dbo].[tblSites].[CardInTimeout],[dbo].[tblSites].[TerminalControlNumber],[dbo].[tblSites].[BlockCloseOnUnpostedBOL],[dbo].[tblSites].[InhibitLoadRackCardIns],[dbo].[tblSites].[PromptForThirdTrailer],[dbo].[tblSites].[PromptForTransactionCompletion],[dbo].[tblSites].[InhibitCustomerConfirmationPrompt],[dbo].[tblSites].[EnableBOLPDFArchiving],[dbo].[tblSites].[BOLPDFArchivingPath],[dbo].[tblSites].[RequireTrailerScully],[dbo].[tblSites].[Latitude],[dbo].[tblSites].[Longitude],[dbo].[tblSites].[Zoom],[dbo].[tblSites].[GlobalAccessToPersonnel],[dbo].[tblSites].[GlobalAccessToEquipment],[dbo].[tblSites].[Enterprise],[dbo].[tblSites].[OperateTabGroups],[dbo].[tblSites].[EnterpriseUserId],[dbo].[tblSites].[EnterprisePassword],[dbo].[tblSites].[EnterpriseSite]
                    FROM [dbo].[tblSites]
            ) MERGE existingData
            USING (SELECT [FuelsManagerDB_Template].[dbo].[tblSites].[ID],[FuelsManagerDB_Template].[dbo].[tblSites].[Number],[FuelsManagerDB_Template].[dbo].[tblSites].[SPLCCode],[FuelsManagerDB_Template].[dbo].[tblSites].[Address1],[FuelsManagerDB_Template].[dbo].[tblSites].[Address2],[FuelsManagerDB_Template].[dbo].[tblSites].[City],[FuelsManagerDB_Template].[dbo].[tblSites].[State],[FuelsManagerDB_Template].[dbo].[tblSites].[Zip],[FuelsManagerDB_Template].[dbo].[tblSites].[Country],[FuelsManagerDB_Template].[dbo].[tblSites].[Phone],[FuelsManagerDB_Template].[dbo].[tblSites].[FAX],[FuelsManagerDB_Template].[dbo].[tblSites].[EmailAddress],[FuelsManagerDB_Template].[dbo].[tblSites].[EmergencyContact],[FuelsManagerDB_Template].[dbo].[tblSites].[EmergencyPhone],[FuelsManagerDB_Template].[dbo].[tblSites].[Enabled],[FuelsManagerDB_Template].[dbo].[tblSites].[SiteGroupFlag],[FuelsManagerDB_Template].[dbo].[tblSites].[TimeZone],[FuelsManagerDB_Template].[dbo].[tblSites].[LevelUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[TemperatureUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[DensityUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[PressureUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[FlowUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[VolumeUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[MassUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveVolumeUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveProfileCycleAmountUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveProfileRateUnitIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[LevelDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[TemperatureDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[DensityDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[PressureDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[FlowDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[VolumeDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[MassDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveVolumeDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveProfileCycleAmountDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[AdditiveProfileRateDecimalPlaces],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAccessAfterHours],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitMultipleCardIns],[FuelsManagerDB_Template].[dbo].[tblSites].[AccessCardInRequired],[FuelsManagerDB_Template].[dbo].[tblSites].[CheckSiteNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForCustomerCard],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForTractorOrTanker],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForFirstTrailer],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForSecondTrailer],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForCompartment],[FuelsManagerDB_Template].[dbo].[tblSites].[EnforceDriverEquipmentMatch],[FuelsManagerDB_Template].[dbo].[tblSites].[EnableAdditiveAccounting],[FuelsManagerDB_Template].[dbo].[tblSites].[UseCompanyEquipmentIdentifiers],[FuelsManagerDB_Template].[dbo].[tblSites].[UseLastKnownGoodTankData],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumLoadAmount],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumLoadTime],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumIdleTime],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumFlushAmount],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumMeterProvingAmount],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumReturnsAmount],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumNumberOfActiveArms],[FuelsManagerDB_Template].[dbo].[tblSites].[DriverTimeoutPeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[DriverWarningPeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumPrompts],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumVehicleWeight],[FuelsManagerDB_Template].[dbo].[tblSites].[LoadByNet],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForShipmentNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumProductTemperature],[FuelsManagerDB_Template].[dbo].[tblSites].[ListEquipment],[FuelsManagerDB_Template].[dbo].[tblSites].[DeferStationChanges],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitBOLWithBrokenBlends],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitBOLWithImproperAdditization],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitOverweightBOL],[FuelsManagerDB_Template].[dbo].[tblSites].[ExceptionBOLPrinter],[FuelsManagerDB_Template].[dbo].[tblSites].[EnableAutomaticBOLPrinting],[FuelsManagerDB_Template].[dbo].[tblSites].[AutomaticBOLStartNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[AutomaticBOLEndNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[SeparateManualBOLNumbering],[FuelsManagerDB_Template].[dbo].[tblSites].[ManualBOLStartNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[ManualBOLEndNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[TransactionStartNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[TransactionEndNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[OrderStartNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[OrderEndNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[OpenTransactionWindow],[FuelsManagerDB_Template].[dbo].[tblSites].[AdministrativeLockDate],[FuelsManagerDB_Template].[dbo].[tblSites].[OperationalLockDate],[FuelsManagerDB_Template].[dbo].[tblSites].[MaximumDaysToRetainLogs],[FuelsManagerDB_Template].[dbo].[tblSites].[EnableDebugLogging],[FuelsManagerDB_Template].[dbo].[tblSites].[EnableAuditLogging],[FuelsManagerDB_Template].[dbo].[tblSites].[AutomaticallyPrintAlarmsAndEvents],[FuelsManagerDB_Template].[dbo].[tblSites].[AlarmAndEventPrinter],[FuelsManagerDB_Template].[dbo].[tblSites].[MailServer],[FuelsManagerDB_Template].[dbo].[tblSites].[MailFrom],[FuelsManagerDB_Template].[dbo].[tblSites].[MailUserName],[FuelsManagerDB_Template].[dbo].[tblSites].[MailPassword],[FuelsManagerDB_Template].[dbo].[tblSites].[DialupName],[FuelsManagerDB_Template].[dbo].[tblSites].[SCADASystem],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitTemplateGraphics],[FuelsManagerDB_Template].[dbo].[tblSites].[RefreshInterval],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitEndOfDayOperations],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitEndOfMonthOperations],[FuelsManagerDB_Template].[dbo].[tblSites].[EndOfDayWarningPeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAutomaticPhysicalInventory],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAutomaticMeterCloseout],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAutomaticReportGeneration],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAutomaticAdjustmentDistribution],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitAutomaticCloseout],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitTankScan],[FuelsManagerDB_Template].[dbo].[tblSites].[ReportDirectory],[FuelsManagerDB_Template].[dbo].[tblSites].[ManageReports],[FuelsManagerDB_Template].[dbo].[tblSites].[ManagedReportDirectory],[FuelsManagerDB_Template].[dbo].[tblSites].[VRURateLimit],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUHourlyLimit],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUDailyLimit],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUYearlyLimit],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUCurrentYearLimit],[FuelsManagerDB_Template].[dbo].[tblSites].[VRURateActual],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUHourlyActual],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUDailyActual],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUYearlyActual],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUCurrentYearActual],[FuelsManagerDB_Template].[dbo].[tblSites].[VRURateLimitEnabled],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUHourlyLimitEnabled],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUDailyLimitEnabled],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUYearlyLimitEnabled],[FuelsManagerDB_Template].[dbo].[tblSites].[VRUCurrentYearLimitEnabled],[FuelsManagerDB_Template].[dbo].[tblSites].[WatchdogPeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[WatchdogCounterStart],[FuelsManagerDB_Template].[dbo].[tblSites].[WatchdogCounterEnd],[FuelsManagerDB_Template].[dbo].[tblSites].[NumberDecimalSeparator],[FuelsManagerDB_Template].[dbo].[tblSites].[NumberGroupSeparator],[FuelsManagerDB_Template].[dbo].[tblSites].[ListSeparator],[FuelsManagerDB_Template].[dbo].[tblSites].[TimePattern],[FuelsManagerDB_Template].[dbo].[tblSites].[TimeSeparator],[FuelsManagerDB_Template].[dbo].[tblSites].[AMSymbol],[FuelsManagerDB_Template].[dbo].[tblSites].[PMSymbol],[FuelsManagerDB_Template].[dbo].[tblSites].[ShortDatePattern],[FuelsManagerDB_Template].[dbo].[tblSites].[DateSeparator],[FuelsManagerDB_Template].[dbo].[tblSites].[LongDatePattern],[FuelsManagerDB_Template].[dbo].[tblSites].[TwoDigitCalendarEndYear],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData1],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData2],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData3],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData4],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData5],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData6],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData7],[FuelsManagerDB_Template].[dbo].[tblSites].[UserData8],[FuelsManagerDB_Template].[dbo].[tblSites].[CreatedDate],[FuelsManagerDB_Template].[dbo].[tblSites].[CreatedBy],[FuelsManagerDB_Template].[dbo].[tblSites].[UpdatedDate],[FuelsManagerDB_Template].[dbo].[tblSites].[UpdatedBy],[FuelsManagerDB_Template].[dbo].[tblSites].[MinTimeAllowedToChangePwd],[FuelsManagerDB_Template].[dbo].[tblSites].[MinPwdCharacterLength],[FuelsManagerDB_Template].[dbo].[tblSites].[PwdExpirationInDays],[FuelsManagerDB_Template].[dbo].[tblSites].[PwdLockoutThreshold],[FuelsManagerDB_Template].[dbo].[tblSites].[CheckForPreviousPwd],[FuelsManagerDB_Template].[dbo].[tblSites].[StrongPwdUse],[FuelsManagerDB_Template].[dbo].[tblSites].[PwdHistoryCount],[FuelsManagerDB_Template].[dbo].[tblSites].[ApplyToAllSiteMembers],[FuelsManagerDB_Template].[dbo].[tblSites].[InactivityDisablePeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[EnforceSingleOwner],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitBOLSummaryAutoPopulate],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitOrderSummaryAutoPopulate],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitSupplyOrderSummaryAutoPopulate],[FuelsManagerDB_Template].[dbo].[tblSites].[InvoiceStartNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[InvoiceEndNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForReturns],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForTruckCard],[FuelsManagerDB_Template].[dbo].[tblSites].[StartingShortCardNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[UseShortCardNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[ExcessVarianceCount],[FuelsManagerDB_Template].[dbo].[tblSites].[ExcessVarianceTolerance],[FuelsManagerDB_Template].[dbo].[tblSites].[DisableArchivePeriod],[FuelsManagerDB_Template].[dbo].[tblSites].[ExportArchiveDir],[FuelsManagerDB_Template].[dbo].[tblSites].[ImportArchiveDir],[FuelsManagerDB_Template].[dbo].[tblSites].[GroupLedgerByID],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitSiteLedgerRollup],[FuelsManagerDB_Template].[dbo].[tblSites].[UseTankReconciliation],[FuelsManagerDB_Template].[dbo].[tblSites].[SiteGuid],[FuelsManagerDB_Template].[dbo].[tblSites].[LookupNumberGroupSizesTypeIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[LookupQuantityDisplayDefaultIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[LookupSecondaryStorageFillMethodIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[LookupMailConnectModeIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[LookupWatchdogModeIndex],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Name],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Address1],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Address2],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1City],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1State],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Zip],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Country],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1PhoneOffice],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1Fax],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1EmailAddress],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Name],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Address1],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Address2],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2City],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2State],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Zip],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Country],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2PhoneOffice],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2Fax],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2EmailAddress],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact1PhoneMobile],[FuelsManagerDB_Template].[dbo].[tblSites].[Contact2PhoneMobile],[FuelsManagerDB_Template].[dbo].[tblSites].[EnablePasswordHint],[FuelsManagerDB_Template].[dbo].[tblSites].[EnablePasswordReset],[FuelsManagerDB_Template].[dbo].[tblSites].[MeterReconciliationToleranceIsPercent],[FuelsManagerDB_Template].[dbo].[tblSites].[MeterReconciliationReportName],[FuelsManagerDB_Template].[dbo].[tblSites].[TranslatedHelpURL],[FuelsManagerDB_Template].[dbo].[tblSites].[AllowUseOfSpecialChars],[FuelsManagerDB_Template].[dbo].[tblSites].[EnablePeriodicSyncFlag],[FuelsManagerDB_Template].[dbo].[tblSites].[PeriodicSyncIntervalMinutes],[FuelsManagerDB_Template].[dbo].[tblSites].[CardInTimeout],[FuelsManagerDB_Template].[dbo].[tblSites].[TerminalControlNumber],[FuelsManagerDB_Template].[dbo].[tblSites].[BlockCloseOnUnpostedBOL],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitLoadRackCardIns],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForThirdTrailer],[FuelsManagerDB_Template].[dbo].[tblSites].[PromptForTransactionCompletion],[FuelsManagerDB_Template].[dbo].[tblSites].[InhibitCustomerConfirmationPrompt],[FuelsManagerDB_Template].[dbo].[tblSites].[EnableBOLPDFArchiving],[FuelsManagerDB_Template].[dbo].[tblSites].[BOLPDFArchivingPath],[FuelsManagerDB_Template].[dbo].[tblSites].[RequireTrailerScully],[FuelsManagerDB_Template].[dbo].[tblSites].[Latitude],[FuelsManagerDB_Template].[dbo].[tblSites].[Longitude],[FuelsManagerDB_Template].[dbo].[tblSites].[Zoom],[FuelsManagerDB_Template].[dbo].[tblSites].[GlobalAccessToPersonnel],[FuelsManagerDB_Template].[dbo].[tblSites].[GlobalAccessToEquipment],[FuelsManagerDB_Template].[dbo].[tblSites].[Enterprise],[FuelsManagerDB_Template].[dbo].[tblSites].[OperateTabGroups],[FuelsManagerDB_Template].[dbo].[tblSites].[EnterpriseUserId],[FuelsManagerDB_Template].[dbo].[tblSites].[EnterprisePassword],[FuelsManagerDB_Template].[dbo].[tblSites].[EnterpriseSite] FROM [FuelsManagerDB_Template].[dbo].[tblSites]
                    ) AS remoteChanges ([ID],[Number],[SPLCCode],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmailAddress],[EmergencyContact],[EmergencyPhone],[Enabled],[SiteGroupFlag],[TimeZone],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[AdditiveProfileCycleAmountDecimalPlaces],[AdditiveProfileRateDecimalPlaces],[InhibitAccessAfterHours],[InhibitMultipleCardIns],[AccessCardInRequired],[CheckSiteNumber],[PromptForCustomerCard],[PromptForTractorOrTanker],[PromptForFirstTrailer],[PromptForSecondTrailer],[PromptForCompartment],[EnforceDriverEquipmentMatch],[EnableAdditiveAccounting],[UseCompanyEquipmentIdentifiers],[UseLastKnownGoodTankData],[MaximumLoadAmount],[MaximumLoadTime],[MaximumIdleTime],[MaximumFlushAmount],[MaximumMeterProvingAmount],[MaximumReturnsAmount],[MaximumNumberOfActiveArms],[DriverTimeoutPeriod],[DriverWarningPeriod],[MaximumPrompts],[MaximumVehicleWeight],[LoadByNet],[PromptForShipmentNumber],[MaximumProductTemperature],[ListEquipment],[DeferStationChanges],[InhibitBOLWithBrokenBlends],[InhibitBOLWithImproperAdditization],[InhibitOverweightBOL],[ExceptionBOLPrinter],[EnableAutomaticBOLPrinting],[AutomaticBOLStartNumber],[AutomaticBOLEndNumber],[SeparateManualBOLNumbering],[ManualBOLStartNumber],[ManualBOLEndNumber],[TransactionStartNumber],[TransactionEndNumber],[OrderStartNumber],[OrderEndNumber],[OpenTransactionWindow],[AdministrativeLockDate],[OperationalLockDate],[MaximumDaysToRetainLogs],[EnableDebugLogging],[EnableAuditLogging],[AutomaticallyPrintAlarmsAndEvents],[AlarmAndEventPrinter],[MailServer],[MailFrom],[MailUserName],[MailPassword],[DialupName],[SCADASystem],[InhibitTemplateGraphics],[RefreshInterval],[InhibitEndOfDayOperations],[InhibitEndOfMonthOperations],[EndOfDayWarningPeriod],[InhibitAutomaticPhysicalInventory],[InhibitAutomaticMeterCloseout],[InhibitAutomaticReportGeneration],[InhibitAutomaticAdjustmentDistribution],[InhibitAutomaticCloseout],[InhibitTankScan],[ReportDirectory],[ManageReports],[ManagedReportDirectory],[VRURateLimit],[VRUHourlyLimit],[VRUDailyLimit],[VRUYearlyLimit],[VRUCurrentYearLimit],[VRURateActual],[VRUHourlyActual],[VRUDailyActual],[VRUYearlyActual],[VRUCurrentYearActual],[VRURateLimitEnabled],[VRUHourlyLimitEnabled],[VRUDailyLimitEnabled],[VRUYearlyLimitEnabled],[VRUCurrentYearLimitEnabled],[WatchdogPeriod],[WatchdogCounterStart],[WatchdogCounterEnd],[NumberDecimalSeparator],[NumberGroupSeparator],[ListSeparator],[TimePattern],[TimeSeparator],[AMSymbol],[PMSymbol],[ShortDatePattern],[DateSeparator],[LongDatePattern],[TwoDigitCalendarEndYear],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MinTimeAllowedToChangePwd],[MinPwdCharacterLength],[PwdExpirationInDays],[PwdLockoutThreshold],[CheckForPreviousPwd],[StrongPwdUse],[PwdHistoryCount],[ApplyToAllSiteMembers],[InactivityDisablePeriod],[EnforceSingleOwner],[InhibitBOLSummaryAutoPopulate],[InhibitOrderSummaryAutoPopulate],[InhibitSupplyOrderSummaryAutoPopulate],[InvoiceStartNumber],[InvoiceEndNumber],[PromptForReturns],[PromptForTruckCard],[StartingShortCardNumber],[UseShortCardNumber],[ExcessVarianceCount],[ExcessVarianceTolerance],[DisableArchivePeriod],[ExportArchiveDir],[ImportArchiveDir],[GroupLedgerByID],[InhibitSiteLedgerRollup],[UseTankReconciliation],[SiteGuid],[LookupNumberGroupSizesTypeIndex],[LookupQuantityDisplayDefaultIndex],[LookupSecondaryStorageFillMethodIndex],[LookupMailConnectModeIndex],[LookupWatchdogModeIndex],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[EnablePasswordHint],[EnablePasswordReset],[MeterReconciliationToleranceIsPercent],[MeterReconciliationReportName],[TranslatedHelpURL],[AllowUseOfSpecialChars],[EnablePeriodicSyncFlag],[PeriodicSyncIntervalMinutes],[CardInTimeout],[TerminalControlNumber],[BlockCloseOnUnpostedBOL],[InhibitLoadRackCardIns],[PromptForThirdTrailer],[PromptForTransactionCompletion],[InhibitCustomerConfirmationPrompt],[EnableBOLPDFArchiving],[BOLPDFArchivingPath],[RequireTrailerScully],[Latitude],[Longitude],[Zoom],[GlobalAccessToPersonnel],[GlobalAccessToEquipment],[Enterprise],[OperateTabGroups],[EnterpriseUserId],[EnterprisePassword],[EnterpriseSite])
            ON (existingData.[ID] = remoteChanges.[ID])
            WHEN MATCHED AND (remoteChanges.UpdatedDate >= existingData.UpdatedDate)
                THEN
                UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[Number] = existingData.[Number]
                       ,[SPLCCode] = existingData.[SPLCCode]
                       ,[Address1] = existingData.[Address1]
                       ,[Address2] = existingData.[Address2]
                       ,[City] = existingData.[City]
                       ,[State] = existingData.[State]
                       ,[Zip] = existingData.[Zip]
                       ,[Country] = existingData.[Country]
                       ,[Phone] = existingData.[Phone]
                       ,[FAX] = existingData.[FAX]
                       ,[EmailAddress] = existingData.[EmailAddress]
                       ,[EmergencyContact] = existingData.[EmergencyContact]
                       ,[EmergencyPhone] = existingData.[EmergencyPhone]
                       ,[Enabled] = existingData.[Enabled]
                       ,[SiteGroupFlag] = existingData.[SiteGroupFlag]
                       ,[TimeZone] = existingData.[TimeZone]
                       ,[LevelUnitIndex] = existingData.[LevelUnitIndex]
                       ,[TemperatureUnitIndex] = existingData.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = existingData.[DensityUnitIndex]
                       ,[PressureUnitIndex] = existingData.[PressureUnitIndex]
                       ,[FlowUnitIndex] = existingData.[FlowUnitIndex]
                       ,[VolumeUnitIndex] = existingData.[VolumeUnitIndex]
                       ,[MassUnitIndex] = existingData.[MassUnitIndex]
                       ,[AdditiveVolumeUnitIndex] = existingData.[AdditiveVolumeUnitIndex]
                       ,[AdditiveProfileCycleAmountUnitIndex] = existingData.[AdditiveProfileCycleAmountUnitIndex]
                       ,[AdditiveProfileRateUnitIndex] = existingData.[AdditiveProfileRateUnitIndex]
                       ,[LevelDecimalPlaces] = existingData.[LevelDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = existingData.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = existingData.[DensityDecimalPlaces]
                       ,[PressureDecimalPlaces] = existingData.[PressureDecimalPlaces]
                       ,[FlowDecimalPlaces] = existingData.[FlowDecimalPlaces]
                       ,[VolumeDecimalPlaces] = existingData.[VolumeDecimalPlaces]
                       ,[MassDecimalPlaces] = existingData.[MassDecimalPlaces]
                       ,[AdditiveVolumeDecimalPlaces] = existingData.[AdditiveVolumeDecimalPlaces]
                       ,[AdditiveProfileCycleAmountDecimalPlaces] = existingData.[AdditiveProfileCycleAmountDecimalPlaces]
                       ,[AdditiveProfileRateDecimalPlaces] = existingData.[AdditiveProfileRateDecimalPlaces]
                       ,[InhibitAccessAfterHours] = existingData.[InhibitAccessAfterHours]
                       ,[InhibitMultipleCardIns] = existingData.[InhibitMultipleCardIns]
                       ,[AccessCardInRequired] = existingData.[AccessCardInRequired]
                       ,[CheckSiteNumber] = existingData.[CheckSiteNumber]
                       ,[PromptForCustomerCard] = existingData.[PromptForCustomerCard]
                       ,[PromptForTractorOrTanker] = existingData.[PromptForTractorOrTanker]
                       ,[PromptForFirstTrailer] = existingData.[PromptForFirstTrailer]
                       ,[PromptForSecondTrailer] = existingData.[PromptForSecondTrailer]
                       ,[PromptForCompartment] = existingData.[PromptForCompartment]
                       ,[EnforceDriverEquipmentMatch] = existingData.[EnforceDriverEquipmentMatch]
                       ,[EnableAdditiveAccounting] = existingData.[EnableAdditiveAccounting]
                       ,[UseCompanyEquipmentIdentifiers] = existingData.[UseCompanyEquipmentIdentifiers]
                       ,[UseLastKnownGoodTankData] = existingData.[UseLastKnownGoodTankData]
                       ,[MaximumLoadAmount] = existingData.[MaximumLoadAmount]
                       ,[MaximumLoadTime] = existingData.[MaximumLoadTime]
                       ,[MaximumIdleTime] = existingData.[MaximumIdleTime]
                       ,[MaximumFlushAmount] = existingData.[MaximumFlushAmount]
                       ,[MaximumMeterProvingAmount] = existingData.[MaximumMeterProvingAmount]
                       ,[MaximumReturnsAmount] = existingData.[MaximumReturnsAmount]
                       ,[MaximumNumberOfActiveArms] = existingData.[MaximumNumberOfActiveArms]
                       ,[DriverTimeoutPeriod] = existingData.[DriverTimeoutPeriod]
                       ,[DriverWarningPeriod] = existingData.[DriverWarningPeriod]
                       ,[MaximumPrompts] = existingData.[MaximumPrompts]
                       ,[MaximumVehicleWeight] = existingData.[MaximumVehicleWeight]
                       ,[LoadByNet] = existingData.[LoadByNet]
                       ,[PromptForShipmentNumber] = existingData.[PromptForShipmentNumber]
                       ,[MaximumProductTemperature] = existingData.[MaximumProductTemperature]
                       ,[ListEquipment] = existingData.[ListEquipment]
                       ,[DeferStationChanges] = existingData.[DeferStationChanges]
                       ,[InhibitBOLWithBrokenBlends] = existingData.[InhibitBOLWithBrokenBlends]
                       ,[InhibitBOLWithImproperAdditization] = existingData.[InhibitBOLWithImproperAdditization]
                       ,[InhibitOverweightBOL] = existingData.[InhibitOverweightBOL]
                       ,[ExceptionBOLPrinter] = existingData.[ExceptionBOLPrinter]
                       ,[EnableAutomaticBOLPrinting] = existingData.[EnableAutomaticBOLPrinting]
                       ,[AutomaticBOLStartNumber] = existingData.[AutomaticBOLStartNumber]
                       ,[AutomaticBOLEndNumber] = existingData.[AutomaticBOLEndNumber]
                       ,[SeparateManualBOLNumbering] = existingData.[SeparateManualBOLNumbering]
                       ,[ManualBOLStartNumber] = existingData.[ManualBOLStartNumber]
                       ,[ManualBOLEndNumber] = existingData.[ManualBOLEndNumber]
                       ,[TransactionStartNumber] = existingData.[TransactionStartNumber]
                       ,[TransactionEndNumber] = existingData.[TransactionEndNumber]
                       ,[OrderStartNumber] = existingData.[OrderStartNumber]
                       ,[OrderEndNumber] = existingData.[OrderEndNumber]
                       ,[OpenTransactionWindow] = existingData.[OpenTransactionWindow]
                       ,[AdministrativeLockDate] = existingData.[AdministrativeLockDate]
                       ,[OperationalLockDate] = existingData.[OperationalLockDate]
                       ,[MaximumDaysToRetainLogs] = existingData.[MaximumDaysToRetainLogs]
                       ,[EnableDebugLogging] = existingData.[EnableDebugLogging]
                       ,[EnableAuditLogging] = existingData.[EnableAuditLogging]
                       ,[AutomaticallyPrintAlarmsAndEvents] = existingData.[AutomaticallyPrintAlarmsAndEvents]
                       ,[AlarmAndEventPrinter] = existingData.[AlarmAndEventPrinter]
                       ,[MailServer] = existingData.[MailServer]
                       ,[MailFrom] = existingData.[MailFrom]
                       ,[MailUserName] = existingData.[MailUserName]
                       ,[MailPassword] = existingData.[MailPassword]
                       ,[DialupName] = existingData.[DialupName]
                       ,[SCADASystem] = existingData.[SCADASystem]
                       ,[InhibitTemplateGraphics] = existingData.[InhibitTemplateGraphics]
                       ,[RefreshInterval] = existingData.[RefreshInterval]
                       ,[InhibitEndOfDayOperations] = existingData.[InhibitEndOfDayOperations]
                       ,[InhibitEndOfMonthOperations] = existingData.[InhibitEndOfMonthOperations]
                       ,[EndOfDayWarningPeriod] = existingData.[EndOfDayWarningPeriod]
                       ,[InhibitAutomaticPhysicalInventory] = existingData.[InhibitAutomaticPhysicalInventory]
                       ,[InhibitAutomaticMeterCloseout] = existingData.[InhibitAutomaticMeterCloseout]
                       ,[InhibitAutomaticReportGeneration] = existingData.[InhibitAutomaticReportGeneration]
                       ,[InhibitAutomaticAdjustmentDistribution] = existingData.[InhibitAutomaticAdjustmentDistribution]
                       ,[InhibitAutomaticCloseout] = existingData.[InhibitAutomaticCloseout]
                       ,[InhibitTankScan] = existingData.[InhibitTankScan]
                       ,[ReportDirectory] = existingData.[ReportDirectory]
                       ,[ManageReports] = existingData.[ManageReports]
                       ,[ManagedReportDirectory] = existingData.[ManagedReportDirectory]
                       ,[VRURateLimit] = existingData.[VRURateLimit]
                       ,[VRUHourlyLimit] = existingData.[VRUHourlyLimit]
                       ,[VRUDailyLimit] = existingData.[VRUDailyLimit]
                       ,[VRUYearlyLimit] = existingData.[VRUYearlyLimit]
                       ,[VRUCurrentYearLimit] = existingData.[VRUCurrentYearLimit]
                       ,[VRURateActual] = existingData.[VRURateActual]
                       ,[VRUHourlyActual] = existingData.[VRUHourlyActual]
                       ,[VRUDailyActual] = existingData.[VRUDailyActual]
                       ,[VRUYearlyActual] = existingData.[VRUYearlyActual]
                       ,[VRUCurrentYearActual] = existingData.[VRUCurrentYearActual]
                       ,[VRURateLimitEnabled] = existingData.[VRURateLimitEnabled]
                       ,[VRUHourlyLimitEnabled] = existingData.[VRUHourlyLimitEnabled]
                       ,[VRUDailyLimitEnabled] = existingData.[VRUDailyLimitEnabled]
                       ,[VRUYearlyLimitEnabled] = existingData.[VRUYearlyLimitEnabled]
                       ,[VRUCurrentYearLimitEnabled] = existingData.[VRUCurrentYearLimitEnabled]
                       ,[WatchdogPeriod] = existingData.[WatchdogPeriod]
                       ,[WatchdogCounterStart] = existingData.[WatchdogCounterStart]
                       ,[WatchdogCounterEnd] = existingData.[WatchdogCounterEnd]
                       ,[NumberDecimalSeparator] = existingData.[NumberDecimalSeparator]
                       ,[NumberGroupSeparator] = existingData.[NumberGroupSeparator]
                       ,[ListSeparator] = existingData.[ListSeparator]
                       ,[TimePattern] = existingData.[TimePattern]
                       ,[TimeSeparator] = existingData.[TimeSeparator]
                       ,[AMSymbol] = existingData.[AMSymbol]
                       ,[PMSymbol] = existingData.[PMSymbol]
                       ,[ShortDatePattern] = existingData.[ShortDatePattern]
                       ,[DateSeparator] = existingData.[DateSeparator]
                       ,[LongDatePattern] = existingData.[LongDatePattern]
                       ,[TwoDigitCalendarEndYear] = existingData.[TwoDigitCalendarEndYear]
                       ,[UserData1] = existingData.[UserData1]
                       ,[UserData2] = existingData.[UserData2]
                       ,[UserData3] = existingData.[UserData3]
                       ,[UserData4] = existingData.[UserData4]
                       ,[UserData5] = existingData.[UserData5]
                       ,[UserData6] = existingData.[UserData6]
                       ,[UserData7] = existingData.[UserData7]
                       ,[UserData8] = existingData.[UserData8]
                       ,[CreatedDate] = existingData.[CreatedDate]
                       ,[CreatedBy] = existingData.[CreatedBy]
					   ,[UpdatedDate] = SYSDATETIMEOFFSET()
					   ,[UpdatedBy] = N''V9 Upgrade. AAC''
                       ,[MinTimeAllowedToChangePwd] = existingData.[MinTimeAllowedToChangePwd]
                       ,[MinPwdCharacterLength] = existingData.[MinPwdCharacterLength]
                       ,[PwdExpirationInDays] = existingData.[PwdExpirationInDays]
                       ,[PwdLockoutThreshold] = existingData.[PwdLockoutThreshold]
                       ,[CheckForPreviousPwd] = existingData.[CheckForPreviousPwd]
                       ,[StrongPwdUse] = existingData.[StrongPwdUse]
                       ,[PwdHistoryCount] = existingData.[PwdHistoryCount]
                       ,[ApplyToAllSiteMembers] = existingData.[ApplyToAllSiteMembers]
                       ,[InactivityDisablePeriod] = existingData.[InactivityDisablePeriod]
                       ,[EnforceSingleOwner] = existingData.[EnforceSingleOwner]
                       ,[InhibitBOLSummaryAutoPopulate] = existingData.[InhibitBOLSummaryAutoPopulate]
                       ,[InhibitOrderSummaryAutoPopulate] = existingData.[InhibitOrderSummaryAutoPopulate]
                       ,[InhibitSupplyOrderSummaryAutoPopulate] = existingData.[InhibitSupplyOrderSummaryAutoPopulate]
                       ,[InvoiceStartNumber] = existingData.[InvoiceStartNumber]
                       ,[InvoiceEndNumber] = existingData.[InvoiceEndNumber]
                       ,[PromptForReturns] = existingData.[PromptForReturns]
                       ,[PromptForTruckCard] = existingData.[PromptForTruckCard]
                       ,[StartingShortCardNumber] = existingData.[StartingShortCardNumber]
                       ,[UseShortCardNumber] = existingData.[UseShortCardNumber]
                       ,[ExcessVarianceCount] = existingData.[ExcessVarianceCount]
                       ,[ExcessVarianceTolerance] = existingData.[ExcessVarianceTolerance]
                       ,[DisableArchivePeriod] = existingData.[DisableArchivePeriod]
                       ,[ExportArchiveDir] = existingData.[ExportArchiveDir]
                       ,[ImportArchiveDir] = existingData.[ImportArchiveDir]
                       ,[GroupLedgerByID] = existingData.[GroupLedgerByID]
                       ,[InhibitSiteLedgerRollup] = existingData.[InhibitSiteLedgerRollup]
                       ,[UseTankReconciliation] = existingData.[UseTankReconciliation]
                       ,[LookupNumberGroupSizesTypeIndex] = existingData.[LookupNumberGroupSizesTypeIndex]
                       ,[LookupQuantityDisplayDefaultIndex] = existingData.[LookupQuantityDisplayDefaultIndex]
                       ,[LookupSecondaryStorageFillMethodIndex] = existingData.[LookupSecondaryStorageFillMethodIndex]
                       ,[LookupMailConnectModeIndex] = existingData.[LookupMailConnectModeIndex]
                       ,[LookupWatchdogModeIndex] = existingData.[LookupWatchdogModeIndex]
                       ,[Contact1Name] = existingData.[Contact1Name]
                       ,[Contact1Address1] = existingData.[Contact1Address1]
                       ,[Contact1Address2] = existingData.[Contact1Address2]
                       ,[Contact1City] = existingData.[Contact1City]
                       ,[Contact1State] = existingData.[Contact1State]
                       ,[Contact1Zip] = existingData.[Contact1Zip]
                       ,[Contact1Country] = existingData.[Contact1Country]
                       ,[Contact1PhoneOffice] = existingData.[Contact1PhoneOffice]
                       ,[Contact1Fax] = existingData.[Contact1Fax]
                       ,[Contact1EmailAddress] = existingData.[Contact1EmailAddress]
                       ,[Contact2Name] = existingData.[Contact2Name]
                       ,[Contact2Address1] = existingData.[Contact2Address1]
                       ,[Contact2Address2] = existingData.[Contact2Address2]
                       ,[Contact2City] = existingData.[Contact2City]
                       ,[Contact2State] = existingData.[Contact2State]
                       ,[Contact2Zip] = existingData.[Contact2Zip]
                       ,[Contact2Country] = existingData.[Contact2Country]
                       ,[Contact2PhoneOffice] = existingData.[Contact2PhoneOffice]
                       ,[Contact2Fax] = existingData.[Contact2Fax]
                       ,[Contact2EmailAddress] = existingData.[Contact2EmailAddress]
                       ,[Contact1PhoneMobile] = existingData.[Contact1PhoneMobile]
                       ,[Contact2PhoneMobile] = existingData.[Contact2PhoneMobile]
                       ,[EnablePasswordHint] = existingData.[EnablePasswordHint]
                       ,[EnablePasswordReset] = existingData.[EnablePasswordReset]
                       ,[MeterReconciliationToleranceIsPercent] = existingData.[MeterReconciliationToleranceIsPercent]
                       ,[MeterReconciliationReportName] = existingData.[MeterReconciliationReportName]
                       ,[TranslatedHelpURL] = existingData.[TranslatedHelpURL]
                       ,[AllowUseOfSpecialChars] = existingData.[AllowUseOfSpecialChars]
                       ,[EnablePeriodicSyncFlag] = existingData.[EnablePeriodicSyncFlag]
                       ,[PeriodicSyncIntervalMinutes] = existingData.[PeriodicSyncIntervalMinutes]
                       ,[CardInTimeout] = existingData.[CardInTimeout]
                       ,[TerminalControlNumber] = existingData.[TerminalControlNumber]
                       ,[BlockCloseOnUnpostedBOL] = existingData.[BlockCloseOnUnpostedBOL]
                       ,[InhibitLoadRackCardIns] = existingData.[InhibitLoadRackCardIns]
                       ,[PromptForThirdTrailer] = existingData.[PromptForThirdTrailer]
                       ,[PromptForTransactionCompletion] = existingData.[PromptForTransactionCompletion]
                       ,[InhibitCustomerConfirmationPrompt] = existingData.[InhibitCustomerConfirmationPrompt]
                       ,[EnableBOLPDFArchiving] = existingData.[EnableBOLPDFArchiving]
                       ,[BOLPDFArchivingPath] = existingData.[BOLPDFArchivingPath]
                       ,[RequireTrailerScully] = existingData.[RequireTrailerScully]
                       ,[Latitude] = existingData.[Latitude]
                       ,[Longitude] = existingData.[Longitude]
                       ,[Zoom] = existingData.[Zoom]
                       ,[GlobalAccessToPersonnel] = existingData.[GlobalAccessToPersonnel]
                       ,[GlobalAccessToEquipment] = existingData.[GlobalAccessToEquipment]
                       ,[Enterprise] = existingData.[Enterprise]
                       ,[OperateTabGroups] = existingData.[OperateTabGroups]
                       ,[EnterpriseUserId] = existingData.[EnterpriseUserId]
                       ,[EnterprisePassword] = existingData.[EnterprisePassword]
                       ,[EnterpriseSite] = existingData.[EnterpriseSite]
            WHEN NOT MATCHED THEN
                INSERT ([ID],[Number],[SPLCCode],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmailAddress],[EmergencyContact],[EmergencyPhone],[Enabled],[SiteGroupFlag],[TimeZone],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[AdditiveProfileCycleAmountDecimalPlaces],[AdditiveProfileRateDecimalPlaces],[InhibitAccessAfterHours],[InhibitMultipleCardIns],[AccessCardInRequired],[CheckSiteNumber],[PromptForCustomerCard],[PromptForTractorOrTanker],[PromptForFirstTrailer],[PromptForSecondTrailer],[PromptForCompartment],[EnforceDriverEquipmentMatch],[EnableAdditiveAccounting],[UseCompanyEquipmentIdentifiers],[UseLastKnownGoodTankData],[MaximumLoadAmount],[MaximumLoadTime],[MaximumIdleTime],[MaximumFlushAmount],[MaximumMeterProvingAmount],[MaximumReturnsAmount],[MaximumNumberOfActiveArms],[DriverTimeoutPeriod],[DriverWarningPeriod],[MaximumPrompts],[MaximumVehicleWeight],[LoadByNet],[PromptForShipmentNumber],[MaximumProductTemperature],[ListEquipment],[DeferStationChanges],[InhibitBOLWithBrokenBlends],[InhibitBOLWithImproperAdditization],[InhibitOverweightBOL],[ExceptionBOLPrinter],[EnableAutomaticBOLPrinting],[AutomaticBOLStartNumber],[AutomaticBOLEndNumber],[SeparateManualBOLNumbering],[ManualBOLStartNumber],[ManualBOLEndNumber],[TransactionStartNumber],[TransactionEndNumber],[OrderStartNumber],[OrderEndNumber],[OpenTransactionWindow],[AdministrativeLockDate],[OperationalLockDate],[MaximumDaysToRetainLogs],[EnableDebugLogging],[EnableAuditLogging],[AutomaticallyPrintAlarmsAndEvents],[AlarmAndEventPrinter],[MailServer],[MailFrom],[MailUserName],[MailPassword],[DialupName],[SCADASystem],[InhibitTemplateGraphics],[RefreshInterval],[InhibitEndOfDayOperations],[InhibitEndOfMonthOperations],[EndOfDayWarningPeriod],[InhibitAutomaticPhysicalInventory],[InhibitAutomaticMeterCloseout],[InhibitAutomaticReportGeneration],[InhibitAutomaticAdjustmentDistribution],[InhibitAutomaticCloseout],[InhibitTankScan],[ReportDirectory],[ManageReports],[ManagedReportDirectory],[VRURateLimit],[VRUHourlyLimit],[VRUDailyLimit],[VRUYearlyLimit],[VRUCurrentYearLimit],[VRURateActual],[VRUHourlyActual],[VRUDailyActual],[VRUYearlyActual],[VRUCurrentYearActual],[VRURateLimitEnabled],[VRUHourlyLimitEnabled],[VRUDailyLimitEnabled],[VRUYearlyLimitEnabled],[VRUCurrentYearLimitEnabled],[WatchdogPeriod],[WatchdogCounterStart],[WatchdogCounterEnd],[NumberDecimalSeparator],[NumberGroupSeparator],[ListSeparator],[TimePattern],[TimeSeparator],[AMSymbol],[PMSymbol],[ShortDatePattern],[DateSeparator],[LongDatePattern],[TwoDigitCalendarEndYear],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MinTimeAllowedToChangePwd],[MinPwdCharacterLength],[PwdExpirationInDays],[PwdLockoutThreshold],[CheckForPreviousPwd],[StrongPwdUse],[PwdHistoryCount],[ApplyToAllSiteMembers],[InactivityDisablePeriod],[EnforceSingleOwner],[InhibitBOLSummaryAutoPopulate],[InhibitOrderSummaryAutoPopulate],[InhibitSupplyOrderSummaryAutoPopulate],[InvoiceStartNumber],[InvoiceEndNumber],[PromptForReturns],[PromptForTruckCard],[StartingShortCardNumber],[UseShortCardNumber],[ExcessVarianceCount],[ExcessVarianceTolerance],[DisableArchivePeriod],[ExportArchiveDir],[ImportArchiveDir],[GroupLedgerByID],[InhibitSiteLedgerRollup],[UseTankReconciliation],[SiteGuid],[LookupNumberGroupSizesTypeIndex],[LookupQuantityDisplayDefaultIndex],[LookupSecondaryStorageFillMethodIndex],[LookupMailConnectModeIndex],[LookupWatchdogModeIndex],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[EnablePasswordHint],[EnablePasswordReset],[MeterReconciliationToleranceIsPercent],[MeterReconciliationReportName],[TranslatedHelpURL],[AllowUseOfSpecialChars],[EnablePeriodicSyncFlag],[PeriodicSyncIntervalMinutes],[CardInTimeout],[TerminalControlNumber],[BlockCloseOnUnpostedBOL],[InhibitLoadRackCardIns],[PromptForThirdTrailer],[PromptForTransactionCompletion],[InhibitCustomerConfirmationPrompt],[EnableBOLPDFArchiving],[BOLPDFArchivingPath],[RequireTrailerScully],[Latitude],[Longitude],[Zoom],[GlobalAccessToPersonnel],[GlobalAccessToEquipment],[Enterprise],[OperateTabGroups],[EnterpriseUserId],[EnterprisePassword],[EnterpriseSite])
                    VALUES (remoteChanges.[ID],remoteChanges.[Number],remoteChanges.[SPLCCode],remoteChanges.[Address1],remoteChanges.[Address2],remoteChanges.[City],remoteChanges.[State],remoteChanges.[Zip],remoteChanges.[Country],remoteChanges.[Phone],remoteChanges.[FAX],remoteChanges.[EmailAddress],remoteChanges.[EmergencyContact],remoteChanges.[EmergencyPhone],remoteChanges.[Enabled],remoteChanges.[SiteGroupFlag],remoteChanges.[TimeZone],remoteChanges.[LevelUnitIndex],remoteChanges.[TemperatureUnitIndex],remoteChanges.[DensityUnitIndex],remoteChanges.[PressureUnitIndex],remoteChanges.[FlowUnitIndex],remoteChanges.[VolumeUnitIndex],remoteChanges.[MassUnitIndex],remoteChanges.[AdditiveVolumeUnitIndex],remoteChanges.[AdditiveProfileCycleAmountUnitIndex],remoteChanges.[AdditiveProfileRateUnitIndex],remoteChanges.[LevelDecimalPlaces],remoteChanges.[TemperatureDecimalPlaces],remoteChanges.[DensityDecimalPlaces],remoteChanges.[PressureDecimalPlaces],remoteChanges.[FlowDecimalPlaces],remoteChanges.[VolumeDecimalPlaces],remoteChanges.[MassDecimalPlaces],remoteChanges.[AdditiveVolumeDecimalPlaces],remoteChanges.[AdditiveProfileCycleAmountDecimalPlaces],remoteChanges.[AdditiveProfileRateDecimalPlaces],remoteChanges.[InhibitAccessAfterHours],remoteChanges.[InhibitMultipleCardIns],remoteChanges.[AccessCardInRequired],remoteChanges.[CheckSiteNumber],remoteChanges.[PromptForCustomerCard],remoteChanges.[PromptForTractorOrTanker],remoteChanges.[PromptForFirstTrailer],remoteChanges.[PromptForSecondTrailer],remoteChanges.[PromptForCompartment],remoteChanges.[EnforceDriverEquipmentMatch],remoteChanges.[EnableAdditiveAccounting],remoteChanges.[UseCompanyEquipmentIdentifiers],remoteChanges.[UseLastKnownGoodTankData],remoteChanges.[MaximumLoadAmount],remoteChanges.[MaximumLoadTime],remoteChanges.[MaximumIdleTime],remoteChanges.[MaximumFlushAmount],remoteChanges.[MaximumMeterProvingAmount],remoteChanges.[MaximumReturnsAmount],remoteChanges.[MaximumNumberOfActiveArms],remoteChanges.[DriverTimeoutPeriod],remoteChanges.[DriverWarningPeriod],remoteChanges.[MaximumPrompts],remoteChanges.[MaximumVehicleWeight],remoteChanges.[LoadByNet],remoteChanges.[PromptForShipmentNumber],remoteChanges.[MaximumProductTemperature],remoteChanges.[ListEquipment],remoteChanges.[DeferStationChanges],remoteChanges.[InhibitBOLWithBrokenBlends],remoteChanges.[InhibitBOLWithImproperAdditization],remoteChanges.[InhibitOverweightBOL],remoteChanges.[ExceptionBOLPrinter],remoteChanges.[EnableAutomaticBOLPrinting],remoteChanges.[AutomaticBOLStartNumber],remoteChanges.[AutomaticBOLEndNumber],remoteChanges.[SeparateManualBOLNumbering],remoteChanges.[ManualBOLStartNumber],remoteChanges.[ManualBOLEndNumber],remoteChanges.[TransactionStartNumber],remoteChanges.[TransactionEndNumber],remoteChanges.[OrderStartNumber],remoteChanges.[OrderEndNumber],remoteChanges.[OpenTransactionWindow],remoteChanges.[AdministrativeLockDate],remoteChanges.[OperationalLockDate],remoteChanges.[MaximumDaysToRetainLogs],remoteChanges.[EnableDebugLogging],remoteChanges.[EnableAuditLogging],remoteChanges.[AutomaticallyPrintAlarmsAndEvents],remoteChanges.[AlarmAndEventPrinter],remoteChanges.[MailServer],remoteChanges.[MailFrom],remoteChanges.[MailUserName],remoteChanges.[MailPassword],remoteChanges.[DialupName],remoteChanges.[SCADASystem],remoteChanges.[InhibitTemplateGraphics],remoteChanges.[RefreshInterval],remoteChanges.[InhibitEndOfDayOperations],remoteChanges.[InhibitEndOfMonthOperations],remoteChanges.[EndOfDayWarningPeriod],remoteChanges.[InhibitAutomaticPhysicalInventory],remoteChanges.[InhibitAutomaticMeterCloseout],remoteChanges.[InhibitAutomaticReportGeneration],remoteChanges.[InhibitAutomaticAdjustmentDistribution],remoteChanges.[InhibitAutomaticCloseout],remoteChanges.[InhibitTankScan],remoteChanges.[ReportDirectory],remoteChanges.[ManageReports],remoteChanges.[ManagedReportDirectory],remoteChanges.[VRURateLimit],remoteChanges.[VRUHourlyLimit],remoteChanges.[VRUDailyLimit],remoteChanges.[VRUYearlyLimit],remoteChanges.[VRUCurrentYearLimit],remoteChanges.[VRURateActual],remoteChanges.[VRUHourlyActual],remoteChanges.[VRUDailyActual],remoteChanges.[VRUYearlyActual],remoteChanges.[VRUCurrentYearActual],remoteChanges.[VRURateLimitEnabled],remoteChanges.[VRUHourlyLimitEnabled],remoteChanges.[VRUDailyLimitEnabled],remoteChanges.[VRUYearlyLimitEnabled],remoteChanges.[VRUCurrentYearLimitEnabled],remoteChanges.[WatchdogPeriod],remoteChanges.[WatchdogCounterStart],remoteChanges.[WatchdogCounterEnd],remoteChanges.[NumberDecimalSeparator],remoteChanges.[NumberGroupSeparator],remoteChanges.[ListSeparator],remoteChanges.[TimePattern],remoteChanges.[TimeSeparator],remoteChanges.[AMSymbol],remoteChanges.[PMSymbol],remoteChanges.[ShortDatePattern],remoteChanges.[DateSeparator],remoteChanges.[LongDatePattern],remoteChanges.[TwoDigitCalendarEndYear],remoteChanges.[UserData1],remoteChanges.[UserData2],remoteChanges.[UserData3],remoteChanges.[UserData4],remoteChanges.[UserData5],remoteChanges.[UserData6],remoteChanges.[UserData7],remoteChanges.[UserData8],remoteChanges.[CreatedDate],remoteChanges.[CreatedBy],remoteChanges.[UpdatedDate],remoteChanges.[UpdatedBy],remoteChanges.[MinTimeAllowedToChangePwd],remoteChanges.[MinPwdCharacterLength],remoteChanges.[PwdExpirationInDays],remoteChanges.[PwdLockoutThreshold],remoteChanges.[CheckForPreviousPwd],remoteChanges.[StrongPwdUse],remoteChanges.[PwdHistoryCount],remoteChanges.[ApplyToAllSiteMembers],remoteChanges.[InactivityDisablePeriod],remoteChanges.[EnforceSingleOwner],remoteChanges.[InhibitBOLSummaryAutoPopulate],remoteChanges.[InhibitOrderSummaryAutoPopulate],remoteChanges.[InhibitSupplyOrderSummaryAutoPopulate],remoteChanges.[InvoiceStartNumber],remoteChanges.[InvoiceEndNumber],remoteChanges.[PromptForReturns],remoteChanges.[PromptForTruckCard],remoteChanges.[StartingShortCardNumber],remoteChanges.[UseShortCardNumber],remoteChanges.[ExcessVarianceCount],remoteChanges.[ExcessVarianceTolerance],remoteChanges.[DisableArchivePeriod],remoteChanges.[ExportArchiveDir],remoteChanges.[ImportArchiveDir],remoteChanges.[GroupLedgerByID],remoteChanges.[InhibitSiteLedgerRollup],remoteChanges.[UseTankReconciliation],remoteChanges.[SiteGuid],remoteChanges.[LookupNumberGroupSizesTypeIndex],remoteChanges.[LookupQuantityDisplayDefaultIndex],remoteChanges.[LookupSecondaryStorageFillMethodIndex],remoteChanges.[LookupMailConnectModeIndex],remoteChanges.[LookupWatchdogModeIndex],remoteChanges.[Contact1Name],remoteChanges.[Contact1Address1],remoteChanges.[Contact1Address2],remoteChanges.[Contact1City],remoteChanges.[Contact1State],remoteChanges.[Contact1Zip],remoteChanges.[Contact1Country],remoteChanges.[Contact1PhoneOffice],remoteChanges.[Contact1Fax],remoteChanges.[Contact1EmailAddress],remoteChanges.[Contact2Name],remoteChanges.[Contact2Address1],remoteChanges.[Contact2Address2],remoteChanges.[Contact2City],remoteChanges.[Contact2State],remoteChanges.[Contact2Zip],remoteChanges.[Contact2Country],remoteChanges.[Contact2PhoneOffice],remoteChanges.[Contact2Fax],remoteChanges.[Contact2EmailAddress],remoteChanges.[Contact1PhoneMobile],remoteChanges.[Contact2PhoneMobile],remoteChanges.[EnablePasswordHint],remoteChanges.[EnablePasswordReset],remoteChanges.[MeterReconciliationToleranceIsPercent],remoteChanges.[MeterReconciliationReportName],remoteChanges.[TranslatedHelpURL],remoteChanges.[AllowUseOfSpecialChars],remoteChanges.[EnablePeriodicSyncFlag],remoteChanges.[PeriodicSyncIntervalMinutes],remoteChanges.[CardInTimeout],remoteChanges.[TerminalControlNumber],remoteChanges.[BlockCloseOnUnpostedBOL],remoteChanges.[InhibitLoadRackCardIns],remoteChanges.[PromptForThirdTrailer],remoteChanges.[PromptForTransactionCompletion],remoteChanges.[InhibitCustomerConfirmationPrompt],remoteChanges.[EnableBOLPDFArchiving],remoteChanges.[BOLPDFArchivingPath],remoteChanges.[RequireTrailerScully],remoteChanges.[Latitude],remoteChanges.[Longitude],remoteChanges.[Zoom],remoteChanges.[GlobalAccessToPersonnel],remoteChanges.[GlobalAccessToEquipment],remoteChanges.[Enterprise],remoteChanges.[OperateTabGroups],remoteChanges.[EnterpriseUserId],remoteChanges.[EnterprisePassword],remoteChanges.[EnterpriseSite])
			;

--
-- Note: Intentionally skipped [dbo].[tblSitesAncillaryData] - this will be populated later in the migration script.
--

--
-- Merge Enterprise tblUser Records and UserGuids
--
        ;   WITH existingData AS (
                SELECT [dbo].[tblUsers].[UserIndex],[dbo].[tblUsers].[SiteIndex],[dbo].[tblUsers].[UserID],[dbo].[tblUsers].[Password],[dbo].[tblUsers].[LastLoginDate],[dbo].[tblUsers].[LastLogoffDate],[dbo].[tblUsers].[ChangePassword],[dbo].[tblUsers].[PasswordTimeStamp],[dbo].[tblUsers].[Name],[dbo].[tblUsers].[EmailAddress],[dbo].[tblUsers].[CreatedDate],[dbo].[tblUsers].[CreatedBy],[dbo].[tblUsers].[UpdatedDate],[dbo].[tblUsers].[UpdatedBy],[dbo].[tblUsers].[PasswordHistory1],[dbo].[tblUsers].[PasswordHistory2],[dbo].[tblUsers].[PasswordHistory3],[dbo].[tblUsers].[PasswordHistory4],[dbo].[tblUsers].[PasswordHistory5],[dbo].[tblUsers].[PasswordHistory6],[dbo].[tblUsers].[PasswordHistory7],[dbo].[tblUsers].[PasswordHistory8],[dbo].[tblUsers].[PasswordHistory9],[dbo].[tblUsers].[PasswordHistory10],[dbo].[tblUsers].[PasswordHistory11],[dbo].[tblUsers].[PasswordHistory12],[dbo].[tblUsers].[PasswordHistory13],[dbo].[tblUsers].[PasswordHistory14],[dbo].[tblUsers].[PasswordHistory15],[dbo].[tblUsers].[PasswordHistory16],[dbo].[tblUsers].[PasswordHistory17],[dbo].[tblUsers].[PasswordHistory18],[dbo].[tblUsers].[PasswordHistory19],[dbo].[tblUsers].[PasswordHistory20],[dbo].[tblUsers].[PasswordHistory21],[dbo].[tblUsers].[PasswordHistory22],[dbo].[tblUsers].[PasswordHistory23],[dbo].[tblUsers].[PasswordHistory24],[dbo].[tblUsers].[PasswordLockoutCount],[dbo].[tblUsers].[InactivityLockout],[dbo].[tblUsers].[InactivityLockoutDate],[dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid],[dbo].[tblUsers].[PasswordHint],[dbo].[tblUsers].[UserData1],[dbo].[tblUsers].[UserData2],[dbo].[tblUsers].[UserData3],[dbo].[tblUsers].[UserData4],[dbo].[tblUsers].[UserData5],[dbo].[tblUsers].[UserData6],[dbo].[tblUsers].[UserData7],[dbo].[tblUsers].[UserData8],[dbo].[tblUsers].[PhoneNumber],[dbo].[tblUsers].[AccountExpirationDate]
                    FROM [dbo].[tblUsers]
            ) MERGE existingData
            USING (SELECT -1,-1,[FuelsManagerDB_Template].[dbo].[tblUsers].[UserID],[FuelsManagerDB_Template].[dbo].[tblUsers].[Password],[FuelsManagerDB_Template].[dbo].[tblUsers].[LastLoginDate],[FuelsManagerDB_Template].[dbo].[tblUsers].[LastLogoffDate],[FuelsManagerDB_Template].[dbo].[tblUsers].[ChangePassword],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordTimeStamp],[FuelsManagerDB_Template].[dbo].[tblUsers].[Name],[FuelsManagerDB_Template].[dbo].[tblUsers].[EmailAddress],[FuelsManagerDB_Template].[dbo].[tblUsers].[CreatedDate],[FuelsManagerDB_Template].[dbo].[tblUsers].[CreatedBy],[FuelsManagerDB_Template].[dbo].[tblUsers].[UpdatedDate],[FuelsManagerDB_Template].[dbo].[tblUsers].[UpdatedBy],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory1],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory2],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory3],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory4],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory5],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory6],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory7],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory8],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory9],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory10],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory11],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory12],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory13],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory14],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory15],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory16],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory17],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory18],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory19],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory20],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory21],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory22],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory23],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHistory24],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordLockoutCount],[FuelsManagerDB_Template].[dbo].[tblUsers].[InactivityLockout],[FuelsManagerDB_Template].[dbo].[tblUsers].[InactivityLockoutDate],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserGuid],[FuelsManagerDB_Template].[dbo].[tblUsers].[SiteGuid],[FuelsManagerDB_Template].[dbo].[tblUsers].[PasswordHint],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData1],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData2],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData3],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData4],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData5],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData6],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData7],[FuelsManagerDB_Template].[dbo].[tblUsers].[UserData8],[FuelsManagerDB_Template].[dbo].[tblUsers].[PhoneNumber],[FuelsManagerDB_Template].[dbo].[tblUsers].[AccountExpirationDate] FROM [FuelsManagerDB_Template].[dbo].[tblUsers]
                    ) AS remoteChanges ([UserIndex],[SiteIndex],[UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[UserGuid],[SiteGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate])
            ON (existingData.[UserID] = remoteChanges.[UserID])
            WHEN MATCHED AND (remoteChanges.UpdatedDate >= existingData.UpdatedDate)
                THEN
                UPDATE SET [UserGuid] = remoteChanges.[UserGuid]
							,[Password] = remoteChanges.[Password]
							,[ChangePassword] = remoteChanges.[ChangePassword]
							,[PasswordTimeStamp] = remoteChanges.[PasswordTimeStamp]
							,[Name] = remoteChanges.[Name]
							,[EmailAddress] = remoteChanges.[EmailAddress]
							,[CreatedDate] = remoteChanges.[CreatedDate]
							,[CreatedBy] = remoteChanges.[CreatedBy]
							,[UpdatedDate] = remoteChanges.[UpdatedDate]
							,[UpdatedBy] = remoteChanges.[UpdatedBy]
							,[PasswordHistory1] = remoteChanges.[PasswordHistory1]
							,[PasswordHistory2] = remoteChanges.[PasswordHistory2]
							,[PasswordHistory3] = remoteChanges.[PasswordHistory3]
							,[PasswordHistory4] = remoteChanges.[PasswordHistory4]
							,[PasswordHistory5] = remoteChanges.[PasswordHistory5]
							,[PasswordHistory6] = remoteChanges.[PasswordHistory6]
							,[PasswordHistory7] = remoteChanges.[PasswordHistory7]
							,[PasswordHistory8] = remoteChanges.[PasswordHistory8]
							,[PasswordHistory9] = remoteChanges.[PasswordHistory9]
							,[PasswordHistory10] = remoteChanges.[PasswordHistory10]
							,[PasswordHistory11] = remoteChanges.[PasswordHistory11]
							,[PasswordHistory12] = remoteChanges.[PasswordHistory12]
							,[PasswordHistory13] = remoteChanges.[PasswordHistory13]
							,[PasswordHistory14] = remoteChanges.[PasswordHistory14]
							,[PasswordHistory15] = remoteChanges.[PasswordHistory15]
							,[PasswordHistory16] = remoteChanges.[PasswordHistory16]
							,[PasswordHistory17] = remoteChanges.[PasswordHistory17]
							,[PasswordHistory18] = remoteChanges.[PasswordHistory18]
							,[PasswordHistory19] = remoteChanges.[PasswordHistory19]
							,[PasswordHistory20] = remoteChanges.[PasswordHistory20]
							,[PasswordHistory21] = remoteChanges.[PasswordHistory21]
							,[PasswordHistory22] = remoteChanges.[PasswordHistory22]
							,[PasswordHistory23] = remoteChanges.[PasswordHistory23]
							,[PasswordHistory24] = remoteChanges.[PasswordHistory24]
							,[PasswordLockoutCount] = remoteChanges.[PasswordLockoutCount]
							,[InactivityLockout] = remoteChanges.[InactivityLockout]
							,[InactivityLockoutDate] = remoteChanges.[InactivityLockoutDate]
							,[PasswordHint] = remoteChanges.[PasswordHint]
							,[SiteGuid] = remoteChanges.[SiteGuid]
							,[PhoneNumber] = remoteChanges.[PhoneNumber]
							,[AccountExpirationDate] = remoteChanges.[AccountExpirationDate]
            WHEN NOT MATCHED THEN
                INSERT ([SiteIndex],[UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[UserGuid],[SiteGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate])
					VALUES (1,remoteChanges.[UserID],remoteChanges.[Password],remoteChanges.[LastLoginDate],remoteChanges.[LastLogoffDate],remoteChanges.[ChangePassword],remoteChanges.[PasswordTimeStamp],remoteChanges.[Name],remoteChanges.[EmailAddress],remoteChanges.[CreatedDate],remoteChanges.[CreatedBy],remoteChanges.[UpdatedDate],remoteChanges.[UpdatedBy],remoteChanges.[PasswordHistory1],remoteChanges.[PasswordHistory2],remoteChanges.[PasswordHistory3],remoteChanges.[PasswordHistory4],remoteChanges.[PasswordHistory5],remoteChanges.[PasswordHistory6],remoteChanges.[PasswordHistory7],remoteChanges.[PasswordHistory8],remoteChanges.[PasswordHistory9],remoteChanges.[PasswordHistory10],remoteChanges.[PasswordHistory11],remoteChanges.[PasswordHistory12],remoteChanges.[PasswordHistory13],remoteChanges.[PasswordHistory14],remoteChanges.[PasswordHistory15],remoteChanges.[PasswordHistory16],remoteChanges.[PasswordHistory17],remoteChanges.[PasswordHistory18],remoteChanges.[PasswordHistory19],remoteChanges.[PasswordHistory20],remoteChanges.[PasswordHistory21],remoteChanges.[PasswordHistory22],remoteChanges.[PasswordHistory23],remoteChanges.[PasswordHistory24],remoteChanges.[PasswordLockoutCount],remoteChanges.[InactivityLockout],remoteChanges.[InactivityLockoutDate],remoteChanges.[UserGuid],remoteChanges.[SiteGuid],remoteChanges.[PasswordHint],remoteChanges.[UserData1],remoteChanges.[UserData2],remoteChanges.[UserData3],remoteChanges.[UserData4],remoteChanges.[UserData5],remoteChanges.[UserData6],remoteChanges.[UserData7],remoteChanges.[UserData8],remoteChanges.[PhoneNumber],remoteChanges.[AccountExpirationDate])
            ;

--
-- Merge Enterprise tblGroup Records and GroupGuids
--
        ;   WITH existingData AS (
                SELECT [dbo].[tblGroups].[SiteIndex],[dbo].[tblGroups].[GroupID],[dbo].[tblGroups].[GroupDescription],[dbo].[tblGroups].[SessionTimeout],[dbo].[tblGroups].[CreatedDate],[dbo].[tblGroups].[CreatedBy],[dbo].[tblGroups].[UpdatedDate],[dbo].[tblGroups].[UpdatedBy],[dbo].[tblGroups].[GroupGuid],[dbo].[tblGroups].[SiteGuid]
                    FROM [dbo].[tblGroups]
            ) MERGE existingData
            USING (SELECT 1,[FuelsManagerDB_Template].[dbo].[tblGroups].[GroupID],[FuelsManagerDB_Template].[dbo].[tblGroups].[GroupDescription],[FuelsManagerDB_Template].[dbo].[tblGroups].[SessionTimeout],[FuelsManagerDB_Template].[dbo].[tblGroups].[CreatedDate],[FuelsManagerDB_Template].[dbo].[tblGroups].[CreatedBy],[FuelsManagerDB_Template].[dbo].[tblGroups].[UpdatedDate],[FuelsManagerDB_Template].[dbo].[tblGroups].[UpdatedBy],[FuelsManagerDB_Template].[dbo].[tblGroups].[GroupGuid],[FuelsManagerDB_Template].[dbo].[tblGroups].[SiteGuid] FROM [FuelsManagerDB_Template].[dbo].[tblGroups]
                    ) AS remoteChanges ([SiteIndex],[GroupID],[GroupDescription],[SessionTimeout],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GroupGuid],[SiteGuid])
            ON (existingData.[GroupID] = remoteChanges.[GroupID])
            WHEN MATCHED AND (remoteChanges.UpdatedDate >= existingData.UpdatedDate)
                THEN
                UPDATE SET [GroupGuid] = remoteChanges.[GroupGuid]
                       ,[GroupDescription] = remoteChanges.[GroupDescription]
                       ,[SessionTimeout] = remoteChanges.[SessionTimeout]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]

            WHEN NOT MATCHED THEN
                INSERT ([SiteIndex],[GroupID],[GroupDescription],[SessionTimeout],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GroupGuid],[SiteGuid])
                    VALUES (1,remoteChanges.[GroupID],remoteChanges.[GroupDescription],remoteChanges.[SessionTimeout],remoteChanges.[CreatedDate],remoteChanges.[CreatedBy],remoteChanges.[UpdatedDate],remoteChanges.[UpdatedBy],remoteChanges.[GroupGuid],remoteChanges.[SiteGuid])
            ;

	PRINT ''Completed successfully''

GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-1 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-1 Rearrange Data', 
		@step_id=14, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET ANSI_NULLS, QUOTED_IDENTIFIER ON;

/* REMOVE Maintenance Log Records that are referencing DELETED Equipment or Tank records */
DELETE ml FROM tblEquipmentMaintenanceLog ml LEFT JOIN tblequipment e ON ml.Equipmentindex = e.[Index] WHERE e.[index] IS NULL
DELETE ml FROM tblTankMaintenanceLog ml LEFT JOIN tblTanks e ON ml.Tankindex = e.[TankIndex] WHERE e.[Tankindex] IS NULL

UPDATE  t1
SET id =
	CASE [TypeIndex]
		WHEN 1 THEN ''Ledger''
		WHEN 2 THEN ''Meter Reconciliation''
		WHEN 3 THEN ''Receipt Reconciliation''
		WHEN 4 THEN ''Inventory Reconciliation''
		WHEN 5 THEN ''Closeout''
		WHEN 6 THEN ''Equipment Transaction''
		WHEN 7 THEN ''Receipt Assignment Assigned''
		WHEN 8 THEN ''Receipt Assignment Available''
		WHEN 10 THEN ''Automatic Physical Inventory''
		WHEN 11 THEN ''Order Summary''
		WHEN 12 THEN ''Order Associated Transactions''
		WHEN 13 THEN ''BOL Summary''
		WHEN 14 THEN ''Supply Order Summary''
		WHEN 15 THEN ''Supply Order Associated Transactions''
		WHEN 16 THEN ''Payment Invoice Summary''
		WHEN 17 THEN ''Payment Associated Transactions''
		WHEN 18 THEN ''Recovery Invoice Summary''
		WHEN 19 THEN ''Recovery Associated Transactions''
	END
	FROM tblListViews t1 WHERE [Type] = 2 AND [TypeIndex] <> 1 AND ID = ''''


INSERT INTO dbo.tblWebLink (WebLinkGuid, LinkName, LinkAddress, LinkDescription, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT newid(), LinkName, LinkAddress, LinkDescription, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy FROM dbo.tblWebLink
WHERE LinkName NOT IN (SELECT LinkName FROM FuelsManagerDB_Template.dbo.tblWebLink)

--
-- Gets all queries that have a reference to aliasindex
--
SELECT querystorageguid, CAST(queryxml AS xml) AS queryXML  
INTO #tmpT
FROM tblquerystorage WHERE queryxml LIKE ''%AliasIndex%''
 
--
-- Switch the ones with -1 to the empty guid
--
UPDATE a
SET queryxml.modify(''insert <TransactionAliasGuids/> into (/FuelsManager.Queries/FuelsManager.Query)[1]'')
FROM #tmpT a
WHERE a.queryXML.value(''(/FuelsManager.Queries/FuelsManager.Query/AliasIndex)[1]'', ''int'') = -1
 
--
-- Update the others from the alias table before migration
--
UPDATE a
SET queryxml.modify(''insert <TransactionAliasGuids><QueryWriterAliasGuid><AliasGuid>{sql:column("aliases.TransactionAliasGuid")}</AliasGuid></QueryWriterAliasGuid></TransactionAliasGuids> 
       into (/FuelsManager.Queries/FuelsManager.Query)[1]'')
FROM #tmpT a
INNER JOIN tblTransactionAliases aliases ON aliases.AliasID = a.queryXML.value(''(/FuelsManager.Queries/FuelsManager.Query/AliasIndex)[1]'', ''int'')
 
--
-- remove the old aliasindex field
--
UPDATE a
SET queryxml.modify(''delete (/FuelsManager.Queries/FuelsManager.Query/AliasIndex)'')
FROM #tmpT a
 
--
-- update the results
--
UPDATE qs SET qs.QueryXML = convert(nvarchar(max), tmp.queryxml)
FROM tblQueryStorage qs INNER JOIN #tmpT tmp ON qs.QueryStorageGuid = tmp.QueryStorageGuid

	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-2 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-2 Rearrange Data', 
		@step_id=15, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'	SET NOCOUNT ON;

/*
	TAS Database Migration To v10.x
	SCRIPT TO Update BSM-E database to replace GUID with the ones knwon by FM Enterprise based on the ID
	Author: Aloisio(Al) dos Santos
*/
--------------------------------- Start of Point Type creation -----------------------------------------------
DECLARE @PointTemplateTypeIndex INT
DECLARE @ApplicationStringGuid UNIQUEIDENTIFIER
DECLARE @PointSiteGuid UNIQUEIDENTIFIER
SET @PointSiteGuid = ''00000000-0000-0000-0000-000000000001''
SELECT @PointTemplateTypeIndex = ApplicationStringTypeIndex FROM lookup.tblApplicationStringType WHERE ApplicationStringTypeCode = ''POINT_TEMPLATE_TYPE''


IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Tank'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''E78CD406-4C19-4978-8940-FA4E404E3E53''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Tank'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''B0E1B642-9C3C-4587-961B-F5505BD1AA65'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Valve'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''E33A769F-3EFC-46C6-A50F-A103454BFE97''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Valve'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''5233FF4B-354E-4B59-9658-C627546B231D'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Pump'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''1135AA41-525B-4024-BF3D-6BF2D55A034B''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Pump'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''B6C238F9-F4BF-4B49-86EE-24BBE22E7722'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Meter'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''9403A36F-33F6-4DCC-857D-F53C8DC66196''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Meter'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''B5BC41D2-E182-47AB-BE82-09E45B98DC4E'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Preset'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''7EA082F3-6FBF-4136-A2D7-8A3670E9A9EF''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Preset'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''0DCC185F-29C0-43E7-A1A3-4B596D908370'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = ''Pipe'') = 0)
BEGIN
	SET @ApplicationStringGuid = ''55F0E8B8-3A74-40D0-8B8C-675A4B6A478C''

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (''Pipe'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES(''E9A87EF0-AF03-46B9-A33C-2815A22426DE'', @ApplicationStringGuid, @PointSiteGuid, ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', ''2012-08-18 09:06:12.0000000 -04:00'', ''administrator'', @PointSiteGuid)
END


PRINT ''INSERT [dbo].[tblAlarmAndEvents] ...''
DELETE [dbo].[tblAlarmAndEvents]

INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Manual Synchronization Complete'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9A7D7144-07FE-4FED-BDD0-7EC43ABFBBA0'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Manual Synchronization Initiated'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''CEE75B37-0E18-4A3E-8F14-133FD658D607'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Periodic Synchronization Complete'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''356DB540-EE86-4395-A395-590925166FA4'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Periodic Synchronization Initiated'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''B744E7E3-67E8-4415-AF1F-754C1B79A0AE'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Stop Synchronization Complete'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''C6BF383D-1C87-4D93-A67B-46831425AFF4'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Stop Synchronization Initiated'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''795842A6-A6B1-47B3-A679-7C4A977CB1DC'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Synchronization Configuration Error'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''A2B69367-C57F-4A3F-9556-6CFB56219954'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Synchronization Conflict(s) Detected'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''5A1FD84B-21E0-4C62-8DF1-F101BE4AC863'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Synchronization Currently Disabled'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''6F6C6B8F-A5C8-4B67-8CBE-BAD60EAFC89B'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Synchronization Error Encountered'' , NULL, NULL, CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2016-05-12T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''07B31A82-B095-4D1F-B4F2-9CB2F65924AC'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Data Transmission Export'', NULL, NULL, CAST(N''2015-06-04T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:04:40.8974146+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''688ee0df-a459-4f85-b1b8-5603aa68e7e3'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Data Transmission Export Re-process'', NULL, NULL, CAST(N''2015-06-04T01:04:45.5849395+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:04:45.5849395+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''371f0642-6c81-406f-abbd-3c3cca98790c'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Data Transmission Import'', NULL, NULL, CAST(N''2015-06-04T01:04:50.3193348+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:04:50.3193348+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''c4c2c7d7-b7c8-4a0c-8e98-2257aae33c42'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Migration Export Guid Mapping Data'', NULL, NULL, CAST(N''2015-06-04T01:05:05.3662114+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:05:05.3662114+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''f640aae7-dc29-4064-9743-d0dd17a2de7b'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Data Synchronization'', 0, N''Migration Export Guid Mapping Error Encountered'', NULL, NULL, CAST(N''2015-06-04T01:05:09.9130960+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:05:09.9130960+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''3939ceca-ef7c-43d1-ad12-376c94ccbdc3'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''System'', 0, N''Personnel Lock Out'', NULL, NULL, CAST(N''2015-06-04T01:06:46.0068887+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:06:46.0068887+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''384abc0b-8687-4a19-8027-a8b691b30bef'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''System'', 0, N''User Logged In'', NULL, NULL, CAST(N''2015-06-04T01:06:50.4756401+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:06:50.4756401+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''fb8cf06c-cd57-41c9-b2d3-cbbbcc3720f5'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''System'', 0, N''User Logged Out'', NULL, NULL, CAST(N''2015-06-04T01:07:02.7100263+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:07:02.7100263+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''198980ff-1fb8-42f3-85f4-da1747cfe181'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''System'', 0, N''User Login Failed. '', NULL, NULL, CAST(N''2015-06-04T01:07:08.2256490+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:07:08.2256490+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''8ee71e75-8acc-4a5f-9fe0-297970c7bac1'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''System'', 0, N''User has no Group Membership.'', NULL, NULL,  CAST(N''2015-10-04T01:07:08.2256490+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-10-04T01:07:08.2256490+00:00'' AS DateTimeOffset), N''Administrator'', 1,''60ABC255-3FEC-4A81-B8A5-0268ABD4AA6E'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 02 Created'', NULL, NULL, CAST(N''2015-06-04T01:07:41.5069169+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:07:41.5069169+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''b428abb8-0999-4e3b-96f1-621cd0496390'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 02 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:07:45.9444178+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:07:45.9444178+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''064667f5-27e7-469f-83eb-d6be06dc7c59'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 02 Updated'', NULL, NULL, CAST(N''2015-06-04T01:07:51.5694273+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:07:51.5694273+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''ca7c5e51-5655-4a4c-8177-4e6fd4a043ca'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 04 Created'', NULL, NULL, CAST(N''2015-06-04T01:08:18.7413103+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:08:18.7413103+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9537e3ec-4c3a-4e61-9555-1218bbdf9b5d'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 04 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:08:24.5069381+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:08:24.5069381+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''00b47740-85bf-4b09-946e-47ba830a91a6'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 04 Updated'', NULL, NULL, CAST(N''2015-06-04T01:08:37.3975739+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:08:37.3975739+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''db2daf79-3bf0-4948-9c52-663ccc942594'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 05 Created'', NULL, NULL, CAST(N''2015-06-04T01:08:49.1944501+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:08:49.1944501+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''4fe597d5-6cab-4790-b06a-7d77266cce89'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 05 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:08:54.9756721+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:08:54.9756721+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''df32657e-ff58-4ca5-8bb4-ee6276c511ef'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 05 Updated'', NULL, NULL, CAST(N''2015-06-04T01:09:00.3663321+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:00.3663321+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''649a2837-8af4-4352-b5bb-805e228eb525'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 06 Created'', NULL, NULL, CAST(N''2015-06-04T01:09:06.5225898+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:06.5225898+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''914a8703-7f94-4be1-bff1-f618d01102d9'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 06 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:09:17.5225899+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:17.5225899+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''a4773996-1aa1-4016-adb8-c92cd4556d6e'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 06 Updated'', NULL, NULL, CAST(N''2015-06-04T01:09:25.4132197+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:25.4132197+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''d8ba4ca8-a05f-4256-ac2e-af0af3f8239b'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 07 Created'', NULL, NULL, CAST(N''2015-06-04T01:09:30.1476024+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:30.1476024+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9ea3917b-22e9-4d6e-a4d5-fa76a524a318'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 07 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:09:34.3507239+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:34.3507239+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''27a3cc01-aaf5-49a5-ae5e-cffd93a0e886'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 07 Updated'', NULL, NULL, CAST(N''2015-06-04T01:09:39.1319839+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:39.1319839+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9bba54ce-b2d9-40a7-a6e0-32814bf55d19'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 08 Created'', NULL, NULL, CAST(N''2015-06-04T01:09:44.4913534+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:44.4913534+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''e4e4c22c-f173-43c8-ad1d-ce392bcc2acc'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 08 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:09:49.6163419+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:09:49.6163419+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''ebc92f8c-c106-4401-a4fe-4ff2d38faba8'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 08 Updated'', NULL, NULL, CAST(N''2015-06-04T01:10:00.5850278+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:10:00.5850278+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9c569352-af48-4e01-964b-74e75e3473ee'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 09 Created'', NULL, NULL, CAST(N''2015-06-04T01:10:05.8506221+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:10:05.8506221+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''bb300099-c78c-44c2-ae75-21f7de90fd8a'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 09 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:10:13.6474509+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:10:13.6474509+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''65c48133-820a-4440-b997-9162bfa6e6be'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 09 Updated'', NULL, NULL, CAST(N''2015-06-04T01:10:35.5848233+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:10:35.5848233+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''240628f1-b31a-47a0-95f4-a7401a58abf3'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 10 Created'', NULL, NULL, CAST(N''2015-06-04T01:10:49.8972379+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:10:49.8972379+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''64a33326-e678-487f-b019-5ba1e2674f1c'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 10 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:11:03.8346566+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:03.8346566+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''823535a6-e31d-4345-9ded-63d6bd756796'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 10 Updated'', NULL, NULL, CAST(N''2015-06-04T01:11:08.5846274+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:08.5846274+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''d3c2a4dd-2295-4a5f-81ff-cbabc9c309d1'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 11 Created'', NULL, NULL, CAST(N''2015-06-04T01:11:14.3814676+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:14.3814676+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''9230f00a-ea0b-4823-af06-be0298d4d056'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 11 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:11:36.7094621+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:36.7094621+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''58846d40-5cb3-44bd-93f2-30bcb411282e'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 11 Updated'', NULL, NULL, CAST(N''2015-06-04T01:11:45.1156672+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:45.1156672+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''ea7afcb3-2e88-476f-97c4-8cc2ecaf7101'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 12 Created'', NULL, NULL, CAST(N''2015-06-04T01:11:50.6781288+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:50.6781288+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''7b14bd95-154b-4c25-aed7-3a0a3258ccb8'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 12 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:11:56.0689384+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:11:56.0689384+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''727c4704-4e05-4a4f-98b9-0b1d6211ce5c'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 12 Updated'', NULL, NULL, CAST(N''2015-06-04T01:12:00.5847348+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:00.5847348+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''8b1aec7e-466f-4e54-bcf2-6a32a02ef8b6'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 14 Created'', NULL, NULL, CAST(N''2015-06-04T01:12:20.6792671+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:20.6792671+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''e8f5a08b-7a2f-4426-8ddb-10aaf239800a'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 14 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:12:25.5232054+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:25.5232054+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''36c548cb-5bb1-4313-baec-2c1066c802d5'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 14 Updated'', NULL, NULL, CAST(N''2015-06-04T01:12:30.8046614+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:30.8046614+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''4177cae1-bbe9-422f-9eb1-b7d8efa2e43c'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 15 Created'', NULL, NULL, CAST(N''2015-06-04T01:12:41.7269629+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:41.7269629+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''ecff8c71-023f-46dd-bb53-a16728062e62'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 15 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:12:46.3833919+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:46.3833919+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''d3d8a723-5c03-433c-aabd-5588e1466589'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 15 Updated'', NULL, NULL, CAST(N''2015-06-04T01:12:56.1493989+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:12:56.1493989+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''e20e579f-b850-4339-9f47-7852d1ea47b6'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 17 Created'', NULL, NULL, CAST(N''2015-06-04T01:13:12.4156328+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:13:12.4156328+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''1bc02897-c366-4642-8b62-7008cb88552f'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 17 Status Changed'', NULL, NULL, CAST(N''2015-06-04T01:13:17.4783604+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:13:17.4783604+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''785d9ed8-30a2-40f4-a1bd-80c85be3cebc'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 17 Updated'', NULL, NULL, CAST(N''2015-06-04T01:13:21.8691481+00:00'' AS DateTimeOffset), N''Administrator'', CAST(N''2015-06-04T01:13:21.8691481+00:00'' AS DateTimeOffset), N''Administrator'', 1, N''77118fd9-273a-4c7d-9a7b-0780d13d011a'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 18 Created'', NULL, NULL, CAST(N''2015-08-04T17:52:54.7001686+00:00'' AS DateTimeOffset), N''SMITH.JAMES.A.1077784329'', CAST(N''2015-08-04T17:52:54.7001686+00:00'' AS DateTimeOffset), N''SMITH.JAMES.A.1077784329'', 1, N''3f047db5-03f7-413e-8c96-ebbfdea486ed'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 01 Created'', NULL, NULL, CAST(N''2015-08-04T17:53:24.3099330+00:00'' AS DateTimeOffset), N''SMITH.JAMES.A.1077784329'', CAST(N''2015-08-04T17:53:24.3099330+00:00'' AS DateTimeOffset), N''SMITH.JAMES.A.1077784329'', 1, N''ef309e8b-883b-489c-be20-998521b5daaa'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 13 Created'', NULL, NULL, CAST(N''2015-08-05T15:52:51.1529114+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:52:51.1529114+00:00'' AS DateTimeOffset), N''administrator'', 1, N''07972f9c-d74e-4bc8-9251-5ebae2eba34c'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 16 Created'', NULL, NULL, CAST(N''2015-08-05T15:54:32.1373588+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:54:32.1373588+00:00'' AS DateTimeOffset), N''administrator'', 1, N''fdcb011f-1b8f-42e5-b929-36d51a002e53'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 19 Created'', NULL, NULL, CAST(N''2015-08-05T15:54:55.8717447+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:54:55.8717447+00:00'' AS DateTimeOffset), N''administrator'', 1, N''2faf556c-59a5-49d9-8d8c-06d2b388f2e6'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 20 Created'', NULL, NULL, CAST(N''2015-08-05T15:55:14.2780044+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:55:14.2780044+00:00'' AS DateTimeOffset), N''administrator'', 1, N''ed4a3a9a-f640-46fa-b6a9-4c01086475c2'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 21 Created'', NULL, NULL, CAST(N''2015-08-05T15:55:36.2623970+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:55:36.2623970+00:00'' AS DateTimeOffset), N''administrator'', 1, N''e0df3e9b-77d9-485c-aa97-6fc6d482434d'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 22 Created'', NULL, NULL, CAST(N''2015-08-05T15:56:31.5280166+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:56:31.5280166+00:00'' AS DateTimeOffset), N''administrator'', 1, N''c54876fd-b6f4-4385-a491-40e5546e4cbe'', -1, NULL, NULL)
INSERT INTO [dbo].[tblAlarmAndEvents] ([Source], [Alarm], [ID], [CategoryIndex], [PriorityIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Enabled], [AlarmAndEventGuid], [SiteIndex], [CategoryGuid], [PriorityGuid]) VALUES (N''Transactions'', 0, N''Transaction Type 23 Created'', NULL, NULL, CAST(N''2015-08-05T15:57:02.4343140+00:00'' AS DateTimeOffset), N''administrator'', CAST(N''2015-08-05T15:57:02.4343140+00:00'' AS DateTimeOffset), N''administrator'', 1, N''3c6b04ac-83ab-471a-b596-4dca58c84cfd'', -1, NULL, NULL)


	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-3 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-3 Rearrange Data', 
		@step_id=16, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'	SET NOCOUNT ON;
/*
	TAS Database Migration To v10.x
	SCRIPT TO Initialize dbo.tblHelpMapping -- This should be done by the lastest FuelsManager Installation Package to make sure it contains the most recent help mappings.
*/
	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-4 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-4 Rearrange Data', 
		@step_id=17, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'	SET NOCOUNT ON;
/*
	TAS Database Migration To v10.x
	SCRIPT TO Initialize dbo.tblHelpMapping -- This should be done by the lastest FuelsManager Installation Package to make sure it contains the most recent help mappings.
*/
	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-5 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-5 Rearrange Data', 
		@step_id=18, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'	SET NOCOUNT ON;
/*
	TAS Database Migration To v10.x
	SCRIPT TO Initialize dbo.tblHelpMapping -- This should be done by the lastest FuelsManager Installation Package to make sure it contains the most recent help mappings.
*/
	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00060-6 Rearrange Data]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00060-6 Rearrange Data', 
		@step_id=19, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'	SET NOCOUNT ON;
/*
	TAS Database Migration To v10.x
	SCRIPT TO Initialize dbo.tblHelpMapping -- This should be done by the lastest FuelsManager Installation Package to make sure it contains the most recent help mappings.
*/
	PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00069 Prepare Table for script 00070 Update Parent GUID]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00069 Prepare Table for script 00070 Update Parent GUID', 
		@step_id=20, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
Notes:
1.	tblAllocationLineItems.AssignedIndex depends on the Type column:
	> When Type is 0(zero) then it fills the AssignedProductGuid column with tblProducts.ProductGuid
	> When type is 1(one) then it fills out the AssignedProductGroupdGuid column with tblApplicationString.ApplicationStringGuid
	
2. tblAllocationLineItems.AssignedIndex
3. tblAllocations.CompanyMapIndex
4. tblAppointments.AssociatedTypeIndex
5. tblAppointments.TestSetIndex
6. tblChangesQueue.RecordIndex (CHANGES TO RecordGuid BUT IT IS NOT A FK >>> ?????
7. IATAIndex ON tblSites should be addressed on tblAncilaryData
8. Table tblTurnoverPeriod has been dropped
9. Table tblUserDataFields go split


*/

SET NOCOUNT ON;


IF EXISTS (SELECT 1 FROM sys.tables WHERE name= ''_CirrusUpgColumnConvert'')
	DROP TABLE _CirrusUpgColumnConvert
GO


CREATE TABLE _CirrusUpgColumnConvert(
	RowNumber INT IDENTITY PRIMARY KEY
,	SchemaName NVARCHAR(50)
,	TableName NVARCHAR(300)
,	OriginalColumn NVARCHAR(500)
,	ConvertToColumn NVARCHAR(500)
,	ParentSchema NVARCHAR(200) --DEFAULT(''dbo'')
,	ParentTable NVARCHAR(500)
,	ParentNewColumn NVARCHAR(500)
,	ParentOldColumn NVARCHAR(500)
,	RenameColumnTo NVARCHAR(500)
)
GO


INSERT INTO _CirrusUpgColumnConvert(
	SchemaName,TableName,OriginalColumn
	)
SELECT	c1.Table_Schema AS SchemaName
	,	c1.Table_Name AS TableName
	,	c1.Column_Name AS ColumnNameIndex
FROM INFORMATION_SCHEMA.COLUMNS c1
WHERE Column_Name LIKE ''%INDEX%''
AND c1.Table_Schema = ''dbo''
AND LEFT(c1.TABLE_NAME,3) = ''tbl''
AND RIGHT(c1.Table_Name,3) <> ''Map''
AND c1.Table_Name NOT IN (
	''tblProductMap_BackupCKViolators'',''tblCompanyMap_BackupCKViolators'',''tblQualificationsMap_BackupCKViolators'',
	''tblPIDXProfileCompanyMap_BackupFKViolators'',''tblAppointments'',''tblArchivedUsers'',''tblTurnoverPeriod'',
	''tblUserDataFields'',''tblTurnoverPeriods'', ''tblProductValuesBySite'', ''tblReportApprovals'', 
	''tblExportResults'', ''tblExportResultDetails'', ''tblTransactionUserData'')
/*
Column exception notes:
- AdjustmentTransactionAliasGuid column has relocate from tblSites to dbo.tblSitesAncillaryData tables
- InventoryTransactionAliasGuid column has relocate from tblSites to dbo.tblSitesAncillaryData tables
- IATAGuid column has relocate from tblSites to dbo.tblSitesAncillaryData tables

*/
AND c1.Column_Name NOT IN (
	''AdditiveProfileCycleAmountUnitIndex'',''AdditiveProfileRateUnitIndex'',
	''AdditiveVolumeUnitIndex'',''AdjustmentTransactionAliasIndex'',''AdditiveProfileRateUnitIndex'',
	''AdditiveVolumeUnitIndex'',''InventoryTransactionAliasIndex'',''IATAIndex''
	)
AND NOT (c1.TABLE_SCHEMA = ''dbo'' and c1.Table_Name  in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
ORDER BY Table_Schema,Table_Name,COLUMN_NAME


-- Site
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''SiteGuid''
	,	ParentTable=''tblSites''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''SiteGuid''
	,	ParentOldColumn =''SiteIndex''
WHERE OriginalColumn=''SiteIndex''


-- Product
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''ProductGuid''
	,	ParentTable=''tblProducts''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ProductGuid''
	,	ParentOldColumn =''ProductIndex''
WHERE OriginalColumn=''ProductIndex''

-- Company
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CompanyGuid''
	,	ParentTable=''tblCompanies''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''CompanyGuid''
	,	ParentOldColumn =''CompanyIndex''
WHERE OriginalColumn=''CompanyIndex''

UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''ShipperTypeApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''ShipperTypeIndex''

UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CustomerBillToTypeApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''CustomerBillToTypeIndex''

UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CustomerShipToTypeApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''CustomerShipToTypeIndex''

-- Equipment
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''EquipmentGuid''
	,	ParentTable=''tblEquipment''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''EquipmentGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''Index''

-- FuelCard
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''FuelCardGuid''
	,	ParentTable=''tblFuelCards''
	,	ParentSchema=''dbo''
	,	ParentNewColumn =''FuelCardGuid''
	,	ParentOldColumn =''FuelCardIndex''
WHERE OriginalColumn=''FuelCardIndex''


-- Additive Profile
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''AdditiveProfileGuid''
	,	ParentTable=''tblAdditiveProfiles''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''AdditiveProfileGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''AdditiveProfileIndex''


-- IATAIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''IATAGuid''
	,	ParentTable=''tblIATA''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''IATAGuid''
	,	ParentOldColumn =''IATAIndex''
	
WHERE OriginalColumn=''IATAIndex''
AND TableName NOT IN (''tblIATA'',''tblSites'')


-- AliasIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''TransactionAliasGuid''
	,	ParentTable=''tblTransactionAliases''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''TransactionAliasGuid''
	,	ParentOldColumn =''AliasID''
WHERE OriginalColumn=''AliasIndex''


-- MaintenanceReasonIndex MaintenanceReasonGuid
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''MaintenanceReasonGuid''
	,	ParentTable=''tblMaintenanceReasons''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''MaintenanceReasonGuid''
	,	ParentOldColumn =''MaintenanceReasonIndex''
WHERE OriginalColumn=''MaintenanceReasonIndex''


-- VesselTypeIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''LookupVesselTypeIndex''
WHERE OriginalColumn=''MaintenanceReasonIndex''


-- VolumeUnitIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''VolumeUnitIndex''
WHERE OriginalColumn=''VolumeUnitIndex''


-- VesselTypeIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''LookupVesselTypeIndex''
WHERE OriginalColumn=''VesselTypeIndex''


-- UserIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''UserGuid''
	,	ParentTable=''tblUsers''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''UserGuid''
	,	ParentOldColumn =''UserIndex''
WHERE OriginalColumn=''UserIndex''
AND TableName <> ''tblUsers''


-- OwnerIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''OwnerCompanyGuid''
	,	ParentTable=''tblCompanies''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''CompanyGuid''
	,	ParentOldColumn =''CompanyIndex''
WHERE OriginalColumn=''OwnerIndex''
AND TableName <> ''tblQueryStorage''


UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''OwnerUserGuid''
	,	ParentTable=''tblUsers''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''UserGuid''
	,	ParentOldColumn =''CompanyIndex''
WHERE OriginalColumn=''UserIndex''
AND TableName = ''tblQueryStorage''


-- BillToIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''BillToCompanyGuid''
	,	ParentTable=''tblCompanies''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''CompanyGuid''
	,	ParentOldColumn =''CompanyIndex''
WHERE OriginalColumn=''BillToIndex''

-- ManagerIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''ManagerCompanyGuid''
	,	ParentTable=''tblCompanies''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''CompanyGuid''
	,	ParentOldColumn =''CompanyIndex''
WHERE OriginalColumn=''ManagerIndex''

-- DensityUnitIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''DensityUnitIndex''
WHERE OriginalColumn=''DensityUnitIndex''

-- CurrencyIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''CurrencyIndex''
WHERE OriginalColumn=''CurrencyIndex''

-- LevelUnitIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''LevelUnitIndex''
WHERE OriginalColumn=''LevelUnitIndex''

-- MassUnitIndex
UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=''MassUnitIndex''
WHERE OriginalColumn=''MassUnitIndex''

-- BayAStationIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=LEFT(OriginalColumn,4)+''StationGuid''
	,	ParentTable=''tblStations''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''StationGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn IN(''BayAStationIndex'',''BayBStationIndex'')

-- TransIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''TransactionGuid''
	,	ParentTable=''tblTransactions''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''TransactionGuid''
	,	ParentOldColumn =''TransIndex''
WHERE OriginalColumn=''TransIndex''
AND TableName <> ''tblTransactions''

-- DestinationEquipmentIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''DestinationEquipmentGuid''
	,	ParentTable=''tblEquipment''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''EquipmentGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''DestinationEquipmentIndex''

-- DestinationEquipmentIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''Destination''+RIGHT(OriginalColumn,1)+''EquipmentGuid''
	,	ParentTable=''tblEquipment''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''EquipmentGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn IN (
''DestinationEquipmentIndex1'',''DestinationEquipmentIndex2'',''DestinationEquipmentIndex3'')


UPDATE _CirrusUpgColumnConvert
SET		RenameColumnTo=OriginalColumn
WHERE OriginalColumn IN (''TemperatureUnitIndex'',''PressureUnitIndex'',''FlowUnitIndex'')

-- AllocationGroupIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''AllocationGroupApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''AllocationGroupIndex''

-- UserGroupIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''UserGroupGuid''
	,	ParentTable=''tblGroups''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''GroupGuid''
	,	ParentOldColumn =''GroupIndex''
WHERE OriginalColumn=''UserGroupIndex''

-- AllocationIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''AllocationGuid''
	,	ParentTable=''tblAllocations''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''AllocationGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''AllocationIndex''

-- AssignedEquipmentIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''AssignedEquipmentGuid''
	,	ParentTable=''tblEquipment''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''EquipmentGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''AssignedEquipmentIndex''

-- CategoryIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CategoryGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''CategoryIndex''

--PriorityIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''PriorityGuid''
	,	ParentTable=''tblAlarmPriorities''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''AlarmPriorityGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''PriorityIndex''

-- InvoiceTransIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''InvoiceTransactionGuid''
	,	ParentTable=''tblTransactions''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''TransactionGuid''
	,	ParentOldColumn =''TransIndex''
WHERE OriginalColumn=''InvoiceTransIndex''

-- CustomerBillToTypeIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CustomerBillToTypeApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''CustomerBillToTypeIndex''

-- CustomerShipToTypeIndex
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''CustomerShipToTypeApplicationStringGuid''
	,	ParentTable=''tblApplicationString''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''ApplicationStringGuid''
	,	ParentOldColumn =''Index''
WHERE OriginalColumn=''CustomerShipToTypeIndex''

-- AssociatedTankIndex 
UPDATE _CirrusUpgColumnConvert
SET		ConvertToColumn=''TankGuid''
	,	ParentTable=''tblTanks''
	,	ParentSchema=''dbo''
	,	ParentNewColumn=''TankGuid''
	,	ParentOldColumn =''TankIndex''
WHERE OriginalColumn=''AssociatedTankIndex''


	PRINT ''Completed successfully''
GO', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00070 Update Tables With Parent GUIDS based on legacy Index]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00070 Update Tables With Parent GUIDS based on legacy Index', 
		@step_id=21, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
SET NOCOUNT ON;

DECLARE @SchemaName NVARCHAR(200)
	,	@TableName NVARCHAR(200)
	,	@OriginalColumn NVARCHAR(300)
	,	@ConvertToColumn NVARCHAR(300)
	,	@ParentSchema NVARCHAR(200)
	,	@ParentTable NVARCHAR(300)
	,	@ParentNewColumn NVARCHAR(300)
	,	@ParentOldColumn NVARCHAR(300)
	,	@RenameColumnTo NVARCHAR(300)
	,	@Sql NVARCHAR(MAX)
	,	@ProcessStartTime DATETIME2
	,	@StepStartTime DATETIME2
	,	@StepEndTime DATETIME2
	,	@AffectedRecords INT
	,	@StepAffectedRecords INT


IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name= ''_CirrusUpgColumnConvert'')
BEGIN
	declare  @msg nvarchar(512) = ''TABLE _CirrusUpgColumnConvert does not exists.''
	raiserror(@msg, 15,-1)
	RETURN
END
	
SET @AffectedRecords = 0
SET @ProcessStartTime = GETDATE()
PRINT ''*** Process Started.''
PRINT ''*** Process started on ''+CAST(GETDATE() AS NVARCHAR(50))

PRINT ''Gathering database metadata...''
DECLARE UpgCursor CURSOR FOR
	SELECT	SchemaName
		,	TableName
		,	OriginalColumn
		,	ConvertToColumn
		,	ParentSchema
		,	ParentTable
		,	ParentNewColumn
		,	ParentOldColumn
		,	RenameColumnTo
	FROM _CirrusUpgColumnConvert
	WHERE TableName <> ParentTable
	AND ParentTable IS NOT NULL
	AND TableName NOT IN (''tblObfuscateTransactionsTemp'',''tblBulkPaymentLinks'')
	ORDER BY TableName

OPEN UpgCursor
	FETCH NEXT FROM UpgCursor INTO
			@SchemaName,@TableName,@OriginalColumn,@ConvertToColumn,@ParentSchema,
			@ParentTable,@ParentNewColumn,@ParentOldColumn,@RenameColumnTo
WHILE @@FETCH_STATUS=0
BEGIN

	SET @Sql = ''IF NOT EXISTS (SELECT  TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS c1 WHERE table_schema=''''''+@schemaname+'''''' AND table_name=''''''+@tablename+'''''' AND c1.Column_Name=''''''+@ConvertToColumn+'''''') 
				ALTER TABLE [''+@schemaname+''].[''+@tablename+''] ADD  [''+@ConvertToColumn+''] uniqueidentifier''

	print @sql
	EXEC sp_executesql @statment=@Sql


	SET @StepStartTime = GETDATE()
	SET @Sql = ''UPDATE tb1 ''
	SET @Sql+= ''SET tb1.[''+@ConvertToColumn+'']=tb2.[''+@ParentNewColumn+''] ''
	SET	@Sql+= ''FROM [''+@SchemaName+''].[''+@TableName+''] tb1 ''
	SET @Sql+= ''INNER JOIN [''+@ParentSchema+''].[''+@ParentTable+''] tb2 ON tb2.[''+@ParentOldColumn+'']=tb1.[''+@OriginalColumn+''] ''
	SET @Sql+= ''WHERE tb1.[''+@ConvertToColumn+''] IS NULL;''
	PRINT ''''
	PRINT @Sql
	PRINT ''***>>> Step started at ''+CAST(@StepStartTime AS NVARCHAR(50))
	EXEC sp_executesql @Statement=@Sql


	SET @StepAffectedRecords = @@ROWCOUNT
	SET @AffectedRecords = @AffectedRecords + @StepAffectedRecords
	SET @StepEndTime=GETDATE()
	PRINT ''***>>> Step finished on ''+CAST(@StepEndTime AS NVARCHAR(50))
	PRINT ''***>>> Affected Records: ''+CAST (@StepAffectedRecords AS NVARCHAR(50))
	PRINT ''***>>> Step elapse time in seconds: ''+ CAST(DATEDIFF(ss,@StepStartTime,@StepEndTime) AS NVARCHAR(50))
	PRINT ''***>>> Step elapse time in minutes: ''+ CAST(DATEDIFF(mi,@StepStartTime,@StepEndTime) AS NVARCHAR(50))

	
	FETCH NEXT FROM UpgCursor INTO
			@SchemaName,@TableName,@OriginalColumn,@ConvertToColumn,@ParentSchema,
			@ParentTable,@ParentNewColumn,@ParentOldColumn,@RenameColumnTo

END

PRINT ''*** Process finished on ''+CAST(@StepEndTime AS NVARCHAR(50))
PRINT ''*** Total Number of Affected Records: '' + CAST(@AffectedRecords AS NVARCHAR(50))
PRINT ''*** Process elapse time in minutes: ''+ CAST(DATEDIFF(mi,@ProcessStartTime,@StepEndTime) AS NVARCHAR(50))
PRINT ''*** Process Complete.''
CLOSE UpgCursor
DEALLOCATE UpgCursor

IF EXISTS(SELECT 1 FROM sys.tables WHERE name=''_CirrusUpgColumnConvert'')
	DROP TABLE _CirrusUpgColumnConvert


-----------------------------------------------------------------------------------------------------------------------------
-- SOME COLUMNS HAVE BEEN MOVED TO A COLUMN NAME WITH A DIFFERENT NAME, COPY THE OLD VALUES TO THE NEW COLUMNS
-----------------------------------------------------------------------------------------------------------------------------

-- Migrate PIDXProductCode to new column PIDXCode in dbo.tblProducts
UPDATE dbo.tblProducts SET PIDXCode = PIDXProductCode;

-- Migrate old columns in AllocationLineItems to their new column
UPDATE dbo.tblAllocationLineItems SET LookupAllocationTypeIndex = [Type];
UPDATE dbo.tblAllocationLineItems SET LookupResetMethodIndex = ResetPeriod;
UPDATE dbo.tblAllocationLineItems SET LookupResetPeriodIndex = ResetMethod;

-----------------------------------------------------------------------------------------------------------------------------
-- TABLES WITH A PARENT/CHILD REFERENCE COLUMN NEED TO HAVE THEIR LINKING COLUMNS RESOLVED HERE
-----------------------------------------------------------------------------------------------------------------------------

UPDATE tb1
SET tb1.ParentEquipmentGuid=tb2.EquipmentGuid
FROM tblEquipment tb1
INNER JOIN tblEquipment tb2 on tb2.[Index]=tb1.EquipmentIndex

UPDATE tb1
SET tb1.EquipmentTypeGuid=tb2.EquipmentTypeGuid
FROM tblEquipment tb1
INNER JOIN tblEquipmentTypes tb2 on tb2.EqTypeIndex=tb1.EqTypeIndex

UPDATE tb1
SET tb1.[OperatorPersonnelGuid]=tb2.PersonnelGuid
FROM [tblEquipmentMaintenanceLog] tb1
INNER JOIN tblPersonnel tb2 on tb2.PersonIndex=tb1.OperatorIndex

UPDATE tb1
SET tb1.[OperatorPersonnelGuid]=tb2.PersonnelGuid
FROM [tblTankMaintenanceLog] tb1
INNER JOIN tblPersonnel tb2 on tb2.PersonIndex=tb1.OperatorIndex

UPDATE tb1
SET tb1.ShipToCompanyGuid=tb2.CompanyGuid
FROM tblFuelCards tb1
INNER JOIN tblCompanies tb2 on tb2.CompanyIndex=tb1.ShipToIndex

UPDATE tb1
SET tb1.ShipperCompanyGuid=tb2.CompanyGuid
FROM tblFuelCards tb1
INNER JOIN tblCompanies tb2 on tb2.CompanyIndex=tb1.ShipperIndex

UPDATE tb1
SET tb1.SupervisorPersonnelGuid=tb2.PersonnelGuid
FROM tblPersonnel tb1
INNER JOIN tblPersonnel tb2 on tb2.PersonIndex=tb1.SupervisorIndex

UPDATE tb1
SET tb1.OwnerUserGuid=tb2.UserGuid
FROM tblQueryStorage tb1
INNER JOIN tblUsers tb2 on tb2.UserIndex=tb1.OwnerIndex

UPDATE tb1
SET tb1.UserGuid=tb2.UserGuid
FROM tblSavedQueries tb1
INNER JOIN tblUsers tb2 on tb2.UserIndex=tb1.UserIndex

UPDATE tb1
SET tb1.ReportGroupGuid=tb2.ReportGroupGuid
FROM tblReportDetails tb1
INNER JOIN tblReportGroups tb2 on tb2.GroupIndex=tb1.GroupIndex

UPDATE tb1
SET tb1.[LoginSiteGuid]=tb2.SiteGuid
FROM [tblSessions] tb1
INNER JOIN tblSites tb2 on tb2.SiteIndex=tb1.LoginSiteIndex

UPDATE tb1 
SET [LookupEquipmentTypeIndex]=Attribute
FROM [tblEquipmentTypes] tb1

UPDATE tb1 
SET [LookupProductTypeIndex]=ProductType
FROM [tblProducts] tb1

UPDATE tb1 
SET [LookupNumberGroupSizesTypeIndex]=NumberGroupSizesType
--,	[LookupQuantityDisplayDefaultIndex]=QuantityDisplayDefault -- Only valid for Defense
,	[LookupSecondaryStorageFillMethodIndex]=SecondaryStorageFillMethod
,	[LookupMailConnectModeIndex]=MailConnectMode
,	[LookupWatchdogModeIndex]=WatchdogMode
FROM [tblSites] tb1

UPDATE tb1 
SET [LookupApplicationStringTypeIndex]=[Type]
FROM [tblApplicationString] tb1

UPDATE tb1 
SET [LookupVesselTypeIndex]=[VesselTypeIndex]
FROM [tblTankQualityTagLog] tb1

UPDATE tb1 
SET [LookupVesselTypeIndex]=[VesselTypeIndex]
FROM [tblTanks] tb1

UPDATE tb1
SET tb1.[PersonnelGuid]=tb2.PersonnelGuid
FROM [dbo].[tblMessageLog] tb1
INNER JOIN [dbo].[tblPersonnel] tb2 on tb2.PersonIndex=tb1.PersonIndex

UPDATE tb1
SET tb1.[CompanyGuid]=tb2.CompanyGuid
FROM [dbo].[tblMessageLog] tb1
INNER JOIN [dbo].[tblCompanies] tb2 on tb2.CompanyIndex=tb1.CompanyIndex

UPDATE tb1
SET tb1.[MessageGuid]=tb2.MessageGuid
FROM [dbo].[tblMessageLog] tb1
INNER JOIN [dbo].[tblMessages] tb2 on tb2.[Index]=tb1.MessageIndex

UPDATE tb1
SET tb1.[PersonnelGuid]=tb2.PersonnelGuid
FROM [dbo].[tblMessages] tb1
INNER JOIN [dbo].[tblPersonnel] tb2 on tb2.PersonIndex=tb1.PersonIndex

UPDATE tb1
SET tb1.[LookupFrequencyTypeIndex]=tb2.MessageFrequencyTypeIndex
FROM [dbo].[tblMessages] tb1
INNER JOIN [lookup].[tblMessageFrequencyType] tb2 on tb2.MessageFrequencyTypeIndex=tb1.FrequencyType

UPDATE tb1
SET tb1.[LookupLocationTypeIndex]=tb2.MessageLocationTypeIndex
FROM [dbo].[tblMessages] tb1
INNER JOIN [lookup].[tblMessageLocationType] tb2 on tb2.MessageLocationTypeIndex=tb1.LocationType

UPDATE tb1
SET tb1.AssignedProductGuid = tb2.ProductGuid
FROM [dbo].[tblAllocationLineItems] tb1
INNER JOIN [dbo].[tblProducts] tb2 ON tb2.ProductIndex=tb1.AssignedIndex
WHERE tb1.LookupAllocationTypeIndex=0

UPDATE tb1
SET tb1.AssignedApplicationStringGuid = tb2.ApplicationStringGuid
FROM [dbo].[tblAllocationLineItems] tb1
INNER JOIN [dbo].[tblApplicationString] tb2 ON tb2.[Index]=tb1.AssignedIndex
WHERE tb1.LookupAllocationTypeIndex=1

-----------------------------------------------------------------------------------------------------------------------------
-- MISSING REFERENCES THAT PREVENT THE RECORD FROM BEING MIGRATED DUE TO FOREIGN KEY CONSTRAINTS IN NEW DATABASE
-----------------------------------------------------------------------------------------------------------------------------

-- MassQuantity does not appear to be a valid column anymore for the List Views.
PRINT ''Removing MASSQUANTITY from defined List/Ledger Views.''
DELETE FROM [dbo].[tblListViewFields] WHERE TypeIndex = 141

-- REMOVE ANY MessageLog Entries associated with a PersonnelIndex (PersonnelGuid would be NULL) that no longer exists
DELETE FROM [dbo].[tblMessageLog] WHERE PersonnelGuid IS NULL

-- REMOVE ANY MessageLog Entries associated with a CompanyIndex (CompanyGuid would be NULL) that no longer exists
DELETE FROM [dbo].[tblMessageLog] WHERE CompanyGuid IS NULL

-- REMOVE ANY MessageLog Entries associated with a MessageIndex (MessageGuid would be NULL) that no longer exists
DELETE FROM [dbo].[tblMessageLog] WHERE CompanyGuid IS NULL


PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-1 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-1 Update Map and Split Tables', 
		@step_id=22, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	UPGRADE Entity To Site Map
	Source Script: 8.0.5.0-011 WI-24688 DB Revision - Create And Populate Entity To Site Map Split Tables
*/

/*
	Several of the tables populated during this step have an AssignedFromSiteGuid column that IS NOT NULLable.
	A temporary GUID of ''00000000-0000-0000-0000-000000000000'' is used until a final section on the script can perform an update once all split and mapping tables are in place
*/

--
-- Populate map.tblEntityAdditiveProfileToSite
--
PRINT ''Populating map.tblEntityAdditiveProfileToSite...''
TRUNCATE TABLE map.tblEntityAdditiveProfileToSite

INSERT INTO map.tblEntityAdditiveProfileToSite(AdditiveProfileGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.AdditiveProfileGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblAdditiveProfiles a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Additive Profiles'' 

--
-- Populating map.tblEntityAlarmAndEventCategoryToSite
--
PRINT ''Populating map.tblEntityAlarmAndEventCategoryToSite...''
TRUNCATE TABLE map.tblEntityAlarmAndEventCategoryToSite

INSERT INTO map.tblEntityAlarmAndEventCategoryToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Alarm & Event Categories'' 

--
-- Populating map.tblEntityAlarmAndEventToSite
--
PRINT ''Populating map.tblEntityAlarmAndEventToSite...''
TRUNCATE TABLE map.tblEntityAlarmAndEventToSite

INSERT INTO map.tblEntityAlarmAndEventToSite(OwnerSiteGuid,MapToSiteGuid,AssignedFromSiteGuid) 
SELECT	a.SiteGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER JOIN tblSites a on m.[Index] = a.SiteIndex
WHERE	m.TypeID = ''Alarm And Events'' 

--
-- Updating map.tblEntityAppointmentEquipmentToSite
--
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentEquipmentToSite] a
INNER JOIN tblAppointmentEquipment b
ON b.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid
WHERE a.AssignedFromSiteGuid IS NULL

--
-- Updating map.tblEntityAppointmentTankToSite
--
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentTankToSite] a
INNER JOIN tblAppointmentTank b
ON b.AppointmentTankGuid = a.AppointmentTankGuid
WHERE a.AssignedFromSiteGuid IS NULL

--
-- Populating map.tblEntityCompanyToSite
--
PRINT ''Populating map.tblEntityCompanyToSite...''
TRUNCATE TABLE map.tblEntityCompanyToSite

INSERT INTO map.tblEntityCompanyToSite(CompanyGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.CompanyGuid,s.SiteGuid,a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblCompanies a ON a.[CompanyIndex] = m.[Index] 
WHERE	m.TypeID = ''Companies'' 

--
-- Populating map.tblEntityCompanyGroupToSite
--
PRINT ''Populating map.tblEntityCompanyGroupToSite...''
TRUNCATE TABLE map.tblEntityCompanyGroupToSite

INSERT INTO map.tblEntityCompanyGroupToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Company Groups'' and a.[Type]=8

--
-- Create default All Companies for SiteAdmin SiteGroup
--
IF NOT EXISTS (SELECT TOP 1 1 FROM map.tblEntityCompanyGroupToSite WHERE ApplicationStringGuid = ''b2d8440b-0629-4de9-8cbb-f8dad5943116'')
BEGIN
	INSERT INTO map.tblEntityCompanyGroupToSite(CompanyGroupToSiteGuid, ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
	VALUES (''f0d5a187-5e28-416b-9e74-f43ddcd9be2b'', ''b2d8440b-0629-4de9-8cbb-f8dad5943116'', ''00000000-0000-0000-0000-000000000001'', ''00000000-0000-0000-0000-000000000001'')
END

--
-- Populating map.tblEntityCompanyTypeToSite
--
PRINT ''Populating map.tblEntityCompanyTypeToSite...''
TRUNCATE TABLE map.tblEntityCompanyTypeToSite

INSERT INTO map.tblEntityCompanyTypeToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Company Types'' and a.[Type]=4

--
-- Populating map.tblEntityMaintenanceReasonToSite
--
PRINT ''Populating map.tblEntityMaintenanceReasonToSite...''
TRUNCATE TABLE map.tblEntityMaintenanceReasonToSite

INSERT INTO map.tblEntityMaintenanceReasonToSite(MaintenanceReasonGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.MaintenanceReasonGuid,s.SiteGuid,a.SiteGuid
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblMaintenanceReasons a ON a.[MaintenanceReasonIndex] = m.[Index] 
WHERE	m.TypeID = ''Maintenance Reasons'' 

--
-- Populating map.tblEntityPersonnelToSite
--
PRINT ''Populating map.tblEntityPersonnelToSite...''
TRUNCATE TABLE map.tblEntityPersonnelToSite

INSERT INTO map.tblEntityPersonnelToSite(PersonnelGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.PersonnelGuid,s.SiteGuid,a.SiteGuid
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblPersonnel a ON a.[PersonIndex] = m.[Index] 
WHERE	m.TypeID = ''Personnel'' 

--
-- Populating map.tblEntityEquipmentToSite
--
PRINT ''Populating map.tblEntityEquipmentToSite...''
TRUNCATE TABLE map.tblEntityEquipmentToSite

INSERT INTO map.tblEntityEquipmentToSite(EquipmentGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.EquipmentGuid,s.SiteGuid,a.SiteGuid
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblEquipment a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Equipment'' 

-- Create mapping records for those compartments in 7.5 which did not have
--  tblEntityToSiteMap entries.  
INSERT INTO map.tblEntityEquipmentToSite(EquipmentGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.EquipmentGuid,a.SiteGuid,a.SiteGuid
FROM tblEquipment a
LEFT JOIN tblEntityToSiteMap m on a.[Index] = m.[Index]
WHERE	m.TypeID IS NULL 

--
-- Populating map.tblEntityEquipmentTypeToSite
--
PRINT ''Populating map.tblEntityEquipmentTypeToSite...''
TRUNCATE TABLE map.tblEntityEquipmentTypeToSite

INSERT INTO map.tblEntityEquipmentTypeToSite(EquipmentTypeGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.EquipmentTypeGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblEquipmentTypes a ON a.[EqTypeIndex] = m.[Index] 
WHERE	m.TypeID = ''Equipment Type'' 

--
-- Populating map.tblEntityAlarmPriorityToSite
--
PRINT ''Populating map.tblEntityAlarmPriorityToSite...''
TRUNCATE TABLE map.tblEntityAlarmPriorityToSite

INSERT INTO map.tblEntityAlarmPriorityToSite(AlarmPriorityGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.AlarmPriorityGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblAlarmPriorities a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Alarm Priorities'' 

--
-- Populating map.tblEntityAllocationGroupToSite
--
PRINT ''Populating map.tblEntityAllocationGroupToSite...''
TRUNCATE TABLE map.tblEntityAllocationGroupToSite

INSERT INTO map.tblEntityAllocationGroupToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Allocation Groups'' 

--
-- Populating map.tblEntityCompanyCertificateAndPermitToSite
--
PRINT ''Populating map.tblEntityCompanyCertificateAndPermitToSite...''
TRUNCATE TABLE map.tblEntityCompanyCertificateAndPermitToSite

INSERT INTO map.tblEntityCompanyCertificateAndPermitToSite(QualificationGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid, a.SiteGuid  
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Company Certificates and Permits'' 

--
-- Populating map.tblEntityCompanyTypeToSite
--
PRINT ''Populating map.tblEntityEquipmentTagAndLicenseToSite...''
TRUNCATE TABLE map.tblEntityEquipmentTagAndLicenseToSite

INSERT INTO map.tblEntityEquipmentTagAndLicenseToSite(QualificationGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Equipment Tags and Licenses'' 

--
-- Populating map.tblEntityEquipmentTestAndInspectionToSite
--
PRINT ''Populating map.tblEntityEquipmentTestAndInspectionToSite...''
TRUNCATE TABLE map.tblEntityEquipmentTestAndInspectionToSite

INSERT INTO map.tblEntityEquipmentTestAndInspectionToSite(QualificationGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Equipment Tests and Inspections'' 

--
-- Populating map.tblEntityPersonnelLicenseToSite
--
PRINT ''Populating map.tblEntityPersonnelLicenseToSite...''
TRUNCATE TABLE map.tblEntityPersonnelLicenseToSite

INSERT INTO map.tblEntityPersonnelLicenseToSite(QualificationGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Personnel Licenses'' 

--
-- Populating map.tblEntityPersonnelQualificationToSite
--
PRINT ''Populating map.tblEntityPersonnelQualificationToSite...''
TRUNCATE TABLE map.tblEntityPersonnelQualificationToSite

INSERT INTO map.tblEntityPersonnelQualificationToSite(QualificationGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Personnel Qualifications''

--
-- Populating map.tblEntityDotHazardousMessagesToSite
--
PRINT ''Populating map.tblEntityDotHazardousMessagesToSite...''
TRUNCATE TABLE map.tblEntityDotHazardousMessagesToSite

INSERT INTO map.tblEntityDotHazardousMessagesToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''DOT Hazardous Messages'' 

--
-- Populating map.tblEntityEntryMessageToSite
--
PRINT ''Populating map.tblEntityEntryMessageToSite...''
TRUNCATE TABLE map.tblEntityEntryMessageToSite

INSERT INTO map.tblEntityEntryMessageToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Entry Message'' 

--
-- Populating map.tblEntityExitMessageToSite
--
PRINT ''Populating map.tblEntityExitMessageToSite...''
TRUNCATE TABLE map.tblEntityExitMessageToSite

INSERT INTO map.tblEntityExitMessageToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Exit Message'' 

--
-- Populating map.tblEntityProductMessageToSite
--
PRINT ''Populating map.tblEntityProductMessageToSite...''
TRUNCATE TABLE map.tblEntityProductMessageToSite

INSERT INTO map.tblEntityProductMessageToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Product Messages'' 

--
-- Populating map.tblEntityEmailAddressToSite
--
PRINT ''Populating map.tblEntityEmailAddressToSite...''
TRUNCATE TABLE map.tblEntityEmailAddressToSite

INSERT INTO map.tblEntityEmailAddressToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''E-mail Address'' 

--
-- Populating map.tblEntityFootNoteToSite
--
PRINT ''Populating map.tblEntityFootNoteToSite...''
TRUNCATE TABLE map.tblEntityFootNoteToSite

INSERT INTO map.tblEntityFootNoteToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Footnotes'' 

--
-- Populating map.tblEntityProductGroupToSite
--
PRINT ''Populating map.tblEntityProductGroupToSite...''
TRUNCATE TABLE map.tblEntityProductGroupToSite

INSERT INTO map.tblEntityProductGroupToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Product Groups'' 

--
-- Populating map.tblEntityEmailGroupToSite
--
PRINT ''Populating map.tblEntityEmailGroupToSite...''
TRUNCATE TABLE map.tblEntityEmailGroupToSite

INSERT INTO map.tblEntityEmailGroupToSite(EmailGroupGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.EmailGroupGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblEmailGroups a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''E-mail Groups'' 

--
-- Populating map.tblEntityListViewToSite
--
PRINT ''Populating map.tblEntityListViewToSite...''
TRUNCATE TABLE map.tblEntityListViewToSite

INSERT INTO map.tblEntityListViewToSite(ListViewGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.ListViewGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblListViews a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''List Views''

--
-- Populating map.tblEntityProductToSite
--
PRINT ''Populating map.tblEntityProductToSite...''
TRUNCATE TABLE map.tblEntityProductToSite

INSERT INTO map.tblEntityProductToSite(ProductGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.ProductGuid,s.SiteGuid,a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblProducts a ON a.[ProductIndex] = m.[Index] 
WHERE	m.TypeID = ''Products'' 

--
-- Populating map.tblEntityProcessVariableMessageToSite
--
PRINT ''Populating map.tblEntityProcessVariableMessageToSite...''
TRUNCATE TABLE map.tblEntityProcessVariableMessageToSite

INSERT INTO map.tblEntityProcessVariableMessageToSite(ApplicationStringGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.ApplicationStringGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblApplicationString a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Process Variable Message'' 

--
-- Populating map.tblEntityStandingOfferToSite
--
PRINT ''Populating map.tblEntityStandingOfferToSite...''
TRUNCATE TABLE map.tblEntityStandingOfferToSite

INSERT INTO map.tblEntityStandingOfferToSite(StandingOfferGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.StandingOfferGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblStandingOffers a ON a.[StandingOfferIndex] = m.[Index] 
WHERE	m.TypeID = ''StandingOffers'' 

--
-- Populating map.tblEntityTransactionAliasToSite
--
PRINT ''Populating map.tblEntityTransactionAliasToSite...''
TRUNCATE TABLE map.tblEntityTransactionAliasToSite

INSERT INTO map.tblEntityTransactionAliasToSite(TransactionAliasGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.TransactionAliasGuid,s.SiteGuid, a.SiteGuid
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblTransactionAliases a ON a.[AliasID ] = m.[Index] 
WHERE	m.TypeID = ''Transaction Aliases'' 

--
-- Populating map.tblEntityUserGroupToSite
--
PRINT ''Populating map.tblEntityUserGroupToSite...''
--TRUNCATE TABLE map.tblEntityUserGroupToSite

INSERT INTO map.tblEntityUserGroupToSite(GroupGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.GroupGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblGroups a ON a.[GroupIndex] = m.[Index] 
WHERE	m.TypeID = ''User Groups'' 

--
-- Populating map.tblEntityUserToSite
--
PRINT ''Populating map.tblEntityUserToSite...''
--TRUNCATE TABLE map.tblEntityUserToSite

INSERT INTO map.tblEntityUserToSite(UserGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.UserGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER JOIN tblUsers a ON a.[UserIndex] = m.[Index] 
WHERE m.TypeID = ''Users'' 

--
-- Populating map.tblEntityFuelCardToSite
--
PRINT ''Populating map.tblEntityFuelCardToSite...''
TRUNCATE TABLE map.tblEntityFuelCardToSite

INSERT INTO map.tblEntityFuelCardToSite(FuelCardGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.FuelCardGuid,s.SiteGuid,a.SiteGuid
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblFuelCards a ON a.[FuelCardIndex] = m.[Index] 
WHERE	m.TypeID = ''Fuel Card'' 

--
-- Populating map.tblEntityIATACodeToSite
--
PRINT ''Populating map.tblEntityIATACodeToSite...''
TRUNCATE TABLE map.tblEntityIATACodeToSite

INSERT INTO map.tblEntityIATACodeToSite(IATAGuid,SiteGuid, AssignedFromSiteGuid) 
SELECT	a.IATAGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblIATA a ON a.[IATAIndex] = m.[Index] 
WHERE	m.TypeID = ''IATA Codes'' 

--
-- Populating map.tblEntityDataDictionaryToSite
--
PRINT ''Populating map.tblEntityDataDictionaryToSite...''
TRUNCATE TABLE map.tblEntityDataDictionaryToSite

INSERT INTO map.tblEntityDataDictionaryToSite(OwnerSiteGuid,MapToSiteGuid,AssignedFromSiteGuid) 
SELECT	a.SiteGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER JOIN tblSites a on m.[Index] = a.SiteIndex
WHERE	m.TypeID = ''Data Dictionary'' 

--
-- Populating map.tblEntityLedgerViewToSite
--
PRINT ''Populating map.tblEntityLedgerViewToSite...''
TRUNCATE TABLE map.tblEntityLedgerViewToSite

INSERT INTO map.tblEntityLedgerViewToSite(ListViewGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.ListViewGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblListViews a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''List Views''  And a.[Type]=2 AND a.[TypeIndex]=1

--
-- Populating map.tblEntityPersonnelTrainingToSite
--
PRINT ''Populating map.tblEntityPersonnelTrainingToSite...''
TRUNCATE TABLE map.tblEntityPersonnelTrainingToSite

INSERT INTO map.tblEntityPersonnelTrainingToSite(QualificationGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.QualificationGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualifications a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Personnel Training'' 

--
-- Populating map.tblEntityUserDataToSite
--
PRINT ''Populating map.tblEntityUserDataToSite...''
TRUNCATE TABLE map.tblEntityUserDataToSite

INSERT INTO map.tblEntityUserDataToSite(OwnerSiteGuid,MapToSiteGuid,AssignedFromSiteGuid) 
SELECT	a.SiteGuid,s.SiteGuid, a.SiteGuid 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER JOIN tblSites a on m.[Index] = a.SiteIndex
WHERE	m.TypeID = ''User Data'' 

--
-- Populating map.tblEntityLedgerAggregateColumnToSite
--
PRINT ''Populating map.tblEntityLedgerAggregateColumnToSite...''
TRUNCATE TABLE map.tblEntityLedgerAggregateColumnToSite

/*  Vivian commented this out because the index column does not exist in the 
INSERT INTO map.tblEntityLedgerAggregateColumnToSite(LedgerAggregateColumnGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.LedgerAggregateColumnGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblLedgerAggregateColumns a ON a.[Index] = m.[Index] 
WHERE	m.TypeID = ''Ledger Aggregate Column'' 
*/

--
-- Populating map.tblEntityQualityTagToSite
--
PRINT ''Populating map.tblEntityQualityTagToSite...''
TRUNCATE TABLE map.tblEntityQualityTagToSite

INSERT INTO map.tblEntityQualityTagToSite(QualityTagGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.QualityTagGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblQualityTags a ON a.[QualityTagIndex] = m.[Index] 
WHERE	m.TypeID = ''Quality Tag'' 

--
-- Populating map.tblEntityQuerySettingToSite
--
PRINT ''Populating map.tblEntityQuerySettingToSite...''
TRUNCATE TABLE map.tblEntityQuerySettingToSite

INSERT INTO map.tblEntityQuerySettingToSite(SiteGuid,MapToSiteGuid, AssignedFromSiteGuid) 
SELECT	a.SiteGuid,s.SiteGuid, a.SiteGuid  
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblSites a ON a.[SiteIndex] = m.[Index] 
WHERE	m.TypeID = ''Query Settings''

--
-- Populating map.tblEntityTestToSite
--
PRINT ''Populating map.tblEntityTestToSite...''
TRUNCATE TABLE map.tblEntityTestToSite

INSERT INTO map.tblEntityTestToSite(TestDefinitionGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.TestDefinitionGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblTestDefinitions a ON a.[TestDefinitionIndex] = m.[Index] 
WHERE	m.TypeID = ''Test''

--
-- Populating map.tblEntityTestSetToSite
--
PRINT ''Populating map.tblEntityTestSetToSite...''
TRUNCATE TABLE map.tblEntityTestSetToSite

INSERT INTO map.tblEntityTestSetToSite(TestSetDefinitionGuid,SiteGuid,AssignedFromSiteGuid) 
SELECT	a.TestSetDefinitionGuid,s.SiteGuid,''00000000-0000-0000-0000-000000000000'' 
FROM	tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.SiteIndex = m.SiteIndex 
INNER	JOIN tblTestSetDefinitions a ON a.[TestSetDefinitionIndex] = m.[Index] 
WHERE	m.TypeID = ''Test Set'' 

/*
	End of script Create And Populate Entity To Site Map Split Tables
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-2 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-2 Update Map and Split Tables', 
		@step_id=23, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*##################2222222222222222222222222222222222222222222222222##################################*/


/*
	UPGRADE Qualifications
	Source Script: 8.0.5.0-011 WI-24688 DB Revision - Create And Populate Entity To Site Map Split Tables
*/

--
-- Populating map.tblQualificationCompanyCertificateAndPermitToCompany
--
PRINT ''Populating map.tblQualificationCompanyCertificateAndPermitToCompany...''
TRUNCATE TABLE map.tblQualificationCompanyCertificateAndPermitToCompany

INSERT INTO map.tblQualificationCompanyCertificateAndPermitToCompany(QualificationGuid, CompanyGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.CompanyGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblCompanies en ON en.[CompanyIndex] = mp.[Index]
WHERE	mp.[Type] = 0 


--
-- Populating map.tblQualificationEquipmentTestAndInspectionToEquipment
--
PRINT ''Populating map.tblQualificationEquipmentTestAndInspectionToEquipment...''
TRUNCATE TABLE map.tblQualificationEquipmentTestAndInspectionToEquipment

INSERT INTO map.tblQualificationEquipmentTestAndInspectionToEquipment(QualificationGuid, EquipmentGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.EquipmentGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblEquipment en ON en.[Index] = mp.[Index]
WHERE	mp.[Type] = 1 


--
-- Populating map.tblQualificationEquipmentTagAndLicenseToEquipment
--
PRINT ''Populating map.tblQualificationEquipmentTagAndLicenseToEquipment...''
TRUNCATE TABLE map.tblQualificationEquipmentTagAndLicenseToEquipment

INSERT INTO map.tblQualificationEquipmentTagAndLicenseToEquipment(QualificationGuid, EquipmentGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.EquipmentGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblEquipment en ON en.[Index] = mp.[Index]
WHERE	mp.[Type] = 2 


--
-- Populating map.tblQualificationPersonQualificationToPerson
--
PRINT ''Populating map.tblQualificationPersonQualificationToPerson...''
TRUNCATE TABLE map.tblQualificationPersonQualificationToPerson

INSERT INTO map.tblQualificationPersonQualificationToPerson(QualificationGuid, PersonnelGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.PersonnelGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblPersonnel en ON en.[PersonIndex] = mp.[Index]
WHERE	mp.[Type] = 3 


--
-- Populating map.tblQualificationPersonLicenseToPerson
--
PRINT ''Populating map.tblQualificationPersonLicenseToPerson...''
TRUNCATE TABLE map.tblQualificationPersonLicenseToPerson

INSERT INTO map.tblQualificationPersonLicenseToPerson(QualificationGuid, PersonnelGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.PersonnelGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblPersonnel en ON en.[PersonIndex] = mp.[Index]
WHERE	mp.[Type] = 4 


--
-- Populating map.tblQualificationPersonTrainingToPerson
--
PRINT ''Populating map.tblQualificationPersonTrainingToPerson...''
TRUNCATE TABLE map.tblQualificationPersonTrainingToPerson

INSERT INTO map.tblQualificationPersonTrainingToPerson(QualificationGuid, PersonnelGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.PersonnelGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblPersonnel en ON en.[PersonIndex] = mp.[Index]
WHERE	mp.[Type] = 5 


--
-- Populating map.tblQualificationPersonQualificationToEquipmentType
--
PRINT ''Populating map.tblQualificationPersonQualificationToEquipmentType...''
TRUNCATE TABLE map.tblQualificationPersonQualificationToEquipmentType

INSERT INTO map.tblQualificationPersonQualificationToEquipmentType(QualificationGuid, EquipmentTypeGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.EquipmentTypeGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblEquipmentTypes en ON en.[EqTypeIndex] = mp.[Index]
WHERE	mp.[Type] = 6 


--
-- Populating map.tblQualificationPersonTrainingToEquipmentType
--
PRINT ''Populating map.tblQualificationPersonTrainingToEquipmentType...''
TRUNCATE TABLE map.tblQualificationPersonTrainingToEquipmentType

INSERT INTO map.tblQualificationPersonTrainingToEquipmentType(QualificationGuid, EquipmentTypeGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.EquipmentTypeGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblEquipmentTypes en ON en.[EqTypeIndex] = mp.[Index]
WHERE	mp.[Type] = 7 


--
-- Populating map.tblQualificationPersonQualificationToStation
--
PRINT ''Populating map.tblQualificationPersonQualificationToStation...''
TRUNCATE TABLE map.tblQualificationPersonQualificationToStation

INSERT INTO map.tblQualificationPersonQualificationToStation(QualificationGuid, StationGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.StationGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblStations en ON en.[Index] = mp.[Index]
WHERE	mp.[Type] = 8 


--
-- Populating map.tblQualificationPersonTrainingToStation
--
PRINT ''Populating map.tblQualificationPersonTrainingToStation...''
TRUNCATE TABLE map.tblQualificationPersonTrainingToStation

INSERT INTO map.tblQualificationPersonTrainingToStation(QualificationGuid, StationGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.StationGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu 
INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblStations en ON en.[Index] = mp.[Index]
WHERE	mp.[Type] = 9 


--
-- Populating map.tblQualificationEquipmentTestAndInspectionToStation
--
PRINT ''Populating map.tblQualificationEquipmentTestAndInspectionToStation...''
TRUNCATE TABLE map.tblQualificationEquipmentTestAndInspectionToStation

INSERT INTO map.tblQualificationEquipmentTestAndInspectionToStation(QualificationGuid, StationGuid, Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,[ID],Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT qu.QualificationGuid,en.StationGuid,mp.Sequence,mp.Instructor,mp.DateCompleted,mp.DateDue,mp.ExpirationDate,mp.[ID],mp.Rating,mp.HistoricalRecord,mp.CreatedDate,mp.CreatedBy,mp.UpdatedDate,mp.UpdatedBy 
FROM tblQualifications qu INNER JOIN tblQualificationsMap mp ON mp.[AssignedIndex]=qu.[Index] 
INNER JOIN tblStations en ON en.[Index] = mp.[Index]
WHERE	mp.[Type] = 10 

/*
End of script  8.0.5.0-011 WI-24688 DB Revision - Create And Populate Entity To Site Map Split Tables
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-3 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-3 Update Map and Split Tables', 
		@step_id=24, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*####################################33333333333333333333333333333333333#################################*/
/*
	UPGRADE Prodcut Map
	Source Script: Create And Populate Product Map Split Tables

	Notes:	Map Product tables had the NoteIndex field which got dropped all tables but map.tblProductToCompany & tbl.ProductToCompanyGroup. A column 
			SpecialIntructionNote column NVARCHAR(2000) was introduced to persists notes (script 21.0035 and script 21.0036)
*/

PRINT ''Populating map.tblProductToBlendComponent...''
TRUNCATE TABLE map.tblProductToBlendComponent

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToBlendComponent'')
	ALTER TABLE map.tblProductToBlendComponent
	ADD _LegacyProductMapIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToBlendComponent'')
	ALTER TABLE map.tblProductToBlendComponent
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToBlendComponent'')
	ALTER TABLE map.tblProductToBlendComponent
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToBlendComponent(
ProductGuid,AssignedToProductGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[ProductGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid, ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblProducts mt ON mt.[ProductIndex]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 1


--*** MIGRATE DATE FROM tblProductMap TO tblProductToProductGroup
PRINT ''Populating map.tblProductToProductGroup...''
TRUNCATE TABLE map.tblProductToProductGroup
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToProductGroup'')
	ALTER TABLE map.tblProductToProductGroup
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToProductGroup'')
	ALTER TABLE map.tblProductToProductGroup
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToProductGroup'')
	ALTER TABLE map.tblProductToProductGroup
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToProductGroup(
ProductGuid,AssignedToApplicationStringGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[ApplicationStringGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblApplicationString mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 2


--*** MIGRATE DATE FROM tblProductMap TO tblProductToPresetRecipe
PRINT ''Populating map.tblProductToPresetRecipe...''
TRUNCATE TABLE map.tblProductToPresetRecipe
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetRecipe'')
	ALTER TABLE map.tblProductToPresetRecipe
	ADD _LegacyProductMapIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetRecipe'')
	ALTER TABLE map.tblProductToPresetRecipe
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetRecipe'')
	ALTER TABLE map.tblProductToPresetRecipe
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToPresetRecipe(
ProductGuid,AssignedToLoadArmGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[LoadArmGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 3


--*** MIGRATE DATE FROM tblProductMap TO tblProductToPresetInjector
PRINT ''Populating map.tblProductToPresetInjector...''
TRUNCATE TABLE map.tblProductToPresetInjector
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetInjector'')
	ALTER TABLE map.tblProductToPresetInjector
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetInjector'')
	ALTER TABLE map.tblProductToPresetInjector
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetInjector'')
	ALTER TABLE map.tblProductToPresetInjector
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToPresetInjector(
ProductGuid,AssignedToLoadArmGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[LoadArmGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,pr.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 4


--*** MIGRATE DATE FROM tblProductMap TO tblProductToPresetFlowControlledAdditive
PRINT ''Populating map.tblProductToPresetFlowControlledAdditive...''
TRUNCATE TABLE map.tblProductToPresetFlowControlledAdditive
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetFlowControlledAdditive'')
	ALTER TABLE map.tblProductToPresetFlowControlledAdditive
	ADD _LegacyProductMapIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetFlowControlledAdditive'')
	ALTER TABLE map.tblProductToPresetFlowControlledAdditive
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetFlowControlledAdditive'')
	ALTER TABLE map.tblProductToPresetFlowControlledAdditive
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToPresetFlowControlledAdditive(
ProductGuid,AssignedToLoadArmGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[LoadArmGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 14


--*** MIGRATE DATE FROM tblProductMap TO tblProductToOffloadExternalMeter
PRINT ''Populating map.tblProductToOffloadExternalMeter...''
TRUNCATE TABLE map.tblProductToOffloadExternalMeter
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToOffloadExternalMeter'')
	ALTER TABLE map.tblProductToOffloadExternalMeter
	ADD _LegacyProductMapIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToOffloadExternalMeter'')
	ALTER TABLE map.tblProductToOffloadExternalMeter
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToOffloadExternalMeter'')
	ALTER TABLE map.tblProductToOffloadExternalMeter
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToOffloadExternalMeter(
ProductGuid,AssignedToLoadArmGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[LoadArmGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 15


--*** MIGRATE DATE FROM tblProductMap TO tblProductToAdditiveProfile
PRINT ''Populating map.tblProductToAdditiveProfile...''
TRUNCATE TABLE map.tblProductToAdditiveProfile
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToAdditiveProfile'')
	ALTER TABLE map.tblProductToAdditiveProfile
	ADD _LegacyProductMapIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToAdditiveProfile'')
	ALTER TABLE map.tblProductToAdditiveProfile
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToAdditiveProfile'')
	ALTER TABLE map.tblProductToAdditiveProfile
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToAdditiveProfile(
ProductGuid,AssignedToAdditiveProfileGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,DesiredTreatRate,EnableRecipe,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[AdditiveProfileGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.DesiredTreatRate,ms.EnableRecipe,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblAdditiveProfiles mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 5


--*** MIGRATE DATE FROM tblProductMap TO tblProductToAdditiveProfile
PRINT ''Populating map.tblProductToCompany...''
TRUNCATE TABLE map.tblProductToCompany

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompany'')
	ALTER TABLE map.tblProductToCompany
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompany'')
	ALTER TABLE map.tblProductToCompany
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompany'')
	ALTER TABLE map.tblProductToCompany
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToCompany(
ProductGuid,AssignedToCompanyGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[CompanyGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblCompanies mt ON mt.[CompanyIndex]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 6


--*** MIGRATE DATE FROM tblProductMap TO tblProductToPresetComponentTankOrTankGroup
PRINT ''Populating map.tblProductToPresetComponentTankOrTankGroup...''
TRUNCATE TABLE map.tblProductToPresetComponentTankOrTankGroup
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetComponentTankOrTankGroup'')
	ALTER TABLE map.tblProductToPresetComponentTankOrTankGroup
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetComponentTankOrTankGroup'')
	ALTER TABLE map.tblProductToPresetComponentTankOrTankGroup
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetComponentTankOrTankGroup'')
	ALTER TABLE map.tblProductToPresetComponentTankOrTankGroup
	ADD _LegacyAssignedToIndex INT NULL
GO

INSERT INTO map.tblProductToPresetComponentTankOrTankGroup(
	ProductGuid,
	AssignedToLoadArmGuid,
	Sequence,
	BlendPercentage,
	AdditiveRate,
	Ratio,
	AdditiveCycleVolume,
	Tolerance,
	PresetNumber,
	AdditiveProfileGuid,
	TankGuid,
	TankGroupApplicationStringGuid,
	MeterID,
	ShipToProductID,
	ShipToProductCode,
	ShipToLoadRackDisplayText,
	UnavailableInventoryGross,
	UnavailableInventoryNet,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	_LegacyProductMapIndex,
	_LegacyAssignedIndex,
	_LegacyAssignedToIndex) 
SELECT
	mf.[ProductGuid],
	la.[LoadArmGuid],
	ms.Sequence,
	ms.BlendPercentage,
	ms.AdditiveRate,
	ms.Ratio,
	ms.AdditiveCycleVolume,
	ms.Tolerance,
	ms.PresetNumber,
	pr.AdditiveProfileGuid,
	tk.TankGuid,
	NULL,
	ms.MeterID,
	ms.ShipToProductID,
	ms.ShipToProductCode,
	ms.ShipToLoadRackDisplayText,
	ms.UnavailableInventoryGross,
	ms.UnavailableInventoryNet,
	ms.CreatedDate,
	ms.CreatedBy,
	ms.UpdatedDate,
	ms.UpdatedBy,
	ms.[Index],
	ms.[AssignedIndex],
	ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms la ON la.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 10

INSERT INTO map.tblProductToPresetComponentTankOrTankGroup(
	ProductGuid,
	AssignedToLoadArmGuid,
	Sequence,
	BlendPercentage,
	AdditiveRate,
	Ratio,
	AdditiveCycleVolume,
	Tolerance,
	PresetNumber,
	AdditiveProfileGuid,
	TankGuid,
	TankGroupApplicationStringGuid,
	MeterID,
	ShipToProductID,
	ShipToProductCode,
	ShipToLoadRackDisplayText,
	UnavailableInventoryGross,
	UnavailableInventoryNet,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	_LegacyProductMapIndex,
	_LegacyAssignedIndex,
	_LegacyAssignedToIndex) 
SELECT 
	mf.[ProductGuid],
	la.[LoadArmGuid],
	ms.Sequence,
	ms.BlendPercentage,
	ms.AdditiveRate,
	ms.Ratio,
	ms.AdditiveCycleVolume,
	ms.Tolerance,
	ms.PresetNumber,
	pr.AdditiveProfileGuid,
	tk.TankGuid,
	NULL,
	ms.MeterID, 
	ms.ShipToProductID,
	ms.ShipToProductCode,
	ms.ShipToLoadRackDisplayText,
	ms.UnavailableInventoryGross,
	ms.UnavailableInventoryNet,
	ms.CreatedDate,
	ms.CreatedBy,
	ms.UpdatedDate,
	ms.UpdatedBy,
	ms.[Index],
	ms.[AssignedIndex],
	ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms la ON la.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 7

--EXEC sp_dbcmptlevel ''ConsolidatedDB'', 100 


--*** MIGRATE DATE FROM tblProductMap TO tblProductToTransactionAliasExclusion
PRINT ''Populating map.tblProductToTransactionAliasExclusion...''
TRUNCATE TABLE map.tblProductToTransactionAliasExclusion
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToTransactionAliasExclusion'')
	ALTER TABLE map.tblProductToTransactionAliasExclusion
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToTransactionAliasExclusion'')
	ALTER TABLE map.tblProductToTransactionAliasExclusion
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToTransactionAliasExclusion'')
	ALTER TABLE map.tblProductToTransactionAliasExclusion
	ADD _LegacyAssignedToIndex INT NULL;
GO

INSERT INTO map.tblProductToTransactionAliasExclusion(
ProductGuid,AssignedToTransactionAliasGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[TransactionAliasGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblTransactionAliases mt ON mt.[AliasID]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 8


--*** MIGRATE DATE FROM tblProductMap TO tblProductToCompanyGroup
PRINT ''Populating map.tblProductToCompanyGroup...''
TRUNCATE TABLE map.tblProductToCompanyGroup
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompanyGroup'')
	ALTER TABLE map.tblProductToCompanyGroup
	ADD _LegacyProductMapIndex INT NULL
GO
	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompanyGroup'')
	ALTER TABLE map.tblProductToCompanyGroup
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToCompanyGroup'')
	ALTER TABLE map.tblProductToCompanyGroup
	ADD _LegacyAssignedToIndex INT NULL;
GO

INSERT INTO map.tblProductToCompanyGroup(
ProductGuid,AssignedToApplicationStringGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[ApplicationStringGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblApplicationString mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 9


--*** MIGRATE DATE FROM tblProductMap TO tblProductToUnavailableInventoryCompany
PRINT ''Populating map.tblProductToUnavailableInventoryCompany...''
TRUNCATE TABLE map.tblProductToUnavailableInventoryCompany

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToUnavailableInventoryCompany'')
	ALTER TABLE map.tblProductToUnavailableInventoryCompany
	ADD _LegacyProductMapIndex INT NULL
GO

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToUnavailableInventoryCompany'')
	ALTER TABLE map.tblProductToUnavailableInventoryCompany
	ADD _LegacyAssignedIndex INT NULL
GO


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToUnavailableInventoryCompany'')
	ALTER TABLE map.tblProductToUnavailableInventoryCompany
	ADD _LegacyAssignedToIndex INT NULL;
GO

INSERT INTO map.tblProductToUnavailableInventoryCompany(
ProductGuid,AssignedToCompanyGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid, MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[CompanyGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblCompanies mt ON mt.[CompanyIndex]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 11

--*** MIGRATE DATE FROM tblProductMap TO tblProductToPresetExternalComponent

PRINT ''Populating map.tblProductToPresetExternalComponent...''
TRUNCATE TABLE map.tblProductToPresetExternalComponent

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetExternalComponent'')
	ALTER TABLE map.tblProductToPresetExternalComponent
	ADD _LegacyProductMapIndex INT NULL
GO

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetExternalComponent'')
	ALTER TABLE map.tblProductToPresetExternalComponent
	ADD _LegacyAssignedIndex INT NULL
GO


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToPresetExternalComponent'')
	ALTER TABLE map.tblProductToPresetExternalComponent
	ADD _LegacyAssignedToIndex INT NULL;
GO


INSERT INTO map.tblProductToPresetExternalComponent(
ProductGuid,AssignedToLoadArmGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[LoadArmGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblLoadArms mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 12


--*** MIGRATE DATE FROM tblProductMap TO tblProductToSupplierProductCompany
PRINT ''Populating map.tblProductToSupplierProductCompany...''
TRUNCATE TABLE map.tblProductToSupplierProductCompany

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToSupplierProductCompany'')
	ALTER TABLE map.tblProductToSupplierProductCompany
	ADD _LegacyProductMapIndex INT NULL
GO


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToSupplierProductCompany'')
	ALTER TABLE map.tblProductToSupplierProductCompany
	ADD _LegacyAssignedIndex INT NULL
GO


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToSupplierProductCompany'')
	ALTER TABLE map.tblProductToSupplierProductCompany
	ADD _LegacyAssignedToIndex INT NULL;
GO

INSERT INTO map.tblProductToSupplierProductCompany(
	ProductGuid,
	AssignedToCompanyGuid,
	Sequence,
	BlendPercentage,
	AdditiveRate,
	Ratio,
	AdditiveCycleVolume,
	Tolerance,
	PresetNumber,
	AdditiveProfileGuid,
	TankGuid,
	MeterID,
	ShipToProductID,
	ShipToProductCode,
	ShipToLoadRackDisplayText,
	UnavailableInventoryGross,
	UnavailableInventoryNet,
	CreatedDate,
	CreatedBy,
	UpdatedDate,UpdatedBy,
	_LegacyProductMapIndex,
	_LegacyAssignedIndex,
	_LegacyAssignedToIndex) 
SELECT 
	mf.[ProductGuid],
	mt.[CompanyGuid],
	ms.Sequence,
	ms.BlendPercentage,
	ms.AdditiveRate,
	ms.Ratio,
	ms.AdditiveCycleVolume,
	ms.Tolerance,
	ms.PresetNumber,
	pr.AdditiveProfileGuid,
	tk.TankGuid,
	ms.MeterID,
	ms.ShipToProductID,
	ms.ShipToProductCode,
	ms.ShipToLoadRackDisplayText,
	ms.UnavailableInventoryGross,
	ms.UnavailableInventoryNet,
	ms.CreatedDate,
	ms.CreatedBy,
	ms.UpdatedDate,
	ms.UpdatedBy,
	ms.[Index],
	ms.[AssignedIndex],
	ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblCompanies mt ON mt.[CompanyIndex]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 13



--*** MIGRATE DATE FROM tblProductMap TO tblProductToLedgerView
PRINT ''Populating map.tblProductToLedgerView...''
TRUNCATE TABLE map.tblProductToLedgerView

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProductMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToLedgerView'')
	ALTER TABLE map.tblProductToLedgerView
	ADD _LegacyProductMapIndex INT NULL
GO

	
IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToLedgerView'')
	ALTER TABLE map.tblProductToLedgerView
	ADD _LegacyAssignedIndex INT NULL
GO

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAssignedToIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblProductToLedgerView'')
	ALTER TABLE map.tblProductToLedgerView
	ADD _LegacyAssignedToIndex INT NULL;
GO



--*** MIGRATE DATE FROM tblProductMap TO tblProductToLedgerView
INSERT INTO map.tblProductToLedgerView(
ProductGuid,AssignedToListViewGuid,Sequence,BlendPercentage,AdditiveRate,Ratio,AdditiveCycleVolume,Tolerance,PresetNumber,AdditiveProfileGuid,
TankGuid,MeterID, ShipToProductID,ShipToProductCode,ShipToLoadRackDisplayText,UnavailableInventoryGross,UnavailableInventoryNet,CreatedDate,
CreatedBy,UpdatedDate,UpdatedBy,_LegacyProductMapIndex,_LegacyAssignedIndex,_LegacyAssignedToIndex) 
SELECT mf.[ProductGuid],mt.[ListViewGuid],ms.Sequence,ms.BlendPercentage,ms.AdditiveRate,ms.Ratio,ms.AdditiveCycleVolume,ms.Tolerance,ms.PresetNumber,pr.AdditiveProfileGuid,
tk.TankGuid,ms.MeterID, ms.ShipToProductID,ms.ShipToProductCode,ms.ShipToLoadRackDisplayText,ms.UnavailableInventoryGross,ms.UnavailableInventoryNet,ms.CreatedDate,
ms.CreatedBy,ms.UpdatedDate,ms.UpdatedBy,ms.[Index],ms.[AssignedIndex],ms.[AssignedToIndex] 
FROM tblProductMap ms 
INNER JOIN tblProducts mf ON mf.[ProductIndex]=ms.AssignedIndex 
INNER JOIN tblListViews mt ON mt.[Index]=ms.AssignedToIndex 
LEFT JOIN tblTanks tk ON tk.TankIndex=ms.TankIndex 
LEFT JOIN tblAdditiveProfiles pr ON pr.[Index] = ms.AdditiveProfileIndex
LEFT JOIN tblNotes ON tblNotes.[Index] = ms.SpecialInstructionIndex
WHERE ms.Type = 14


/*
	END OF SCRIPT 8.0.5.0-013 WI-24895 DB Revision - Create And Populate Product Map Split Tables.sql
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-4 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-4 Update Map and Split Tables', 
		@step_id=25, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*#############################44444444444444444444444444444444444444444444444444444########################*/


/*
	START OF SCRIPT 8.0.5.0-014 WI-25260 DB Revision - Create And Populate Application String Map Split Tables
*/
--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToDotHazardousMessage
PRINT ''Populating map.tblApplicationStringToDotHazardousMessage...''
TRUNCATE TABLE map.tblApplicationStringToDotHazardousMessage

INSERT INTO map.tblApplicationStringToDotHazardousMessage(ApplicationStringGuid,ProductGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ProductGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON mf.[Index] = sr.[AssignedIndex]
INNER JOIN tblProducts mt ON mt.[ProductIndex] = sr.[Index] 
WHERE	sr.[Type] = 0 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToProductMessage
PRINT ''Populating map.tblApplicationStringToProductMessage...''
TRUNCATE TABLE map.tblApplicationStringToProductMessage

INSERT INTO map.tblApplicationStringToProductMessage(ApplicationStringGuid,ProductGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ProductGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON mf.[Index] = sr.[AssignedIndex]
INNER JOIN tblProducts mt ON mt.[ProductIndex] = sr.[Index] 
WHERE	sr.[Type] = 1 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToAlarmEventCategory
PRINT ''Populating map.tblApplicationStringToAlarmEventCategory...''
TRUNCATE TABLE map.tblApplicationStringToAlarmEventCategory

INSERT INTO map.tblApplicationStringToAlarmEventCategory(ApplicationStringGuid,EmailGroupGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.EmailGroupGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblEmailGroups mt ON sr.[Index] = mt.[Index] 
WHERE	sr.[Type] = 6 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToEmailAddress
PRINT ''Populating map.tblApplicationStringToEmailAddress...''
TRUNCATE TABLE map.tblApplicationStringToEmailAddress

INSERT INTO map.tblApplicationStringToEmailAddress(ApplicationStringGuid,EmailGroupGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.EmailGroupGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblEmailGroups mt ON sr.[Index] = mt.[Index] 
WHERE	sr.[Type] = 7 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToEntryMessage
PRINT ''Populating map.tblApplicationStringToEntryMessage...''
TRUNCATE TABLE map.tblApplicationStringToEntryMessage

INSERT INTO map.tblApplicationStringToEntryMessage(ApplicationStringGuid,ProductGroupApplicationStringGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ApplicationStringGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblApplicationString mt ON sr.[Index] = mt.[Index] 
WHERE	sr.[Type] = 9 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToExitMessage
PRINT ''Populating map.tblApplicationStringToExitMessage...''
TRUNCATE TABLE map.tblApplicationStringToExitMessage

INSERT INTO map.tblApplicationStringToExitMessage(ApplicationStringGuid,ProductGroupApplicationStringGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ApplicationStringGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblApplicationString mt ON sr.[Index] = mt.[Index] 
WHERE	sr.[Type] = 10 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToFootNoteShipTo
PRINT ''Populating map.tblApplicationStringToFootNoteShipTo...''
TRUNCATE TABLE map.tblApplicationStringToFootNoteShipTo

INSERT INTO map.tblApplicationStringToFootNoteShipTo(ApplicationStringGuid,CompanyGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.CompanyGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblCompanies mt ON sr.[Index] = mt.[CompanyIndex] 
WHERE	sr.[Type] = 11 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToFootNoteShipper

PRINT ''Populating map.tblApplicationStringToFootNoteShipper...''
TRUNCATE TABLE map.tblApplicationStringToFootNoteShipper

INSERT INTO map.tblApplicationStringToFootNoteShipper(ApplicationStringGuid,CompanyGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.CompanyGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblCompanies mt ON sr.[Index] = mt.[CompanyIndex] 
WHERE	sr.[Type] = 12 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToFootNoteShipToState
PRINT ''Populating map.tblApplicationStringToFootNoteShipToState...''
TRUNCATE TABLE map.tblApplicationStringToFootNoteShipToState

INSERT INTO map.tblApplicationStringToFootNoteShipToState(ApplicationStringGuid,AssignedToApplicationStringGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ApplicationStringGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblApplicationString mt ON sr.[Index] = mt.[Index] 
WHERE	sr.[Type] = 13 


--*** MIGRATE DATE FROM tblProductMap TO tblApplicationStringToFootNoteProduct
PRINT ''Populating map.tblApplicationStringToFootNoteProduct...''
TRUNCATE TABLE map.tblApplicationStringToFootNoteProduct

INSERT INTO map.tblApplicationStringToFootNoteProduct(ApplicationStringGuid,ProductGuid,Sequence,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT mf.ApplicationStringGuid,mt.ProductGuid,sr.Sequence,sr.CreatedDate,sr.CreatedBy,sr.UpdatedDate,sr.UpdatedBy 
FROM tblApplicationStringMap sr 
INNER JOIN tblApplicationString mf ON sr.[AssignedIndex]=mf.[Index] 
INNER JOIN tblProducts mt ON sr.[Index] = mt.[ProductIndex] 
WHERE	sr.[Type] = 14 

/*
	END OF SCRIPT 8.0.5.0-014 WI-25260 DB Revision - Create And Populate Application String Map Split Tables
*/


/*###############################################################################################*/


/*
	START OF SCRIPT 8.0.5.0-015 WI-25260 DB Revision - Create And Populate Company Role Map Tables
*/


--*** MIGRATE DATE FROM tblProductMap TO tblCompanyToRole

PRINT ''Populating map.tblCompanyToRole...''


TRUNCATE TABLE map.tblCompanyToRole


INSERT INTO [map].[tblCompanyToRole]([CompanyGuid],[LookupCompanyRoleIndex],[SiteGuid],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	co.CompanyGuid,ma.[Role],si.SiteGuid,ma.CreatedDate,ma.CreatedBy,ma.CreatedDate,ma.CreatedBy
FROM	tblCompanyRoleMap ma
INNER JOIN tblCompanies co ON co.CompanyIndex=ma.CompanyIndex
INNER JOIN tblSites si ON si.SiteIndex=ma.SiteIndex

/*
	END OF SCRIPT 8.0.5.0-015 WI-25260 DB Revision - Create And Populate Company Role Map Tables
*/


/*###############################################################################################*/
/*
	START OF 8.0.5.0-016 WI-25260 DB Revision - Create And Populate AlarmPriorityToEmailGroup tables
*/
--*** MIGRATE DATE FROM tblProductMap TO [tblAlarmPriorityToEmailGroup]
PRINT ''Populating map.[tblAlarmPriorityToEmailGroup]...''
TRUNCATE TABLE map.[tblAlarmPriorityToEmailGroup]


INSERT INTO [map].[tblAlarmPriorityToEmailGroup](
	[EmailGroupGuid],[AlarmPriorityGuid])
SELECT 	mf.[EmailGroupGuid],mt.[AlarmPriorityGuid]
FROM	 tblAlarmPriorityEmailGroupMap ms
INNER JOIN tblEmailGroups mf ON mf.[Index] = ms.EmailGroupIndex
INNER JOIN tblAlarmPriorities mt ON mt.[Index] = ms.AlarmPriorityIndex

/*
	END OF SCRIPT 8.0.5.0-016 WI-25260 DB Revision - Create And Populate AlarmPriorityToEmailGroup tables
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-018 WI-25260 DB Revision - Create And Populate Associated Transaction Alias tables
*/
--*** MIGRATE DATE FROM tblProductMap TO [tblAlarmPriorityToEmailGroup]
PRINT ''Populating map.[tblAssociatedTransactionAliases]...''
TRUNCATE TABLE map.[tblAssociatedTransactionAliases]

INSERT INTO [map].[tblAssociatedTransactionAliases](
	[ParentTransactionAliasGuid],[ChildTransactionAliasGuid])
SELECT 	pa.[TransactionAliasGuid],ch.[TransactionAliasGuid]
FROM	 tblAssociatedAliasesMap ms
INNER JOIN tblTransactionAliases pa ON pa.[AliasID] = ms.ParentAliasIndex
INNER JOIN tblTransactionAliases ch ON ch.[AliasID] = ms.ChildAliasIndex


--*** MIGRATE DATE FROM tblProductMap TO [tblAlarmPriorityToEmailGroup]
PRINT ''Populating map.[tblAssociatedTransactionAliases]...''
TRUNCATE TABLE map.[tblAssociatedTransactionAliases]

INSERT INTO [map].[tblAssociatedTransactionAliases](
	[ParentTransactionAliasGuid],[ChildTransactionAliasGuid])
SELECT 	pa.[TransactionAliasGuid],ch.[TransactionAliasGuid]
FROM	 tblAssociatedAliasesMap ms
INNER JOIN tblTransactionAliases pa ON pa.[AliasID] = ms.ParentAliasIndex
INNER JOIN tblTransactionAliases ch ON ch.[AliasID] = ms.ChildAliasIndex

/*
	END OF SCRIPT 	8.0.5.0-018 WI-25260 DB Revision - Create And Populate Associated Transaction Alias tables
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-020 WI-25260 DB Revision - Create And Populate Site To Site Map table
*/

/******************************************** FMSyncTool *********************************************/
---------------------------------------------------------------------------
-- THIS IS NO LONGER NEEDED BECAUSE THE FMSyncTool will download the
-- valid mappings from the Enterprise that this terminal will join.
---------------------------------------------------------------------------

-- Comment out for 2nd Migration when we merge into Enterprise
PRINT ''Populating map.[tblSiteToSite]...''
TRUNCATE TABLE map.[tblSiteToSite]

INSERT INTO [map].[tblSiteToSite](ParentSiteGuid,ChildSiteGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.SiteGuid,ch.SiteGuid,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblSiteToSiteMap lg
INNER JOIN tblSites pa ON pa.SiteIndex = lg.ParentSiteIndex
INNER JOIN tblSites ch ON ch.SiteIndex = lg.ChildSiteIndex

/*
	END OF SCRIPT 	8.0.5.0-020 WI-25260 DB Revision - Create And Populate Site To Saite Map table
*/


/*###############################################################################################*/

/*
	START OF SCRIPT Create And Populate User To Group Map tables
	*** REPLACED BY SCRIPT 8.0.5.21-0122 WI-38955 modify table and update users to support siteguid in tblUserToGroup
	*** NEEDS TO BE AFTER map.tblSiteToSiteMap is populated above because the Guid columns are being used
*/

-------------------------------------------------------------------------
-- THIS IS NO LONGER NEEDED BECAUSE THE FMSyncTool will download the
-- valid mappings from the Enterprise that this terminal will join.
-------------------------------------------------------------------------
-- Comment out for 2nd Migration when we merge into Enterprise
-- Update map.tblSiteToSite to correct the mapping of SiteAdmin to SiteAdmin (extracted from 8.0.5.21-0122 WI-38955 script)
UPDATE map.tblSiteToSite SET ParentSiteGuid = ChildSiteGuid where ChildSiteGuid = ''00000000-0000-0000-0000-000000000001''


-- Propogate User To Group Mappings to new [map].[tblUserToGroup] mapping table.
PRINT ''Populating map.[tblUserToGroup]...''
TRUNCATE TABLE map.[tblUserToGroup]

INSERT INTO [map].[tblUserToGroup]([UserGuid],[GroupGuid],[ExpirationDate],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
SELECT	u.UserGuid,g.GroupGuid,(CONVERT([date],dateadd(year,(20),getdate()))),g.SiteGuid,sysdatetimeoffset() ''CreatedDate'',ma.CreatedBy ''CreatedBy'',sysdatetimeoffset() ''UpdatedDate'',ma.CreatedBy ''UpdatedBy''
FROM	dbo.tblUserGroupMap ma
INNER JOIN tblUsers u ON u.UserIndex=ma.UserIndex
INNER JOIN tblGroups g ON g.GroupIndex=ma.GroupIndex
ORDER BY ma.[UserIndex]


/*
	END OF SCRIPT 	S8.0.5.0-017 WI-25260 DB Revision - Create And Populate User To Group Map tables
	*** REPLACED BY SCRIPT 8.0.5.21-0122 WI-38955 modify table and update users to support siteguid in tblUserToGroup
	*** NEEDS TO BE AFTER map.tblSiteToSiteMap is populated above because the Guid columns are being used
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-021 WI-25260 DB Revision - Create And Populate Personnel Role Map table
*/

PRINT ''Populating map.[tblPersonnelToRole]...''
TRUNCATE TABLE map.[tblPersonnelToRole]


INSERT INTO [map].[tblPersonnelToRole]([PersonnelGuid],[LookupPersonnelRoleIndex],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	co.PersonnelGuid,ma.[Role],ma.CreatedDate,ma.CreatedBy,ma.CreatedDate,ma.CreatedBy
FROM	tblPersonRoleMap ma
INNER JOIN tblPersonnel co ON co.PersonIndex=ma.PersonIndex


/*
	END OF SCRIPT 	8.0.5.0-021 WI-25260 DB Revision - Create And Populate Personnel Role Map table
*/
/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-022 WI-25260 DB Revision - Create And Populate Excise Company Map tablee
*/

/*
	END OF SCRIPT 	8.0.5.0-022 WI-25260 DB Revision - Create And Populate Excise Company Map table
*/
/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-023 WI-25260 DB Revision - Create And Populate Company to Company Map table

	[ConsolidatedDB].[dbo].[tblCompanyMap] gets split out into several new [map].[tblCompany??] tables.
	
	Here are the mappings based on the [dbo].[tblCompanyMap].[Type] value to the new [map].[tblCompany??] map tables.

	Type	Destination Table								Migration Order (because of Dependecies)
	0		[map].[tblCompanyLoadOwnerToManager]			1
	1		[map].[tblCompanyShipperToOwner]				2
	2		[map].[tblCompanyBillToToShipper]				3
	3		[map].[tblCompanyShipToToBillTo]				4
	4		[map].[tblCompanyAuthorizedCarrierToCompany]	9
	5		[map].[tblCompanyPersonnelToShipToBillTo]		5
	6		[map].[tblCompanyCompanyToUserGroup]			10
	7		[map].[tblCompanyCompanyToCompanyGroup]			11
	8
	9
	10
	11		[map].[tblCompanySupplierToOwner]				7
	12		[map].[tblCompanyPersonnelToSupplierOwner]		8
	13		
	14		[map].tblCompanyOffLoadOwnerToManager]			6
	15		[map].[tblCompanyPersonnelAssignedToCompany]	12
*/

PRINT ''Processing CompanyMap Type = 0''
PRINT ''Populating map.[tblCompanyLoadOwnerToManager]...''
TRUNCATE TABLE map.[tblCompanyLoadOwnerToManager]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyLoadOwnerToManager'')
	ALTER TABLE map.tblCompanyLoadOwnerToManager
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyLoadOwnerToManager(
	CompanyGuid,AssignedToCompanyGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
INNER JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblCompanies mt ON mt.CompanyIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 0


PRINT ''Processing CompanyMap Type = 1''
PRINT ''Populating map.[tblCompanyShipperToOwner]...''
TRUNCATE TABLE map.[tblCompanyShipperToOwner]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyShipperToOwner'')
	ALTER TABLE map.tblCompanyShipperToOwner
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyShipperToOwner(
	CompanyGuid,CompanyLoadOwnerToManagerGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyLoadOwnerToManagerGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
INNER JOIN dbo.tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN map.tblCompanyLoadOwnerToManager mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 1


PRINT ''Processing CompanyMap Type = 2''
PRINT ''Populating map.[tblCompanyBillToToShipper]...''
TRUNCATE TABLE map.[tblCompanyBillToToShipper]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyBillToToShipper'')
	ALTER TABLE map.tblCompanyBillToToShipper
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyBillToToShipper(
	CompanyGuid,CompanyShipperToOwnerGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyShipperToOwnerGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
INNER JOIN dbo.tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN map.tblCompanyShipperToOwner mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 2


PRINT ''Processing CompanyMap Type = 3''
PRINT ''Populating map.[tblCompanyShipToToBillTo]...''
TRUNCATE TABLE map.[tblCompanyShipToToBillTo]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyShipToToBillTo'')
	ALTER TABLE map.tblCompanyShipToToBillTo
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyShipToToBillTo(
	CompanyGuid,CompanyBillToToShipperGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyBillToToShipperGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
INNER JOIN dbo.tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN map.tblCompanyBillToToShipper mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 3


PRINT ''Processing CompanyMap Type = 5''
PRINT ''Populating map.[tblCompanyPersonnelToShipToBillTo]...''
TRUNCATE TABLE map.[tblCompanyPersonnelToShipToBillTo]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyPersonnelToShipToBillTo'')
	ALTER TABLE map.tblCompanyPersonnelToShipToBillTo
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyPersonnelToShipToBillTo(
	PersonnelGuid,CompanyShipToToBillToGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	CASE WHEN lg.AssignedIndex = 0 THEN NULL ELSE mf.PersonnelGuid END
	,	mt.CompanyShipToToBillToGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
LEFT OUTER JOIN dbo.tblPersonnel mf ON mf.PersonIndex=lg.AssignedIndex
INNER JOIN map.tblCompanyShipToToBillTo mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 5


PRINT ''Processing CompanyMap Type = 14''
PRINT ''Populating map.[tblCompanyOffLoadOwnerToManager]...''
TRUNCATE TABLE map.[tblCompanyOffLoadOwnerToManager]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyOffLoadOwnerToManager'')
	ALTER TABLE map.tblCompanyOffLoadOwnerToManager
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyOffLoadOwnerToManager(
	CompanyGuid,AssignedToCompanyGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
INNER JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblCompanies mt ON mt.CompanyIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 14


PRINT ''Processing CompanyMap Type = 11''
PRINT ''Populating map.[tblCompanySupplierToOwner]...''
TRUNCATE TABLE map.[tblCompanySupplierToOwner]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanySupplierToOwner'')
	ALTER TABLE map.tblCompanySupplierToOwner
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanySupplierToOwner(
	CompanyGuid,CompanyOffLoadOwnerToManagerGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyOffLoadOwnerToManagerGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
INNER JOIN dbo.tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN map.tblCompanyOffLoadOwnerToManager mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 11


PRINT ''Processing CompanyMap Type = 12''
PRINT ''Populating map.[tblCompanyPersonnelToSupplierOwner]...''
TRUNCATE TABLE map.[tblCompanyPersonnelToSupplierOwner]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyPersonnelToSupplierOwner'')
	ALTER TABLE map.tblCompanyPersonnelToSupplierOwner
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyPersonnelToSupplierOwner(
	PersonnelGuid,CompanySupplierToOwnerGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.PersonnelGuid
	,	mt.CompanySupplierToOwnerGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	dbo.tblCompanyMap lg
INNER JOIN dbo.tblPersonnel mf ON mf.PersonIndex=lg.AssignedIndex
INNER JOIN map.tblCompanySupplierToOwner mt ON mt._LegacyCompanyMapIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 12


PRINT ''Processing CompanyMap Type = 4''
PRINT ''Populating map.[tblCompanyAuthorizedCarrierToCompany]...''
TRUNCATE TABLE map.[tblCompanyAuthorizedCarrierToCompany]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyAuthorizedCarrierToCompany'')
	ALTER TABLE map.tblCompanyAuthorizedCarrierToCompany
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyAuthorizedCarrierToCompany(
	CompanyGuid,AssignedToCompanyGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.CompanyGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
INNER JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblCompanies mt ON mt.CompanyIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 4


PRINT ''Processing CompanyMap Type = 6''
PRINT ''Populating map.[tblCompanyCompanyToUserGroup]...''
TRUNCATE TABLE map.[tblCompanyCompanyToUserGroup]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyCompanyToUserGroup'')
	ALTER TABLE map.tblCompanyCompanyToUserGroup
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyCompanyToUserGroup(
	CompanyGuid,GroupGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	case when lg.[AssignedIndex] = 0 and mf.CompanyIndex is NULL THEN NULL ELSE mf.CompanyGuid END
	,	mt.GroupGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
LEFT JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblGroups mt ON mt.GroupIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE (mf.CompanyGuid IS NOT NULL 
		OR lg.[AssignedIndex] = 0) 
		AND lg.[Type] = 6


PRINT ''Processing CompanyMap Type = 7''
PRINT ''Populating map.[tblCompanyCompanyToCompanyGroup]...''
TRUNCATE TABLE map.[tblCompanyCompanyToCompanyGroup]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyCompanyToCompanyGroup'')
	ALTER TABLE map.tblCompanyCompanyToCompanyGroup
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyCompanyToCompanyGroup(
	CompanyGuid,ApplicationStringGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	mt.ApplicationStringGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
INNER JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblApplicationString mt ON mt.[Index]=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 7


PRINT ''Processing CompanyMap Type = 15''
PRINT ''Populating map.[tblCompanyPersonnelAssignedToCompany]...''
TRUNCATE TABLE map.[tblCompanyPersonnelAssignedToCompany]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyCompanyMapIndex'' 
				AND TABLE_SCHEMA=''map'' AND TABLE_NAME=''tblCompanyPersonnelAssignedToCompany'')
	ALTER TABLE map.tblCompanyPersonnelAssignedToCompany
	ADD _LegacyCompanyMapIndex INT NULL
GO

INSERT INTO map.tblCompanyPersonnelAssignedToCompany(
	CompanyGuid,PersonnelGuid,SiteGuid,[ID],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,_LegacyCompanyMapIndex
	)
SELECT	mf.CompanyGuid
	,	p.PersonnelGuid
	,	si.SiteGuid
	,	lg.[ID]
	,	lg.CreatedDate
	,	lg.CreatedBy
	,	lg.UpdatedDate
	,	lg.UpdatedBy
	,	lg.[Index]
FROM	tblCompanyMap lg
INNER JOIN tblCompanies mf ON mf.CompanyIndex=lg.AssignedIndex
INNER JOIN tblPersonnel p ON p.PersonIndex=lg.AssignedToIndex
INNER JOIN tblSites si ON si.SiteIndex = lg.SiteIndex
WHERE lg.[Type] = 15


/*
	END OF SCRIPT 8.0.5.0-023 WI-25260 DB Revision - Create And Populate Company to Company Map table
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-5 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-5 Update Map and Split Tables', 
		@step_id=26, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*################################555555555555555555555555555555555555555555555555###########################*/
/*
	START OF SCRIPT Create And Populate Group Rights Map table
*/
-- Comment out during 2nd Merge when we go to Enterprise
PRINT ''Populating map.[tblGroupToRight]...''
TRUNCATE TABLE map.[tblGroupToRight]

--TRANSLATE 8.0 PERMISSIONS TO 9.x
DECLARE @RightsMappings TABLE(old8Index int, new9Index int)
INSERT INTO @RightsMappings (old8Index, new9Index) 
VALUES 
		(132,	183), --MODIFY_UNOBTAINABLE = 183,
		(133,	184), --CONFIGURE_LOCATIONS = 184,
		(134,	185), --VIEW_MOVEMENT		= 185,
		(135,	186), --CONFIGURE_WEB_LINKS = 186,
		(136,	187) --CONFIGURE_DLA_TEST	= 187

UPDATE GM 
	SET GM.RightIndex = RM.new9Index
FROM tblGroupRightsMap GM INNER JOIN @RightsMappings RM on GM.RightIndex = RM.old8Index 


/*
	This needs to be updated to it checks the Template Database to see if the GroupsToRight mapping records already exist.  If so we need to set the GroupToRightGuid column during the INSERT.
*/
INSERT INTO [map].[tblGroupToRight]([GroupGuid],[LookupRightIndex],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	co.GroupGuid,ma.[RightIndex],ma.CreatedDate,ma.CreatedBy,ma.CreatedDate,ma.CreatedBy
FROM	tblGroupRightsMap ma
INNER JOIN tblGroups co ON co.GroupIndex=ma.GroupIndex
ORDER BY ma.[RightIndex]

/*
	END OF SCRIPT Create And Populate Group Rights Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-025 WI-25260 DB Revision - Create And Populate Group Report Detail Map table
*/
PRINT ''Populating map.[tblGroupToReportDetail]...''
TRUNCATE TABLE map.[tblGroupToReportDetail]

INSERT INTO [map].[tblGroupToReportDetail](GroupGuid,ReportDetailGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.GroupGuid,ch.ReportDetailGuid,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblGroupReportMap lg
INNER JOIN tblGroups pa ON pa.GroupIndex = lg.GroupIndex
INNER JOIN tblReportDetails ch ON ch.ReportIndex = lg.ReportIndex

/*
	END OF SCRIPT 8.0.5.0-025 WI-25260 DB Revision - Create And Populate Group Report Detail Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-026 WI-25260 DB Revision - Create And Populate Group Transaction Aliases Map table
*/

PRINT ''Populating map.[tblGroupToTransactionAlias]...''
TRUNCATE TABLE map.[tblGroupToTransactionAlias]

INSERT INTO [map].[tblGroupToTransactionAlias](GroupGuid,TransactionAliasGuid,LookupRightIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.GroupGuid,ch.TransactionAliasGuid,1,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblGroupTransactionAliasMap lg
INNER JOIN tblGroups pa ON pa.GroupIndex = lg.GroupIndex
INNER JOIN tblTransactionAliases ch ON ch.AliasID = lg.AliasID

/*************************************/

/*
	END OF SCRIPT 8.0.5.0-026 WI-25260 DB Revision - Create And Populate Group Transaction Aliases Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-029 WI-25260 DB Revision - Create And Populate Markup To Company Map table
*/
PRINT ''Populating map.[tblMarkupToCompany]...''
TRUNCATE TABLE map.[tblMarkupToCompany]

INSERT INTO [map].[tblMarkupToCompany](MarkupGuid,CompanyGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.MarkupGuid,ch.CompanyGuid,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblMarkupCompanyMap lg
INNER JOIN tblMarkup pa ON pa.MarkupIndex = lg.MarkupIndex
INNER JOIN tblCompanies ch ON ch.CompanyIndex = lg.CompanyIndex

/*
	END OF SCRIPT 8.0.5.0-029 WI-25260 DB Revision - Create And Populate Markup To Company Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-030 WI-25260 DB Revision - Create And Populate Query To Group Map table
*/
PRINT ''Populating map.[tblQueryStorageToGroup]...''
TRUNCATE TABLE map.[tblQueryStorageToGroup]

INSERT INTO [map].[tblQueryStorageToGroup](QueryStorageGuid,GroupGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.QueryStorageGuid,ch.GroupGuid,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblQueryGroupMap lg
INNER JOIN tblQueryStorage pa ON pa.[Index] = lg.QueryIndex
INNER JOIN tblGroups ch ON ch.[GroupIndex] = lg.GroupIndex

/*
	END OF SCRIPT 8.0.5.0-030 WI-25260 DB Revision - Create And Populate Query To Group Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-031 WI-25260 DB Revision - Create And Populate Test to Test Set Map table
*/
PRINT ''Populating map.[tblTestDefinitionToTestSetDefinition]...''
TRUNCATE TABLE map.[tblTestDefinitionToTestSetDefinition]

INSERT INTO [map].[tblTestDefinitionToTestSetDefinition](TestDefinitionGuid,TestSetDefinitionGuid,DeleteFlag,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT pa.TestDefinitionGuid,ch.TestSetDefinitionGuid,lg.DeleteFlag,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM dbo.tblTestToTestSetMap lg
INNER JOIN tblTestDefinitions pa ON pa.[TestDefinitionIndex] = lg.TestDefinitionIndex
INNER JOIN tblTestSetDefinitions ch ON ch.[TestSetDefinitionIndex] = lg.TestSetDefinitionIndex

/*
	END OF SCRIPT 8.0.5.0-031 WI-25260 DB Revision - Create And Populate Test to Test Set Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-033 WI-25260 DB Revision - Create And PopulatePIDXProfile To Company Map table
*/
PRINT ''Populating map.[tblPIDXProfileToCompany]...''
TRUNCATE TABLE map.[tblPIDXProfileToCompany]

INSERT INTO [map].[tblPIDXProfileToCompany]([PIDXProfileGuid],[CompanyPersonnelToShipToBillToGuid],SiteGuid,SellerID,ShipperID,ConsigneeNumber,DenialOverride,UnavailableOverride,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	mf.PIDXProfileGuid,mt.CompanyPersonnelToShipToBillToGuid,si.SiteGuid,lg.SellerID,lg.ShipperID,lg.ConsigneeNumber,lg.DenialOverride,lg.UnavailableOverride,lg.CreatedDate,lg.CreatedBy,lg.CreatedDate,lg.CreatedBy
FROM	tblPIDXProfileCompanyMap lg
INNER JOIN tblPIDXProfiles mf ON mf.[Index]=lg.PIDXProfileIndex
INNER JOIN map.tblCompanyPersonnelToShipToBillTo mt ON mt._LegacyCompanyMapIndex = lg.LoadIDCompanyMapIndex
INNER JOIN tblSites si ON si.SiteIndex=lg.SiteIndex

/*
	END OF SCRIPT 8.0.5.0-033 WI-25260 DB Revision - Create And PopulatePIDXProfile To Company Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-034 WI-25260 DB Revision - Create And Allocation To Company Map tables
*/
PRINT ''Updating Allocation on Company map tables...''

UPDATE	a
SET		a.CompanyBillToToShipperGuid = m.CompanyBillToToShipperGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanyBillToToShipper m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 2

UPDATE	a
SET		a.CompanyLoadOwnerToManagerGuid = m.CompanyLoadOwnerToManagerGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanyLoadOwnerToManager m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 0

UPDATE	a
SET		a.CompanyOffLoadOwnerToManagerGuid = m.CompanyOffLoadOwnerToManagerGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanyOffLoadOwnerToManager m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 14

UPDATE	a
SET		a.CompanyShipperToOwnerGuid = m.CompanyShipperToOwnerGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanyShipperToOwner m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 1

UPDATE	a
SET		a.CompanyShipToToBillToGuid = m.CompanyShipToToBillToGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanyShipToToBillTo m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 3

UPDATE	a
SET		a.CompanySupplierToOwnerGuid = m.CompanySupplierToOwnerGuid
FROM	tblAllocations a
INNER	JOIN map.tblCompanySupplierToOwner m ON m._LegacyCompanyMapIndex = a.CompanyMapIndex
WHERE	a.CompanyMapType = 11

/*
	END OF SCRIPT 8.0.5.0-034 WI-25260 DB Revision - Create And Allocation To Company Map tables
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-035 WI-25260 DB Revision - Create And Populate Tank To Tank Map table
*/
PRINT ''Populating map.[tblTankToTankGroup]...''
TRUNCATE TABLE map.[tblTankToTankGroup]

INSERT INTO map.tblTankToTankGroup(
	TankGuid
,	AssignedToTankGroupGuid
,	CreatedDate
,	CreatedBy
,	UpdatedDate
,	UpdatedBy)
SELECT 	
	mf.TankGuid
,	mt.TankGroupGuid
,	mp.CreatedDate
,	mp.CreatedBy
,	mp.UpdatedDate
,	mp.UpdatedBy
FROM	tblTanks mf
INNER JOIN tblTankMap mp on mp.AssignedIndex = mf.TankIndex
INNER JOIN tblTankGroups mt on mt.[Index] = mp.AssignedToIndex

/*
	END OF SCRIPT 8.0.5.0-035 WI-25260 DB Revision - Create And Populate Tank To Tank Map table
*/


/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-300 WI-25309 DB Revision - Break up tblSchedules table
*/
PRINT ''Populating dbo.[tblScheduleTerminalOperation]...''
TRUNCATE TABLE dbo.[tblScheduleTerminalOperation]


INSERT INTO [tblScheduleTerminalOperation]([SiteGuid],[LookupDayOfWeekIndex],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	si.SiteGuid,tb.[Day],tb.[Enabled],tb.[OpeningTime],tb.[ClosingTime],tb.[EndOfDayEnabled],tb.[EndOfDayTime],tb.CreatedDate,tb.CreatedBy,tb.UpdatedDate,tb.UpdatedBy
FROM	tblSchedules tb
INNER JOIN tblSites si ON si.SiteIndex = tb.EntityIndex
WHERE tb.[Type] = 0

-- And the rest. FMD9 wants each site to have its operating schedules in table
INSERT INTO [dbo].[tblScheduleTerminalOperation]
(
		[ScheduleTerminalOperationGuid]
      ,[SiteGuid]
      ,[LookupDayOfWeekIndex]
      ,[Enabled]
      ,[OpeningTime]
      ,[ClosingTime]
      ,[EndOfDayEnabled]
      ,[EndOfDayTime]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
)
SELECT convert(uniqueidentifier, --ok
				hashbytes(''md5'',	
						(
						convert(varchar(36), s.[SiteGuid])+ --
						convert(varchar(36), o.[LookupDayOfWeekIndex])))) AS [ScheduleTerminalOperationGuid]
      ,s.[SiteGuid]
      ,o.[LookupDayOfWeekIndex]
      ,o.[Enabled]
      ,o.[OpeningTime]
      ,o.[ClosingTime]
      ,o.[EndOfDayEnabled]
      ,o.[EndOfDayTime]
      ,o.[CreatedDate]
      ,''V9 Upgrade. AAC'' AS [CreatedBy]
      ,o.[UpdatedDate]
      ,o.[UpdatedBy]
  FROM [dbo].[tblScheduleTerminalOperation] o, tblSites s 
  WHERE o.siteguid=''00000000-0000-0000-0000-000000000001''
  AND NOT EXISTS(SELECT TOP 1 1FROM [dbo].[tblScheduleTerminalOperation] WHERE s.siteguid=siteguid AND o.LookupDayOfWeekIndex=LookupDayOfWeekIndex)

PRINT ''Populating dbo.[tblScheduleCompanyAccess]...''
TRUNCATE TABLE dbo.[tblScheduleCompanyAccess]

INSERT INTO [tblScheduleCompanyAccess]([CompanyGuid],[LookupDayOfWeekIndex],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	sp.CompanyGuid,tb.[Day],tb.[Enabled],tb.[OpeningTime],tb.[ClosingTime],tb.[EndOfDayEnabled],tb.[EndOfDayTime],tb.CreatedDate,tb.CreatedBy,tb.UpdatedDate,tb.UpdatedBy
FROM	tblSchedules tb
INNER JOIN tblCompanies sp ON sp.CompanyIndex = tb.EntityIndex
WHERE tb.[Type] = 1


PRINT ''Populating dbo.[tblScheduleHoliday]...''
TRUNCATE TABLE dbo.[tblScheduleHoliday]


INSERT INTO [tblScheduleHoliday]([SiteGuid],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	si.SiteGuid,tb.[Enabled],tb.[OpeningTime],tb.[ClosingTime],tb.[EndOfDayEnabled],tb.[EndOfDayTime],tb.CreatedDate,tb.CreatedBy,tb.UpdatedDate,tb.UpdatedBy
FROM	tblSchedules tb
INNER JOIN tblSites si ON si.SiteIndex = tb.EntityIndex
WHERE tb.[Type] = 2


PRINT ''Populating dbo.[tblSchedulePersonnelAccess]...''
TRUNCATE TABLE dbo.[tblSchedulePersonnelAccess]

INSERT INTO [tblSchedulePersonnelAccess]([PersonnelGuid],[LookupDayOfWeekIndex],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
SELECT	si.PersonnelGuid,tb.[Day],tb.[Enabled],tb.[OpeningTime],tb.[ClosingTime],tb.[EndOfDayEnabled],tb.[EndOfDayTime],tb.CreatedDate,tb.CreatedBy,tb.UpdatedDate,tb.UpdatedBy
FROM	tblSchedules tb
INNER JOIN tblPersonnel si ON si.PersonIndex = tb.EntityIndex
WHERE tb.[Type] = 3

/*
	END OF SCRIPT 8.0.5.0-300 WI-25309 DB Revision - Break up tblSchedules table
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-6 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:51 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-6 Update Map and Split Tables', 
		@step_id=27, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*######################6666666666666666666666666666666666666666########################################################*/
/*
	START OF SCRIPT 8.0.5.0-301 WI-25309 DB Revision - Break up tblProcessVariable table
*/

PRINT ''Populating dbo.[tblProcessVariableTank]...''
TRUNCATE TABLE dbo.[tblProcessVariableTank]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableTank'')
	ALTER TABLE dbo.tblProcessVariableTank
	ADD _LegacyProcessVariableIndex INT NULL
GO


INSERT INTO tblProcessVariableTank(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[TankGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[TankGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN tblTanks tbl ON tbl.TankIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=1

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableTank
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableTank
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableTank
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)



PRINT ''Populating dbo.[tblProcessVariableLoadArm]...''
TRUNCATE TABLE dbo.[tblProcessVariableLoadArm]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableLoadArm'')
	ALTER TABLE dbo.tblProcessVariableLoadArm
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableLoadArm(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[LoadArmGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[LoadArmGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN tblLoadArms tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=2

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableLoadArm
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArm
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArm
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableStation]...''
TRUNCATE TABLE dbo.[tblProcessVariableStation]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableStation'')
	ALTER TABLE dbo.tblProcessVariableStation
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableStation(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[StationGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[StationGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN tblStations tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=3

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableStation
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStation
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStation
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableSite]...''
TRUNCATE TABLE dbo.[tblProcessVariableSite]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableSite'')
	ALTER TABLE dbo.tblProcessVariableSite
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableSite(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[SiteGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[SiteGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN tblSites tbl ON tbl.[SiteIndex]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=4

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableSite
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableSite
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableSite
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableComponentInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableComponentInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableComponentInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableComponentInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableComponentInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetComponentTankOrTankGroupGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetComponentTankOrTankGroupGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetComponentTankOrTankGroup tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=5

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableComponentInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableComponentInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableComponentInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)

PRINT ''Populating dbo.[tblProcessVariableComponentOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableComponentOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableComponentOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableComponentOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableComponentOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetComponentTankOrTankGroupGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetComponentTankOrTankGroupGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetComponentTankOrTankGroup tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=6

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableComponentOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableComponentOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableComponentOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableAdditiveInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableAdditiveInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableAdditiveInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableAdditiveInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableAdditiveInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetInjectorGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetInjectorGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetInjector tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=7

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableAdditiveInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableAdditiveInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableAdditiveInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableAdditiveOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableAdditiveOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableAdditiveOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableAdditiveOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableAdditiveOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetInjectorGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetInjectorGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetInjector tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=8

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableAdditiveOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableAdditiveOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableAdditiveOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableRecipeInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableRecipeInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableRecipeInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableRecipeInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableRecipeInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetRecipeGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetRecipeGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetRecipe tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=9

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableRecipeInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableRecipeInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableRecipeInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableRecipeOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableRecipeOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableRecipeOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableRecipeOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableRecipeOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetRecipeGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetRecipeGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetRecipe tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=10

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableRecipeOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableRecipeOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableRecipeOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-7 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-7 Update Map and Split Tables', 
		@step_id=28, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'--777777777777777777777777777777777777777777777777777777777777777777###########################################


PRINT ''Populating dbo.[tblProcessVariableLoadArmInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableLoadArmInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableLoadArmInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableLoadArmInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO


INSERT INTO tblProcessVariableLoadArmInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[LoadArmGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[LoadArmGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblLoadArms tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=11

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableLoadArmInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArmInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArmInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableLoadArmOutPutPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableLoadArmOutPutPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableLoadArmOutPutPermissive'')
	ALTER TABLE dbo.tblProcessVariableLoadArmOutPutPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableLoadArmOutPutPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[LoadArmGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[LoadArmGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblLoadArms tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=12

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableLoadArmOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArmOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableLoadArmOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableNoAdditiveInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableNoAdditiveInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableNoAdditiveInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableNoAdditiveInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO


INSERT INTO tblProcessVariableNoAdditiveInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[LoadArmGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[LoadArmGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblLoadArms tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=13

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableNoAdditiveInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableNoAdditiveInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableNoAdditiveInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableNoAdditiveOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableNoAdditiveOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableNoAdditiveOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableNoAdditiveOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableNoAdditiveOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[LoadArmGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[LoadArmGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblLoadArms tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=14

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableNoAdditiveOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableNoAdditiveOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableNoAdditiveOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableStationInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableStationInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableStationInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableStationInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableStationInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[StationGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[StationGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblStations tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=15

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableStationInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStationInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStationInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableStationOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableStationOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableStationOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableStationOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableStationOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[StationGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[StationGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblStations tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=16

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableStationOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStationOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableStationOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableExternalComponentBlendPercentage]...''
TRUNCATE TABLE dbo.[tblProcessVariableExternalComponentBlendPercentage]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableExternalComponentBlendPercentage'')
	ALTER TABLE dbo.tblProcessVariableExternalComponentBlendPercentage
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableExternalComponentBlendPercentage(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetExternalComponentGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetExternalComponentGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	192, -- Force 
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetExternalComponent tbl ON tbl.[_LegacyProductMapIndex]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=17

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableExternalComponentBlendPercentage
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentBlendPercentage
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentBlendPercentage
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableExternalComponentInputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableExternalComponentInputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableExternalComponentInputPermissive'')
	ALTER TABLE dbo.tblProcessVariableExternalComponentInputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableExternalComponentInputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetExternalComponentGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetExternalComponentGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetExternalComponent tbl ON tbl.[_LegacyProductMapIndex]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=18

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableExternalComponentInputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentInputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentInputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableExternalComponentOutputPermissive]...''
TRUNCATE TABLE dbo.[tblProcessVariableExternalComponentOutputPermissive]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableExternalComponentOutputPermissive'')
	ALTER TABLE dbo.tblProcessVariableExternalComponentOutputPermissive
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableExternalComponentOutputPermissive(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetExternalComponentGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetExternalComponentGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetExternalComponent tbl ON tbl.[_LegacyProductMapIndex]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=19

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableExternalComponentOutputPermissive
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentOutputPermissive
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableExternalComponentOutputPermissive
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableEquipment]...''
TRUNCATE TABLE dbo.[tblProcessVariableEquipment]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableEquipment'')
	ALTER TABLE dbo.tblProcessVariableEquipment
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableEquipment(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[EquipmentGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[EquipmentGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN dbo.tblEquipment tbl ON tbl.[Index]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=20

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableEquipment
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableEquipment
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableEquipment
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariablePresetInjector]...''
TRUNCATE TABLE dbo.[tblProcessVariablePresetInjector]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariablePresetInjector'')
	ALTER TABLE dbo.tblProcessVariablePresetInjector
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariablePresetInjector(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToPresetInjectorGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToPresetInjectorGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToPresetInjector tbl ON tbl.[_LegacyProductMapIndex]=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=17 -- unit type 17 used for process variables related to preset external components and preset injectors

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariablePresetInjector
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariablePresetInjector
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariablePresetInjector
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)


PRINT ''Populating dbo.[tblProcessVariableOffloadExternalMeter]...''
TRUNCATE TABLE dbo.[tblProcessVariableOffloadExternalMeter]

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyProcessVariableIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblProcessVariableOffloadExternalMeter'')
	ALTER TABLE dbo.tblProcessVariableOffloadExternalMeter
	ADD _LegacyProcessVariableIndex INT NULL
GO

INSERT INTO tblProcessVariableOffloadExternalMeter(
	[LookupProcessVariableTypeIndex],
	[InstanceNumber],
	[ProductToOffloadExternalMeterGuid],
	[OPCConnectionGuid],
	[OPCItemID],
	[DataType],
	[ServerEngineeringUnitsIndex],
	[Quality],
	[SIValue],
	[DateTimeStamp],
	[Maximum],
	[Minimum],
	[DataTypeEnabled],
	[Input],
	[InputEnabled],
	[MessageApplicationStringGuid],
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy,
	LookupSIValueVariantTypeIndex,
	LookupMaximumVariantTypeIndex,
	LookupMinimumVariantTypeIndex,
	_LegacyProcessVariableIndex
)
SELECT 
	prv.[ProcessVariableType],
	prv.[InstanceNumber],
	tbl.[ProductToOffloadExternalMeterGuid],
	opc.[OPCConnectionGuid],
	prv.[OPCItemID],
	prv.[DataType],
	prv.[ServerEngineeringUnitsIndex],
	prv.[Quality],
	CAST(prv.[SIValue] AS VARBINARY(max)),
	prv.[DateTimeStamp],
	CAST(prv.[Maximum] AS VARBINARY(max)),
	CAST(prv.[Minimum] AS VARBINARY(max)),
	prv.[DataTypeEnabled],
	prv.[Input],
	prv.[InputEnabled],
	aps.[ApplicationStringGuid],
	prv.CreatedDate,
	prv.CreatedBy,
	prv.UpdatedDate,
	prv.UpdatedBy,
	CASE WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''smallint'')) THEN 3 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''float'')) THEN 10 
		 WHEN (prv.[SIValue] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[SIValue], ''BaseType'') = ''nvarchar'')) THEN 9 
		 ELSE NULL 
	END AS ''LookupSIValueVariantTypeIndex'',
	CASE WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Maximum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Maximum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL
	END AS ''LookupMaximumVariantTypeIndex'',
	CASE WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''smallint'')) THEN 3
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''float'')) THEN 10
		 WHEN (prv.[Minimum] IS NOT NULL AND (SQL_VARIANT_PROPERTY(prv.[Minimum], ''BaseType'') = ''nvarchar'')) THEN 9
		 ELSE NULL 
	END AS ''LookupMinimumVariantTypeIndex'',
	prv.[index]

FROM tblProcessVariables prv
INNER JOIN map.tblProductToOffloadExternalMeter tbl ON tbl._LegacyProductMapIndex=prv.UnitIndex
LEFT JOIN tblOPCConnections opc ON opc.[Index]=prv.OPCConnectionIndex
LEFT JOIN tblApplicationString aps ON aps.[Index]=prv.MessageIndex
WHERE prv.UnitType=17

/* Reverse the bit order for smallint and float values because sql_variant stores the values
   in big-endian format, but the FuelsManager code, using System.BitConverter, expects little-endian
*/
UPDATE tblProcessVariableOffloadExternalMeter
SET SIValue = cast(reverse(cast(SIValue as varchar(max))) as varbinary(max))
WHERE LookupSIValueVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableOffloadExternalMeter
SET Maximum = cast(reverse(cast(Maximum as varchar(max))) as varbinary(max))
WHERE LookupMaximumVariantTypeIndex IN (3, 10)

UPDATE tblProcessVariableOffloadExternalMeter
SET Minimum = cast(reverse(cast(Minimum as varchar(max))) as varbinary(max))
WHERE LookupMinimumVariantTypeIndex IN (3, 10)

/*
	END OF SCRIPT 8.0.5.0-301 WI-25309 DB Revision - Break up tblProcessVariable table
*/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-8 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-8 Update Map and Split Tables', 
		@step_id=29, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*###########8888888888888888888888888888888888888888888888####################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-302 WI-25309 DB Revision - Split Tansaction Alias Status Map table
*/

PRINT ''Populating map.[tblTransactionAliasToStatus]...''
TRUNCATE TABLE map.[tblTransactionAliasToStatus]

INSERT INTO map.tblTransactionAliasToStatus(
	TransactionAliasGuid
,	LookupTransactionStatusIndex
)

SELECT
	mf.TransactionAliasGuid
,	mp.TransactionStatus
FROM dbo.tblTransactionAliasStatusesMap mp
INNER JOIN tblTransactionAliases mf ON mf.AliasId = mp.AliasID

/*
	END OF SCRIPT 8.0.5.0-302 WI-25309 DB Revision - Split Tansaction Alias Status Map table
*/
/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-303 WI-25309 DB Revision - Break up tblAppointments
	
*/

PRINT ''Populating dbo.[tblAppointmentTank]...''
TRUNCATE TABLE dbo.tblAppointmentTank

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAppointmentIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblAppointmentTank'')
	ALTER TABLE dbo.tblAppointmentTank
	ADD _LegacyAppointmentIndex INT NULL
GO

INSERT INTO tblAppointmentTank
           ([TankGuid]
           ,[TestSetDefinitionGuid]
           ,[SiteGuid]
           ,[AssetText]
           ,[AppointmentCategory]
           ,[AppointmentIsSingle]
           ,[ScheduleOnWeekends]
           ,[ScheduleOnHolidays]
           ,[StartDate]
           ,[Duration]
           ,[AppointmentPeriod]
           ,[AppointmentPeriodText]
           ,[Description]
           ,[AppointmentTimeInterval]
           ,[AppointmentDayOfTheWeekText]
           ,[AppointmentDayOfTheWeek]
           ,[AppointmentReoccuranceInterval]
           ,[AppointmentOption2Selected]
           ,[AppointmentTimeOptionSelectionText]
           ,[AppointmentTimeOptionSelection]
           ,[AppointmentMonthSelectionText]
           ,[AppointmentMonthSelection]
           ,[AppointmentDayOfTheMonth]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
           ,[_LegacyAppointmentIndex]
           )
SELECT	tblTanks.[TankGuid]
		,tblTestSetDefinitions.[TestSetDefinitionGuid]
		,tblSites.[SiteGuid]
		,tblAppointments.[AssetText]
		,tblAppointments.[AppointmentCategory]
		,tblAppointments.[AppointmentIsSingle]
		,tblAppointments.[ScheduleOnWeekends]
		,tblAppointments.[ScheduleOnHolidays]
		,tblAppointments.[StartDate]
		,tblAppointments.[Duration]
		,tblAppointments.[AppointmentPeriod]
		,tblAppointments.[AppointmentPeriodText]
		,tblAppointments.[Description]
		,tblAppointments.[AppointmentTimeInterval]
		,tblAppointments.[AppointmentDayOfTheWeekText]
		,tblAppointments.[AppointmentDayOfTheWeek]
		,tblAppointments.[AppointmentReoccuranceInterval]
		,tblAppointments.[AppointmentOption2Selected]
		,tblAppointments.[AppointmentTimeOptionSelectionText]
		,tblAppointments.[AppointmentTimeOptionSelection]
		,tblAppointments.[AppointmentMonthSelectionText]
		,tblAppointments.[AppointmentMonthSelection]
		,tblAppointments.[AppointmentDayOfTheMonth]
		,tblAppointments.[CreatedDate]
		,tblAppointments.[CreatedBy]
		,tblAppointments.[UpdatedDate]
		,tblAppointments.[UpdatedBy]
		,tblAppointments.[Index]
FROM tblAppointments 
INNER JOIN tblTanks ON tblTanks.TankIndex = tblAppointments.AssociatedTypeIndex
LEFT OUTER JOIN tblTestSetDefinitions ON tblTestSetDefinitions.TestSetDefinitionIndex = tblAppointments.TestSetIndex
INNER JOIN tblSites ON tblSites.SiteIndex=tblAppointments.SiteIndex
WHERE tblAppointments.AssociatedType = ''Tanks''


PRINT ''Populating dbo.[tblAppointmentEquipment]...''
TRUNCATE TABLE dbo.tblAppointmentEquipment

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAppointmentIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblAppointmentEquipment'')
	ALTER TABLE dbo.tblAppointmentEquipment
	ADD _LegacyAppointmentIndex INT NULL
GO

INSERT INTO tblAppointmentEquipment
           ([EquipmentGuid]
           ,[TestSetDefinitionGuid]
           ,[SiteGuid]
           ,[AssetText]
           ,[AppointmentCategory]
           ,[AppointmentIsSingle]
           ,[ScheduleOnWeekends]
           ,[ScheduleOnHolidays]
           ,[StartDate]
           ,[Duration]
           ,[AppointmentPeriod]
           ,[AppointmentPeriodText]
           ,[Description]
           ,[AppointmentTimeInterval]
           ,[AppointmentDayOfTheWeekText]
           ,[AppointmentDayOfTheWeek]
           ,[AppointmentReoccuranceInterval]
           ,[AppointmentOption2Selected]
           ,[AppointmentTimeOptionSelectionText]
           ,[AppointmentTimeOptionSelection]
           ,[AppointmentMonthSelectionText]
           ,[AppointmentMonthSelection]
           ,[AppointmentDayOfTheMonth]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
           ,[_LegacyAppointmentIndex]
           )
SELECT	tblEquipment.[EquipmentGuid]
		,tblTestSetDefinitions.[TestSetDefinitionGuid]
		,tblSites.[SiteGuid]
		,tblAppointments.[AssetText]
		,tblAppointments.[AppointmentCategory]
		,tblAppointments.[AppointmentIsSingle]
		,tblAppointments.[ScheduleOnWeekends]
		,tblAppointments.[ScheduleOnHolidays]
		,tblAppointments.[StartDate]
		,tblAppointments.[Duration]
		,tblAppointments.[AppointmentPeriod]
		,tblAppointments.[AppointmentPeriodText]
		,tblAppointments.[Description]
		,tblAppointments.[AppointmentTimeInterval]
		,tblAppointments.[AppointmentDayOfTheWeekText]
		,tblAppointments.[AppointmentDayOfTheWeek]
		,tblAppointments.[AppointmentReoccuranceInterval]
		,tblAppointments.[AppointmentOption2Selected]
		,tblAppointments.[AppointmentTimeOptionSelectionText]
		,tblAppointments.[AppointmentTimeOptionSelection]
		,tblAppointments.[AppointmentMonthSelectionText]
		,tblAppointments.[AppointmentMonthSelection]
		,tblAppointments.[AppointmentDayOfTheMonth]
		,tblAppointments.[CreatedDate]
		,tblAppointments.[CreatedBy]
		,tblAppointments.[UpdatedDate]
		,tblAppointments.[UpdatedBy]
		,tblAppointments.[Index]
FROM tblAppointments 
INNER JOIN tblEquipment ON tblEquipment.[Index] = tblAppointments.AssociatedTypeIndex
LEFT OUTER JOIN tblTestSetDefinitions ON tblTestSetDefinitions.TestSetDefinitionIndex = tblAppointments.TestSetIndex
INNER JOIN tblSites ON tblSites.SiteIndex=tblAppointments.SiteIndex
WHERE tblAppointments.AssociatedType = ''Equipment''

PRINT ''Populating dbo.[tblAppointmentPersonnel]...''
TRUNCATE TABLE dbo.tblAppointmentPersonnel

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyAppointmentIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblAppointmentPersonnel'')
	ALTER TABLE dbo.tblAppointmentPersonnel
	ADD _LegacyAppointmentIndex INT NULL
GO

INSERT INTO tblAppointmentPersonnel
           ([PersonnelGuid]
           ,[TestSetDefinitionGuid]
           ,[SiteGuid]
           ,[AssetText]
           ,[AppointmentCategory]
           ,[AppointmentIsSingle]
           ,[ScheduleOnWeekends]
           ,[ScheduleOnHolidays]
           ,[StartDate]
           ,[Duration]
           ,[AppointmentPeriod]
           ,[AppointmentPeriodText]
           ,[Description]
           ,[AppointmentTimeInterval]
           ,[AppointmentDayOfTheWeekText]
           ,[AppointmentDayOfTheWeek]
           ,[AppointmentReoccuranceInterval]
           ,[AppointmentOption2Selected]
           ,[AppointmentTimeOptionSelectionText]
           ,[AppointmentTimeOptionSelection]
           ,[AppointmentMonthSelectionText]
           ,[AppointmentMonthSelection]
           ,[AppointmentDayOfTheMonth]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
           ,[_LegacyAppointmentIndex]
           )
SELECT	tblPersonnel.[PersonnelGuid]
		,tblTestSetDefinitions.[TestSetDefinitionGuid]
		,tblSites.[SiteGuid]
		,tblAppointments.[AssetText]
		,tblAppointments.[AppointmentCategory]
		,tblAppointments.[AppointmentIsSingle]
		,tblAppointments.[ScheduleOnWeekends]
		,tblAppointments.[ScheduleOnHolidays]
		,tblAppointments.[StartDate]
		,tblAppointments.[Duration]
		,tblAppointments.[AppointmentPeriod]
		,tblAppointments.[AppointmentPeriodText]
		,tblAppointments.[Description]
		,tblAppointments.[AppointmentTimeInterval]
		,tblAppointments.[AppointmentDayOfTheWeekText]
		,tblAppointments.[AppointmentDayOfTheWeek]
		,tblAppointments.[AppointmentReoccuranceInterval]
		,tblAppointments.[AppointmentOption2Selected]
		,tblAppointments.[AppointmentTimeOptionSelectionText]
		,tblAppointments.[AppointmentTimeOptionSelection]
		,tblAppointments.[AppointmentMonthSelectionText]
		,tblAppointments.[AppointmentMonthSelection]
		,tblAppointments.[AppointmentDayOfTheMonth]
		,tblAppointments.[CreatedDate]
		,tblAppointments.[CreatedBy]
		,tblAppointments.[UpdatedDate]
		,tblAppointments.[UpdatedBy]
		,tblAppointments.[Index]
FROM tblAppointments 
INNER JOIN tblPersonnel ON tblPersonnel.[PersonIndex] = tblAppointments.AssociatedTypeIndex
LEFT OUTER JOIN tblTestSetDefinitions ON tblTestSetDefinitions.TestSetDefinitionIndex = tblAppointments.TestSetIndex
INNER JOIN tblSites ON tblSites.SiteIndex=tblAppointments.SiteIndex
WHERE tblAppointments.AssociatedType = ''Personnel''

PRINT ''Populating map.[tblEntityAppointmentEquipmentToSite]...''
TRUNCATE TABLE map.tblEntityAppointmentEquipmentToSite

UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentEquipmentToSite] a
INNER JOIN tblAppointmentEquipment b
ON b.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid
WHERE a.AssignedFromSiteGuid IS NULL

--// AssignedFromSiteGuid IS NOT NULL column; a temporary GUID of ''00000000-0000-0000-0000-000000000000'' is used until a final section on the script can perform an update once all split and mapping tables are in place
INSERT INTO map.tblEntityAppointmentEquipmentToSite(AppointmentEquipmentGuid,SiteGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,AssignedFromSiteGuid) 
SELECT	ent.AppointmentEquipmentGuid, s.SiteGuid, m.CreatedDate,m.CreatedBy,m.CreatedDate,m.CreatedBy,''00000000-0000-0000-0000-000000000000''
FROM tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.[SiteIndex] = m.[SiteIndex] 
INNER JOIN tblAppointments ON tblAppointments.[Index] = m.[Index]
INNER JOIN tblAppointmentEquipment ent ON ent._legacyAppointmentIndex=tblAppointments.[Index]
WHERE	m.TypeID = ''Appointment'' 

PRINT ''Populating map.[tblEntityAppointmentPersonnelToSite]...''
TRUNCATE TABLE map.tblEntityAppointmentPersonnelToSite

UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentPersonnelToSite] a
INNER JOIN tblAppointmentPersonnel b
ON b.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid
WHERE a.AssignedFromSiteGuid IS NULL

--// AssignedFromSiteGuid IS NOT NULL column; a temporary GUID of ''00000000-0000-0000-0000-000000000000'' is used until a final section on the script can perform an update once all split and mapping tables are in place
INSERT INTO map.tblEntityAppointmentPersonnelToSite(AppointmentPersonnelGuid,SiteGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,AssignedFromSiteGuid) 
SELECT	ent.AppointmentPersonnelGuid, s.SiteGuid, m.CreatedDate,m.CreatedBy,m.CreatedDate,m.CreatedBy,''00000000-0000-0000-0000-000000000000''
FROM tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.[SiteIndex] = m.[SiteIndex] 
INNER JOIN tblAppointments ON tblAppointments.[Index] = m.[Index]
INNER JOIN tblAppointmentPersonnel ent ON ent._legacyAppointmentIndex=tblAppointments.[Index]
WHERE	m.TypeID = ''Appointment'' 


PRINT ''Populating map.[tblEntityAppointmentTankToSite]...''
TRUNCATE TABLE map.tblEntityAppointmentTankToSite

INSERT INTO map.tblEntityAppointmentTankToSite(AppointmentTankGuid,SiteGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
SELECT	ent.AppointmentTankGuid, s.SiteGuid, m.CreatedDate,m.CreatedBy,m.CreatedDate,m.CreatedBy
FROM tblEntityToSiteMap m 
INNER JOIN tblSites s ON s.[SiteIndex] = m.[SiteIndex] 
INNER JOIN tblAppointments ON tblAppointments.[Index] = m.[Index]
INNER JOIN tblAppointmentTank ent ON ent._legacyAppointmentIndex=tblAppointments.[Index]
WHERE	m.TypeID = ''Appointment'' 

/*
	END OF SCRIPT 8.0.5.0-304 WI-26350 DB Revision - Replace StorageLocationIndex On tblTransactionLineItems
*/

/*###############################################################################################*/
/*
	START OF SCRIPT 8.0.5.0-353 WI-25295 DB Revision - Split tblUserDataField and tblUserDataListValues
	
*/

PRINT ''Populating dbo.[tblUserDataFieldFuelCard]...''
TRUNCATE TABLE dbo.tblUserDataFieldFuelCard

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldFuelCard'')
	ALTER TABLE dbo.tblUserDataFieldFuelCard
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldFuelCard(
	    UserDataFieldFuelCardGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Fuel Card''

PRINT ''Populating dbo.[tblUserDataListValueFuelCard]...''
TRUNCATE TABLE dbo.tblUserDataListValueFuelCard

INSERT INTO [dbo].[tblUserDataListValueFuelCard](
        UserDataListValueFuelCardGuid
	,   UserDataFieldFuelCardGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldFuelCardGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldFuelCard] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldCompany]...''
TRUNCATE TABLE dbo.tblUserDataFieldCompany


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldCompany'')
	ALTER TABLE dbo.tblUserDataFieldCompany
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldCompany(
	    UserDataFieldCompanyGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Companies''

PRINT ''Populating dbo.[tblUserDataListValueCompany]...''
TRUNCATE TABLE dbo.tblUserDataListValueCompany

INSERT INTO [dbo].[tblUserDataListValueCompany](
	    UserDataListValueCompanyGuid
	,	UserDataFieldCompanyGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldCompanyGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldCompany] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldEquipment]...''
TRUNCATE TABLE dbo.tblUserDataFieldEquipment

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldEquipment'')
	ALTER TABLE dbo.tblUserDataFieldEquipment
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldEquipment(
	    UserDataFieldEquipmentGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Equipment''

PRINT ''Populating dbo.[tblUserDataListValueEquipment]...''
TRUNCATE TABLE dbo.tblUserDataListValueEquipment

INSERT INTO [dbo].[tblUserDataListValueEquipment](
	    UserDataListValueEquipmentGuid
	,   UserDataFieldEquipmentGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldEquipmentGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldEquipment] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldPersonnel]...''
TRUNCATE TABLE dbo.tblUserDataFieldPersonnel

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldPersonnel'')
	ALTER TABLE dbo.tblUserDataFieldPersonnel
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldPersonnel(
	    UserDataFieldPersonnelGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Personnel''

PRINT ''Populating dbo.[tblUserDataListValuePersonnel]...''
TRUNCATE TABLE dbo.tblUserDataListValuePersonnel


INSERT INTO [dbo].[tblUserDataListValuePersonnel](
        UserDataListValuePersonnelGuid
	,   UserDataFieldPersonnelGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldPersonnelGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldPersonnel] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldProduct]...''
TRUNCATE TABLE dbo.tblUserDataFieldProduct

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldProduct'')
	ALTER TABLE dbo.tblUserDataFieldProduct
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldProduct(
	    UserDataFieldProductGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Products''

PRINT ''Populating dbo.[tblUserDataListValueProduct]...''
TRUNCATE TABLE dbo.tblUserDataListValueProduct


INSERT INTO [dbo].[tblUserDataListValueProduct](
	    UserDataListValueProductGuid
	,   UserDataFieldProductGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldProductGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldProduct] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 


PRINT ''Populating dbo.[tblUserDataFieldSite]...''
TRUNCATE TABLE dbo.tblUserDataFieldSite


IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldSite'')
	ALTER TABLE dbo.tblUserDataFieldSite
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldSite(
	    UserDataFieldSiteGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	--INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Sites''


PRINT ''Populating dbo.[tblUserDataListValueSite]...''
TRUNCATE TABLE dbo.tblUserDataListValueSite


INSERT INTO [dbo].[tblUserDataListValueSite](
	    UserDataListValueSiteGuid
	,   UserDataFieldSiteGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
	,   fil.UserDataFieldSiteGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldSite] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldTransactionAlias]...''
TRUNCATE TABLE dbo.tblUserDataFieldTransactionAlias

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldTransactionAlias'')
	ALTER TABLE dbo.tblUserDataFieldTransactionAlias
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldTransactionAlias(
	    UserDataFieldTransactionAliasGuid
	,   SiteGuid
	,	TransactionAliasGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	al.TransactionAliasGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Transaction Aliases''

PRINT ''Populating dbo.[tblUserDataListValueTransactionAlias]...''
TRUNCATE TABLE dbo.tblUserDataListValueTransactionAlias

INSERT INTO [dbo].[tblUserDataListValueTransactionAlias](
	    UserDataListValueTransactionAliasGuid
	,   UserDataFieldTransactionAliasGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
    ,   fil.UserDataFieldTransactionAliasGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldTransactionAlias] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 

PRINT ''Populating dbo.[tblUserDataFieldTransactionAliasLineItem]...''
TRUNCATE TABLE dbo.tblUserDataFieldTransactionAliasLineItem

IF NOT EXISTS(	SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=''_LegacyIndex'' 
				AND TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblUserDataFieldTransactionAliasLineItem'')
	ALTER TABLE dbo.tblUserDataFieldTransactionAliasLineItem
	ADD _LegacyIndex INT NULL
GO

	INSERT INTO tblUserDataFieldTransactionAliasLineItem(
	    UserDataFieldTransactionAliasLineItemGuid
	,   SiteGuid
	,	TransactionAliasGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	[UserGroupGuid]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	,	_LegacyIndex)
	SELECT
	    NEWID()
	,   si.SiteGuid
	,	al.TransactionAliasGuid
	,	ud.[Number]
	,	ud.DisplayOrder
	,	ud.DisplayName
	,	ud.[Type]
	,	gr.[GroupGuid]
	,	ud.CreatedDate
	,	ud.CreatedBy
	,	ud.UpdatedDate
	,	ud.UpdatedBy
	,	ud.[Index]
	FROM	tblUserDataFields ud
	INNER JOIN tblSites si ON si.SiteIndex=ud.SiteIndex
	INNER JOIN tblTransactionAliases al ON al.AliasID=ud.AliasID
	LEFT  JOIN tblGroups gr ON gr.GroupIndex=ud.UserGroupIndex
	WHERE EntityTypeID= ''Transaction Alias Line Item''

DECLARE @DisplayOrderForUserData23 int 
DECLARE @DisplayOrderForUserData24 int

IF NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem  f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
			WHERE AliasName=''Sale'' AND [Number]=22)
BEGIN
	SET @DisplayOrderForUserData23  = (SELECT DisplayOrder FROM 
		tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
		WHERE DisplayName=''EBS Process Date'' AND AliasName=''Sale'')
	IF @DisplayOrderForUserData23 IS NULL
		SELECT @DisplayOrderForUserData23 = MAX(DisplayOrder) + 1  FROM (
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'')
			UNION
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'')
			UNION
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'')
			) A
	UPDATE f SET DisplayOrder += 1 FROM tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData23
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData23
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData23
	INSERT INTO tblUserDataFieldTransactionAliasLineItem(
			UserDataFieldTransactionAliasLineItemGuid
		,   SiteGuid
		,	TransactionAliasGuid
		,	[Number]
		,	DisplayOrder
		,	DisplayName
		,	[LookupUserDataTypeIndex]
		,	[UserGroupGuid]
		,	CreatedDate
		,	CreatedBy
		,	UpdatedDate
		,	UpdatedBy
		)
	SELECT
		convert(uniqueidentifier, --ok
		hashbytes(''md5'',(convert(varchar(36),SiteGuid)+
							convert(varchar(36), TransactionAliasGuid)+
								''Customer Name''))),
		SiteGuid,
		TransactionAliasGuid,
		22,
		@DisplayOrderForUserData23,
		''Customer Name'',
		0,
		NULL,
		GETUTCDATE(),
		''Administrator'',
		GETUTCDATE(),
		''Administrator''
	FROM tblTransactionAliases a WHERE aliasname = ''Sale''
	AND NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem WHERE 
		a.TransactionAliasGuid=TransactionAliasGuid AND [Number]=22)
		
END


IF NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem  f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
			WHERE AliasName=''Sale'' AND [Number]=23)
BEGIN
	SET @DisplayOrderForUserData24  = (SELECT DisplayOrder FROM 
		tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
		WHERE DisplayName=''EBS Status Code'' AND AliasName=''Sale'')


	IF @DisplayOrderForUserData24 IS NULL
		SET @DisplayOrderForUserData24 = @DisplayOrderForUserData23
	


	UPDATE f SET DisplayOrder += 1 FROM tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData24
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData24
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Sale'' AND DisplayOrder >= @DisplayOrderForUserData24


	INSERT INTO tblUserDataFieldTransactionAliasLineItem(
			UserDataFieldTransactionAliasLineItemGuid
		,   SiteGuid
		,	TransactionAliasGuid
		,	[Number]
		,	DisplayOrder
		,	DisplayName
		,	[LookupUserDataTypeIndex]
		,	[UserGroupGuid]
		,	CreatedDate
		,	CreatedBy
		,	UpdatedDate
		,	UpdatedBy
		)
	SELECT
		convert(uniqueidentifier, --ok
		hashbytes(''md5'',(convert(varchar(36),SiteGuid)+
							convert(varchar(36), TransactionAliasGuid)+
								''EDI-PI''))),
		SiteGuid,
		TransactionAliasGuid,
		23,
		@DisplayOrderForUserData24,
		''EDI-PI'',
		0,
		NULL,
		GETUTCDATE(),
		''Administrator'',
		GETUTCDATE(),
		''Administrator''
	FROM tblTransactionAliases a WHERE aliasname = ''Sale''
	AND NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem WHERE 
		a.TransactionAliasGuid=TransactionAliasGuid AND [Number]=23)
END


IF NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem  f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
			WHERE AliasName=''Defuel'' AND [Number]=22)
BEGIN
	SET @DisplayOrderForUserData23  = (SELECT DisplayOrder FROM 
		tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
		WHERE DisplayName=''EBS Process Date'' AND AliasName=''Defuel'')

	IF @DisplayOrderForUserData23 IS NULL
		SELECT @DisplayOrderForUserData23 = MAX(DisplayOrder) + 1  FROM (
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'')
			UNION
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'')
			UNION
			(SELECT MAX(DisplayOrder) as DisplayOrder FROM 
				tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'')
			) A


	UPDATE f SET DisplayOrder += 1 FROM tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData23
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData23
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData23

	INSERT INTO tblUserDataFieldTransactionAliasLineItem(
			UserDataFieldTransactionAliasLineItemGuid
		,   SiteGuid
		,	TransactionAliasGuid
		,	[Number]
		,	DisplayOrder
		,	DisplayName
		,	[LookupUserDataTypeIndex]
		,	[UserGroupGuid]
		,	CreatedDate
		,	CreatedBy
		,	UpdatedDate
		,	UpdatedBy
		)
	SELECT
		convert(uniqueidentifier, --ok
		hashbytes(''md5'',(convert(varchar(36),SiteGuid)+
							convert(varchar(36), TransactionAliasGuid)+
								''Customer Name''))),
		SiteGuid,
		TransactionAliasGuid,
		22,
		@DisplayOrderForUserData23,
		''Customer Name'',
		0,
		NULL,
		GETUTCDATE(),
		''Administrator'',
		GETUTCDATE(),
		''Administrator''
	FROM tblTransactionAliases a WHERE aliasname = ''Defuel''
	AND NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem WHERE 
		a.TransactionAliasGuid=TransactionAliasGuid AND [Number]=22)

		END

IF NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem  f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
			WHERE AliasName=''Defuel'' AND [Number]=23)
BEGIN

	SET @DisplayOrderForUserData24  = (SELECT DisplayOrder FROM 
		tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
		WHERE DisplayName=''EBS Status Code'' AND AliasName=''Defuel'')
	IF @DisplayOrderForUserData24 IS NULL
		SET @DisplayOrderForUserData24 = @DisplayOrderForUserData23
	
	UPDATE f SET DisplayOrder += 1 FROM tblTransactionAliasFields f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData24
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAlias f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData24
	UPDATE f SET DisplayOrder += 1 FROM tblUserDataFieldTransactionAliasLineItem f JOIN tblTransactionAliases a ON f.TransactionAliasGuid=a.TransactionAliasGuid 
				WHERE AliasName=''Defuel'' AND DisplayOrder >= @DisplayOrderForUserData24

	INSERT INTO tblUserDataFieldTransactionAliasLineItem(
			UserDataFieldTransactionAliasLineItemGuid
		,   SiteGuid
		,	TransactionAliasGuid
		,	[Number]
		,	DisplayOrder
		,	DisplayName
		,	[LookupUserDataTypeIndex]
		,	[UserGroupGuid]
		,	CreatedDate
		,	CreatedBy
		,	UpdatedDate
		,	UpdatedBy
		)
	SELECT
		convert(uniqueidentifier, --ok
		hashbytes(''md5'',(convert(varchar(36),SiteGuid)+
							convert(varchar(36), TransactionAliasGuid)+
								''EDI-PI''))),
		SiteGuid,
		TransactionAliasGuid,
		23,
		@DisplayOrderForUserData24,
		''EDI-PI'',
		0,
		NULL,
		GETUTCDATE(),
		''Administrator'',
		GETUTCDATE(),
		''Administrator''
	FROM tblTransactionAliases a WHERE aliasname = ''Defuel''
	AND NOT EXISTS(SELECT TOP 1 1 FROM tblUserDataFieldTransactionAliasLineItem WHERE 
		a.TransactionAliasGuid=TransactionAliasGuid AND [Number]=23)
END

UPDATE u SET [DisplayName]=''Customer Name'', UserGroupGuid=null FROM tblUserDataFieldTransactionAliasLineItem u 
	JOIN tblTransactionAliases a ON u.TransactionAliasGuid=a.TransactionAliasGuid
	WHERE [Number]=22 AND AliasName IN (''Sale'',''Defuel'')
UPDATE u SET [DisplayName]=''EDI-PI'', UserGroupGuid=null FROM tblUserDataFieldTransactionAliasLineItem u 
	JOIN tblTransactionAliases a ON u.TransactionAliasGuid=a.TransactionAliasGuid
	WHERE [Number]=23 AND AliasName IN (''Sale'',''Defuel'')

PRINT ''Populating dbo.[tblUserDataListValueTransactionAliasLineItem]...''
TRUNCATE TABLE dbo.tblUserDataListValueTransactionAliasLineItem


INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem](
	    UserDataListValueTransactionAliasLineItemGuid
	,   UserDataFieldTransactionAliasLineItemGuid
	,	[Value]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy)
	SELECT NEWID()
    ,   fil.UserDataFieldTransactionAliasLineItemGuid
	,	val.Value
	,	val.CreatedDate
	,	val.CreatedBy
	,	val.UpdatedDate
	,	val.UpdatedBy
	FROM	[dbo].[tblUserDataFieldTransactionAliasLineItem] fil
	INNER JOIN dbo.tblUserDataListValues val ON val.[UserDataFieldIndex]=fil._LegacyIndex 


-- 8.0.5.0-355 WI-25295 DB Revision - Replace LoadingLocationIndex On TransactionLineItems
----PRINT ''Updating LoadingLocationStationGuid on tblTransactionLineItems...''

----UPDATE a
----SET a.LoadingLocationStationGuid=b.StationGuid
----FROM tblTransactionLineItems a
----INNER JOIN tblStations b ON b.[Index]=a.LoadingLocationIndex

-- 8.0.5.0-360 WI-25701 DB Revision - Create and Popluate tblTransactionAliases.AssociatedAliasGuid
PRINT ''Updating AssociatedTransactionAliasGuid on tblTransactionAliases...''
UPDATE dbo.tblTransactionAliases
SET AssociatedTransactionAliasGuid =
	(SELECT AssocAlias.TransactionAliasGuid
	   FROM dbo.tblTransactionAliases AssocAlias
	  WHERE tblTransactionAliases.AssociatedAliasID = AssocAlias.AliasID)
WHERE AssociatedAliasID IS NOT NULL

------ 8.0.5.0-372 WI-26100 DB Revision - Convert CarrierIndex column on tblTransactions to CarrierCompanyGuid
----PRINT ''Updating CarrierCompanyGuid on tblTransactions...''

----UPDATE t
----SET	t.CarrierCompanyGuid=c.CompanyGuid
----FROM tblTransactions t
----INNER JOIN tblCompanies c ON c.CompanyIndex=t.CarrierIndex


/*******Vivian commented out for review - column AggregateField does not exist in dbo.tblLedgerAggregateColumns in consolidateddb
-- 8.0.5.0-374 WI-25701 DB Revision - Create and Popluate tblLedgerAggregateColumns.LookupAggregateFieldIndex
PRINT ''Updating LookupAggregateFieldIndex on tblLedgerAggregateColumns...''

UPDATE dbo.tblLedgerAggregateColumns
   SET LookupAggregateFieldIndex = AggregateField

*********/


---- 8.0.5.0-376 WI-26099 DB Revision - Modify tblTransactionLinks
--PRINT ''Updating TransactionLineItemGuid on tblTransactionLinks...''

--UPDATE a
--SET		a.TransactionLineItemGuid=b.TransactionLineItemGuid
--FROM tblTransactionLinks a
--INNER JOIN tblTransactionLineItems b ON b.TransLineItemID=a.LinkedLineItemIndex
--GO

PRINT ''Updating LocationIATAGuid on tblStandingOffers...''

UPDATE a
SET a.LocationIATAGuid=b.IATAGuid
FROM dbo.tblStandingOffers a
INNER JOIN tblIATA b ON b.IATAIndex=a.LocationIndex

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-9 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-9 Update Map and Split Tables', 
		@step_id=30, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'--9999999999999999999999999999999999999999999999999999##############################################3

-- 8.0.5.0-1056 WI-33412 Site To Site Map - Modify Table and usp_UpdateDB
PRINT ''Populating  map.tblSiteToSite...''
INSERT INTO map.tblSiteToSite(ParentSiteGuid,ChildSiteGuid)
SELECT ''00000000-0000-0000-0000-000000000001'',''00000000-0000-0000-0000-000000000001''


-- 8.0.5.15-0001 WI-35845 Add DispatchField column to tblTransactionAliasFields
IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=''dbo'' AND TABLE_NAME=''tblTransactionAliasFields'' AND COLUMN_NAME=''DispatchField'')
	ALTER TABLE [dbo].[tblTransactionAliasFields] 
	ADD [DispatchField] bit NULL CONSTRAINT DF_tblTransactionAliasFields_DispatchField DEFAULT 0
GO
	

UPDATE [dbo].[tblTransactionAliasFields]
SET [DispatchField]=0
WHERE [DispatchField] IS NULL

-- 8.0.5.16-0013 WI-36333 Add tblSitesAncillaryData

-- Original script created the table and altered tblSites, but this will have already been done.
-- In order to populate the tblSitesAncillaryData table, we''ll need to translate the Index from tblSites into the appropriate Guid
-- value for each field because they were excluded from being translated into Guids in script 00069.
INSERT INTO [dbo].[tblSitesAncillaryData] (SiteGuid
											,AdjustmentTransactionAliasGuid
											,IATAGuid
											,InventoryTransactionAliasGuid
											,NoteGuid
											,CreatedBy
											,UpdatedBy) SELECT tb1.SiteGuid
																,tb2.TransactionAliasGuid as ''AdjustmentTransactionAliasGuid''
																,NULL as ''IATAGuid''
																,tb4.TransactionAliasGuid as ''InventoryTransactionAliasGuid''
																,NULL as ''NoteGuid''
																,tb1.CreatedBy
																,tb1.UpdatedBy 
															FROM [dbo].[tblSites] tb1
																LEFT JOIN [dbo].[tblTransactionAliases] tb2
																	ON tb1.AdjustmentTransactionAliasIndex = tb2.AliasID
																LEFT JOIN [dbo].[tblTransactionAliases] tb4
																	ON tb1.InventoryTransactionAliasIndex = tb4.AliasID
															WHERE tb1.SiteGuid NOT IN (SELECT tb5.SiteGuid FROM [dbo].[tblSitesAncillaryData] tb5)


-- 8.0.5.17-0019 WI-36718 Change Station Fields in tblTransactions to Reference tblIATA instead of tblStations 
-- The original script presents issues with risk of data loss. This is because the script
ALTER TABLE dbo.tblIATA ALTER COLUMN Name NVARCHAR(200) NOT NULL;
GO
-- 8.0.5.17-0056 WI 36906 Add more fields to transaction line items: The tempalte does not have the new column, thus they have have been dropped on
-- another script.

-- 8.0.5.17-0067 WI-36892 Service Request Messaging - Modify tblTransactionWeightReadings to support previous versions

----UPDATE tblTransactionWeightReadings SET FuelsManagerVersionNumber = 1
----GO

----UPDATE tblTransactionWeightReadings SET HistoricalFlag = 0
----GO

-- Base on script: 8.0.5.17-0078 WI-37122 Add Rights mapping to tblGroupRights
-- The original script was modified because it relies on CASCADE UPDATE but the database will not have 
-- constraints during the upgrade process
DECLARE @GroupGuid UNIQUEIDENTIFIER

create table #tmp(x uniqueidentifier)

insert into #tmp
EXEC ( ''
SELECT GroupGuid
FROM tblGroups
WHERE GroupID=''''Administrator'''''')
select @GroupGuid=x from #tmp



DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(300)
	,	@Column NVARCHAR(500)
	,	@Sql NVARCHAR(max)

DECLARE GroupCursor CURSOR FOR

	SELECT TABLE_SCHEMA,TABLE_NAME
	FROM information_schema.columns
	WHERE Column_name=''GroupGuid''
	GROUP BY TABLE_SCHEMA,TABLE_NAME

OPEN GroupCursor
FETCH NEXT FROM GroupCursor INTO @Schema,@Table
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''UPDATE [''+@Schema+''].[''+@Table+''] SET [GroupGuid]=''''00000000-0000-0000-0000-000000000003'''' WHERE [GroupGuid]=''''''+CAST(@GroupGuid AS NVARCHAR(100))+'''''';''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM GroupCursor INTO @Schema,@Table
END
CLOSE GroupCursor
DEALLOCATE GroupCursor


drop table #tmp

GO

-- 8.0.5.18-0043 WI-37360 Update any Station fields configured in tblTransactionAliasFields

UPDATE tblTransactionAliasFields SET DbName = ''FinalStationIATAID'' WHERE DbName = ''FinalStationID''
UPDATE tblTransactionAliasFields SET DbName = ''NextStationIATAID'' WHERE DbName = ''NextStationID''
UPDATE tblTransactionAliasFields SET DbName = ''PreviousStationIATAID'' WHERE DbName = ''PreviousStationID''
UPDATE tblTransactionAliasFields SET DbName = ''OriginStationIATAID'' WHERE DbName = ''OriginStationID''


-- 8.0.5.19-0028 WI-34234 Record Versioning - Prototyping - Prepare Entity Tables for Record Versioning
UPDATE dbo.tblEquipment
SET _MasterRecordGUID = EquipmentGuid
WHERE _MasterRecordGuid IS NULL


ALTER TABLE dbo.tblEquipment 
ALTER COLUMN _MasterRecordGuid uniqueidentifier NOT NULL
GO


UPDATE dbo.tblProducts
SET _MasterRecordGUID = ProductGuid
WHERE _MasterRecordGuid IS NULL


ALTER TABLE dbo.tblProducts 
ALTER COLUMN _MasterRecordGuid uniqueidentifier NOT NULL

GO

-- 8.0.5.19-0032 WI-34236 Record Versioning - AddAssignedFromFieldToTblProductToSite
/* Set the AssignedFrom field value to be the owner site for each Product. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityProductToSite] a
INNER JOIN tblProducts b
ON b.ProductGuid = a.ProductGuid
WHERE a.AssignedFromSiteGuid IS NULL



/* Set the AssignedFrom field as non-nullable so as to force it to be set in all future mappings */
ALTER TABLE [map].[tblEntityProductToSite]
ALTER COLUMN AssignedFromSiteGuid uniqueidentifier NOT NULL
GO

-- below ''IF'' introduced to fix a duplicate record
IF EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupToRightGuid=''204BAEF2-71BB-4F36-8762-C4281E7CBCFF'' AND LookupRightIndex = 146)
	UPDATE map.tblGroupToRight
	SET LookupRightIndex = 147
	WHERE GroupToRightGuid=''204BAEF2-71BB-4F36-8762-C4281E7CBCFF'' AND LookupRightIndex = 146

IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 143)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''F3E7AB51-D080-4715-9ED9-F7DA1721EF37'',''00000000-0000-0000-0000-000000000003'',143)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 144)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''9F5CFE11-ACBB-44F1-BA7B-404B3B299B0F'',''00000000-0000-0000-0000-000000000003'',144)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 145)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''CE922A1A-C991-46E0-A847-A53573BC0F18'',''00000000-0000-0000-0000-000000000003'',145)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 146)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''1E39A468-1109-497F-8D22-9C2C639FFEB2'',''00000000-0000-0000-0000-000000000003'',146)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 147)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''204BAEF2-71BB-4F36-8762-C4281E7CBCFF'',''00000000-0000-0000-0000-000000000003'',147)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 148)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''460837B2-7AD8-4A6F-9E9F-B68F6CB0F1A9'',''00000000-0000-0000-0000-000000000003'',148)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 149)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''597664FE-F8ED-4C17-95CD-78BA14C9AFEC'',''00000000-0000-0000-0000-000000000003'',149)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 150)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''31CE8E42-0F5B-47C4-8844-8058083575EE'',''00000000-0000-0000-0000-000000000003'',150)
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid=''00000000-0000-0000-0000-000000000003'' AND LookupRightIndex = 151)
	INSERT INTO map.tblGroupToRight (GroupToRightGuid,GroupGuid,LookupRightIndex) VALUES (''0A89F2DE-7C80-4728-80D7-C7B572C9A23A'',''00000000-0000-0000-0000-000000000003'',151)



-- 8.0.5.21-0003 WI-34234 Record Versioning - Prepare CompanyTable for Record Versioning


UPDATE dbo.tblCompanies
SET _MasterRecordGUID = CompanyGuid
WHERE _MasterRecordGuid IS NULL


-- 8.0.5.21-0004 WI-34236 Record Versioning - AddAssignedFromFieldToTblCompanyToSite
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyToSite] a
INNER JOIN tblCompanies b
ON b.CompanyGuid = a.CompanyGuid
WHERE a.AssignedFromSiteGuid IS NULL

-- 8.0.5.21-0035 WI-39043 Modify Note Field tblCompany


UPDATE [dbo].[tblCompanies] SET [dbo].[tblCompanies].[Note] = [dbo].[tblNotes].[Note]
    FROM [dbo].[tblNotes]  INNER JOIN [dbo].[tblCompanies] 
            ON [dbo].[tblCompanies].[NotesIndex] = [dbo].[tblNotes].[Index] 

-- Based on script 8.0.5.21-0036 WI-39043 Modify SpecialInstructionNote Field
-- It was modified because at this stage the [map].[tblProductToCompany] does not have the SpecialInstructionNoteGuid column
-- so the process was modified to establish the join with its original tblCompanyMap table

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompany'' AND COLUMN_NAME = ''SpecialInstructionNoteGuid'')
	ALTER TABLE [map].[tblProductToCompany] ADD [SpecialInstructionNote] NVARCHAR(2000) NULL
GO



IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompany'' AND COLUMN_NAME = ''SpecialInstructionNoteGuid'') AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompany'' AND COLUMN_NAME = ''SpecialInstructionNote'')
	UPDATE [map].[tblProductToCompany] SET [map].[tblProductToCompany].[SpecialInstructionNote] = [dbo].[tblNotes].[Note]
	FROM [dbo].[tblNotes]
	INNER JOIN dbo.tblProductMap ON dbo.tblProductMap.SpecialInstructionIndex=tblNotes.[Index]
	INNER JOIN [map].[tblProductToCompany] ON [map].[tblProductToCompany]._LegacyProductMapIndex=dbo.tblProductMap.[Index]
	


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompanyGroup'' AND COLUMN_NAME = ''SpecialInstructionNoteGuid'')
	ALTER TABLE [map].[tblProductToCompanyGroup] ADD [SpecialInstructionNote] NVARCHAR(2000) NULL
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompanyGroup'' AND COLUMN_NAME = ''SpecialInstructionNoteGuid'') AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''map'' AND TABLE_NAME = ''tblProductToCompanyGroup'' AND COLUMN_NAME = ''SpecialInstructionNote'')
	UPDATE [map].[tblProductToCompanyGroup] SET [map].[tblProductToCompanyGroup].[SpecialInstructionNote] = [dbo].[tblNotes].[Note]
    FROM [dbo].[tblNotes] 
	INNER JOIN dbo.tblProductMap ON dbo.tblProductMap.SpecialInstructionIndex=tblNotes.[Index]
	INNER JOIN [map].[tblProductToCompanyGroup] ON [map].[tblProductToCompanyGroup]._LegacyProductMapIndex=dbo.tblProductMap.[Index]


-- 8.0.5.21-0079 WI-39030 Removing Graphical View Toolbar Options
delete from lookup.tblCustomToolbarCommandType where CustomToolbarCommandTypeCode = ''FILTERS''
delete from lookup.tblCustomToolbarCommandType where CustomToolbarCommandTypeCode = ''GRAPHICAL_VIEW''
delete from lookup.tblCustomToolbarCommandType where CustomToolbarCommandTypeCode = ''TRANSFER_TO_ACCOUNTING''

delete from tblCustomToolbarCommand
delete from tblCustomToolbar
delete from lookup.tblCustomToolbarCommandType where LookupCustomToolbarTypeIndex = 2
delete from lookup.tblCustomToolbarType where CustomToolbarTypeIndex = 2

-- WI 55115 - Web Dispatch - Optional Times button not available
INSERT INTO [dbo].[tblCustomToolbar]
(
		[CustomToolbarGuid]
      ,[DispatchConfigurationGuid]
      ,[LookupCustomToolbarTypeIndex]
      ,[ID]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
 )
 SELECT
	DispatchConfigurationGuid AS [CustomToolbarGuid],
	DispatchConfigurationGuid,	
	1,	
	''Dispatch Tabular View'', 
	GETUTCDATE(), 
	''administrator'', 
	GETUTCDATE(), 
	''administrator''
 FROM [dbo].[tblDispatchConfiguration]
 WHERE SiteGuid <> ''00000000-0000-0000-0000-000000000001'' 
 AND DispatchConfigurationGuid NOT IN (SELECT DispatchConfigurationGuid FROM [dbo].[tblDispatchConfiguration])

 INSERT INTO [dbo].[tblCustomToolbarCommand]
	(
		[CustomToolbarCommandGuid]
      ,[CustomToolbarGuid]
      ,[CustomToolbarID]
      ,[LookupCustomToolbarCommandTypeIndex]
      ,[TransactionAliasGuid]
      ,[ID]
      ,[ColumnOrder]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]

	  )
SELECT
	convert(uniqueidentifier, --ok
				hashbytes(''md5'',	
						(convert(varchar(36),[CustomToolbarGuid])+
						convert(varchar(36), l.CustomToolbarCommandTypeIndex)))) AS [CustomToolbarCommandGuid],
	[CustomToolbarGuid],
	t.[ID],
	l.CustomToolbarCommandTypeIndex,
	null,
	l.[CustomToolbarCommandTypeName], 
	--ROW_NUMBER() OVER(ORDER BY l.[CustomToolbarCommandTypeName]) - 1,
	(CASE [CustomToolbarCommandTypeCode]
		WHEN ''REQUEST'' THEN 0
		WHEN ''TRANSIENT'' THEN 1
		WHEN ''FAST_LOG'' THEN 2
		WHEN ''FAST_LOG_FILLSTAND'' THEN 3
		WHEN ''RELOG'' THEN 4
		WHEN ''DISPATCHING_VIEW'' THEN 5
		WHEN ''CONTROL_LOG'' THEN 6
		WHEN ''STANDBY'' THEN 7
		WHEN ''FLIGHT_LINE_STATUS'' THEN 8
		WHEN ''DISPATCHERS_LIST'' THEN 9
		WHEN ''OPTIONAL_TIMES'' THEN 10
		WHEN ''RELEASE_TO_ACCOUNTING'' THEN 11
		WHEN ''CANCEL'' THEN 12
		WHEN ''REFRESH'' THEN 13
		END),
	GETUTCDATE(), 
	''administrator'', 
	GETUTCDATE(), 
	''administrator''
  FROM [dbo].[tblCustomToolbar] t, 
  (SELECT CustomToolbarCommandTypeIndex, [CustomToolbarCommandTypeName], [CustomToolbarCommandTypeCode]  FROM [lookup].tblCustomToolbarCommandType WHERE
  [CustomToolbarCommandTypeCode] IN (
		''CANCEL'',--
		''CONTROL_LOG'',--
		''DISPATCHING_VIEW'',--
		''DISPATCHERS_LIST'',--
		''RELEASE_TO_ACCOUNTING'',--
		''FAST_LOG'',--
		''FAST_LOG_FILLSTAND'',--
		''FLIGHT_LINE_STATUS'',--
		''OPTIONAL_TIMES'',--
		''RELOG'',--
		''REQUEST'',--
		''STANDBY'',--
		''TRANSIENT'',--
		''REFRESH''--
	) ) l
	WHERE convert(uniqueidentifier, --ok
				hashbytes(''md5'',	
						(convert(varchar(36),[CustomToolbarGuid])+
						convert(varchar(36), l.CustomToolbarCommandTypeIndex)))) 
						NOT IN (SELECT [CustomToolbarCommandGuid] FROM [dbo].[tblCustomToolbarCommand])

	 

-- 8.0.5.21-0085 WI-39089 Add Audit Configuration Setting
if not exists(SELECT * FROM  dbo.tblConfigurationSetting WHERE KeyType=''SZ'' and SettingKey=''AuditEnabled'')
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedBy,UpdatedBy,CreatedDate,UpdatedDate)
	VALUES(''38364AB6-EFDD-43B2-8D36-4D9CF381D092'',''SZ'',''AuditEnabled'',''0'',''Administrator'',''Administrator'',''2013-02-25'',''2013-02-25'')

-- 8.0.5.21-0094 WI-39053 Record Versioning - Prepare Entity Tables for Record Versioning - Company
UPDATE dbo.tblCompanies
SET _MasterRecordGUID = CompanyGuid
WHERE _MasterRecordGuid IS NULL


ALTER TABLE dbo.tblCompanies
ALTER COLUMN _MasterRecordGuid uniqueidentifier NOT NULL
GO

ALTER TABLE dbo.tblCompanies
ALTER COLUMN ID NVARCHAR(30) NOT NULL
GO

-- 8.0.5.21-0095 WI-39057 Record Versioning - AddAssignedFromFieldToTblCompanyToSite
/* Set the AssignedFrom field value to be the owner site for each Company. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyToSite] a
INNER JOIN tblCompanies b
ON b.CompanyGuid = a.CompanyGuid
WHERE a.AssignedFromSiteGuid IS NULL


/* Set the AssignedFrom field as non-nullable so as to force it to be set in all future mappings */
ALTER TABLE [map].[tblEntityCompanyToSite]
ALTER COLUMN AssignedFromSiteGuid uniqueidentifier NOT NULL
GO
-- 8.0.5.21-0138 WI-40121 Update tblVersion for iteration 21
--UPDATE VERSION
INSERT INTO tblVersion([Version],packageName,DateApplied,Comments,Check1,Check2,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (''8.0.5.21'',''StandardDatabase'',''2013-03-12 14:15:00.000'',''FuelsManager Cirrus 8.0.5.21'',0,0,''2013-03-12 14:15:00.000'',''Administrator'',''2013-03-12 14:15:00.000'',''Administrator'')


-- 8.0.5.21-0139 WI-40121 Update CreateBy UpdatedBy of Lookup tables

/*
	UPDATES CreatedBy and UpdatedBy columns to ''Administrator''
*/

DECLARE @Table NVARCHAR(300)
	,	@Schema NVARCHAR(200)
	,	@Sql NVARCHAR(max)
DECLARE	@LineFeed CHAR(2)
	
SET @LineFeed=CHAR(13)+CHAR(10)
	
DECLARE TableCur CURSOR FOR
	SELECT	s.name as SchemaName
		,	t.name as TableName
	FROM sys.tables t
	INNER JOIN sys.schemas s ON  s.schema_id=t.schema_id
	INNER JOIN sys.columns c on c.object_id=t.object_id
	WHERE c.name=''CreatedBy''
	AND s.name=''lookup''
	ORDER BY s.name,t.name

OPEN TableCur
FETCH NEXT FROM TableCur INTO @Schema,@Table
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql =''UPDATE [''+@Schema+''].[''+@Table+''] ''+@LineFeed
	SET @Sql+=''SET [CreatedBy]=''''Administrator'''', [UpdatedBy]=''''Adminsitrator''''''
	PRINT @Sql
	EXEC sp_executesql @statement=@Sql
	FETCH NEXT FROM TableCur INTO @Schema,@Table
END
CLOSE TableCur
DEALLOCATE TableCur

GO

---- 8.0.5.22-0016 WI-40173 Drop columns from tblTransactionLineItems
--DECLARE @DefaultName NVARCHAR(200)
--SET @DefaultName = (SELECT sys.default_constraints.name FROM sys.default_constraints 
--					INNER JOIN sys.columns ON sys.default_constraints.parent_column_id = sys.columns.column_id
--											AND sys.default_constraints.parent_object_id = sys.columns.object_id
--					INNER JOIN sys.tables ON sys.default_constraints.parent_object_id = sys.tables.object_id
--					WHERE sys.columns.name = ''FuelAdditiveFlag'' AND sys.tables.name = ''tblTransactionLineItems'')
--IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''[dbo].['' + @DefaultName + '']'') AND type = ''D'')
--BEGIN
--	EXEC (''ALTER TABLE [dbo].[tblTransactionLineItems] DROP CONSTRAINT ['' + @DefaultName + '']'')
--END
--GO

/* FuelsManager v10.x replaced some core database columns in dbo.tblProducts with a single new column that stores the data in an XML structure.
   We need to take the old values and build up a new XML structure for each Product Record
*/

SET QUOTED_IDENTIFIER ON
GO

DECLARE @VcfModuleXmlData TABLE 
(
	ProductIndex bigint,
	ProductID nvarchar(30),
	SiteIndex bigint,
	ProductGuid uniqueidentifier,
	SiteGuid uniqueidentifier,
	DensityPressureXml XML,
	AlternateTemperatureXml XML,
	BaseTemperatureXml XML,
	AlternateBasePressureXml XML,
	CorrectionFactor0Xml XML,
	CorrectionFactor1Xml XML,
	CorrectionFactor2Xml XML,
	CorrectionFactor3Xml XML,
	CorrectionFactor4Xml XML,
	AlphaXml XML,
	UseProductObservedDensityXml XML,
	UseHydrometerCorrectionXml XML,
	ForceVcfTo4DigitsXml XML,
	CorrectionMethodTypeXml XML,
	CorrectionMethodSpecificXml XML
)

--
-- Fix up missing MinorCorrectionMethod values
--
UPDATE [dbo].[tblProducts] SET MinorCorrectionMethod = 0 WHERE MinorCorrectionMethod IS NULL

DECLARE @AdjustedMinorCorrectionMethodIndex TABLE 
(
	OldMajorCorrectionMethodIndex int,
	OldMinorCorrectionMethodIndex int,
	LookupMinorCorrectionMethodIndex int
)

INSERT INTO @AdjustedMinorCorrectionMethodIndex (p.OldMajorCorrectionMethodIndex, p.OldMinorCorrectionMethodIndex)
	SELECT p.MajorCorrectionMethod, p.MinorCorrectionMethod FROM [dbo].[tblProducts] p GROUP BY p.MajorCorrectionMethod, p.MinorCorrectionMethod

-- Convert the old MinorCorrectionMethod (Index) value to the new LookupMinorCorrectionMethodIndex (non-overlapping Index values).
UPDATE @AdjustedMinorCorrectionMethodIndex 
	SET LookupMinorCorrectionMethodIndex = OldMinorCorrectionMethodIndex + case OldMajorCorrectionMethodIndex
																				when 0 then	0 -- none
																				when 1 then	0 -- none 1980
																				when 2 then	1 -- API C
																				when 3 then	1 -- API C 1980
																				when 4 then	12 -- API F
																				when 5 then	12 -- API F 1980
																				when 6 then	17 -- Polynomial F
																				when 7 then	17 -- Polynomial F 1980
																				when 8 then	18 -- LPG C
																				when 9 then	18 -- LPG C 1980
																				when 10 then	19 -- ASTM D1555 F
																				when 11 then	19 -- ASTM D1555 F 1980
																				when 12 then	19 -- ASTM D1555 C
																				when 13 then	19 -- ASTM D1555 C 1980
																				when 14 then	0 -- Japan none
																				when 15 then	1 -- Japan JIS 2249
																				when 16 then	30 -- Japan JIS 2250
																				when 17 then	19 -- Japan ASTM D1555
																				when 18 then	49 -- Japan ASTM D1250
																				when 19 then	35 -- Japan Chemical
																				when 20 then	37 -- Japan JIS 2249 Table
																				when 21 then	40 -- GBT
																				when 22 then	43 -- GOST
																				when 23 then	44 -- Asphalt
																				when 24 then	49 -- ASTM D1250 1952
																				when 25 then	50 -- ASTM Commodities 2004
																				when 26 then	19 -- ASTM D1555 F 2009
																				ELSE 0
																			END
INSERT INTO @VcfModuleXmlData 
	SELECT	p.ProductIndex ''ProductIndex'',
			p.ProductID ''ProductID'',
			p.SiteIndex ''SiteIndex'',
			p.ProductGuid ''ProductGuid'',
			p.SiteGuid ''SiteGuid'',
			''<Value>0</Value>'' ''DensityPressureXml'',
			''<Value>'' + CASE WHEN p.AlternateTemperature IS NOT NULL THEN CONVERT(varchar, p.AlternateTemperature) ELSE ''0'' END + ''</Value>'' ''AlternateTemperatureXml'',
			''<Value>'' + CASE WHEN p.StandardTemperature IS NOT NULL THEN CONVERT(varchar, p.StandardTemperature) ELSE ''60'' END + ''</Value>'' ''BaseTemperatureXml'',
			''<Value>'' + CASE WHEN p.AlternatePressure IS NOT NULL THEN CONVERT(varchar, p.AlternatePressure) ELSE ''0'' END + ''</Value>'' ''AlternateBasePressureXml'',
			''<double>'' + CASE WHEN p.CorrectionFactor0 IS NOT NULL THEN CONVERT(varchar, p.CorrectionFactor0) ELSE ''0'' END + ''</double>'' ''CorrectionFactor0Xml'',
			''<double>'' + CASE WHEN p.CorrectionFactor1 IS NOT NULL THEN CONVERT(varchar, p.CorrectionFactor1) ELSE ''0'' END + ''</double>'' ''CorrectionFactor1Xml'',
			''<double>'' + CASE WHEN p.CorrectionFactor2 IS NOT NULL THEN CONVERT(varchar, p.CorrectionFactor2) ELSE ''0'' END + ''</double>'' ''CorrectionFactor2Xml'',
			''<double>'' + CASE WHEN p.CorrectionFactor3 IS NOT NULL THEN CONVERT(varchar, p.CorrectionFactor3) ELSE ''0'' END + ''</double>'' ''CorrectionFactor3Xml'',
			''<double>'' + CASE WHEN p.CorrectionFactor4 IS NOT NULL THEN CONVERT(varchar, p.CorrectionFactor4) ELSE ''0'' END + ''</double>'' ''CorrectionFactor4Xml'',
			''<Alpha>0</Alpha>'' ''AlphaXml'',
			''<UseProductObservedDensity>false</UseProductObservedDensity>'' ''UseProductObservedDensityXml'',
			''<UseHydrometerCorrection>'' + CASE WHEN p.ApplyVolumeCorrection IS NOT NULL AND p.ApplyVolumeCorrection = 1 THEN ''true'' ELSE ''false'' END + ''</UseHydrometerCorrection>'' ''UseHydrometerCorrectionXml'',
			''<ForceVcfTo4Digits>false</ForceVcfTo4Digits>'' ''ForceVcfTo4DigitsXml'',
			''<CorrectionMethodType>'' + CASE WHEN major.MajorCorrectionTypeCode IS NOT NULL THEN major.MajorCorrectionTypeCode ELSE '''' END + ''</CorrectionMethodType>'' ''CorrectionMethodTypeXml'',
			''<CorrectionMethodSpecific>'' + CASE WHEN minor.MinorCorrectionTypeCode IS NOT NULL THEN minor.MinorCorrectionTypeCode ELSE '''' END + ''</CorrectionMethodSpecific>'' ''CorrectionMethodSpecificXml''
	FROM [dbo].[tblProducts] p
		INNER JOIN @AdjustedMinorCorrectionMethodIndex am
			ON am.OldMajorCorrectionMethodIndex = p.MajorCorrectionMethod AND am.OldMinorCorrectionMethodIndex = p.MinorCorrectionMethod
		INNER JOIN [lookup].[tblMinorCorrectionType] minor 
			ON am.LookupMinorCorrectionMethodIndex = minor.MinorCorrectionTypeIndex	-- v7.5.3 SU7
		INNER JOIN [lookup].[tblMajorCorrectionType] major
			ON p.MajorCorrectionMethod = major.MajorCorrectionTypeIndex	-- v7.5.3 SU7

-- We can update all the Products with this initial XML structure.  The rest will be added as we iterate through the products below.
UPDATE [dbo].[tblProducts] SET VcfModuleSettings = N''<VcfModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <DensityPressure>
    <EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
  </DensityPressure>
  <AlternateTemperature>
    <EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
  </AlternateTemperature>
  <BaseTemperature>
    <EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
  </BaseTemperature>
  <AlternateBasePressure>
    <EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
  </AlternateBasePressure>
  <K>
  </K>
</VcfModuleSettings>''

PRINT ''Loop through all the product records from the table variable created above and set the dbo.tblProduct record VcfModuleSettings column.''
PRINT ''''
DECLARE @ProductIndex bigint, @ProductID nvarchar(30), @SiteIndex bigint, @ProductGuid uniqueidentifier, @SiteGuid uniqueidentifier, @DensityPressureXml XML, @AlternateTemperatureXml XML, @BaseTemperatureXml XML, @AlternateBasePressureXml XML, @CorrectionFactor0Xml XML, @CorrectionFactor1Xml XML, @CorrectionFactor2Xml XML, @CorrectionFactor3Xml XML, @CorrectionFactor4Xml XML, @AlphaXml XML, @UseProductObservedDensityXml XML, @UseHydrometerCorrectionXml XML, @ForceVcfTo4DigitsXml XML, @CorrectionMethodTypeXml XML, @CorrectionMethodSpecificXml XML

DECLARE ProductVcfModuleSettingCursor CURSOR FOR
		SELECT ProductIndex, ProductID, SiteIndex, ProductGuid, SiteGuid, DensityPressureXml, AlternateTemperatureXml, BaseTemperatureXml, AlternateBasePressureXml, CorrectionFactor0Xml, CorrectionFactor1Xml, CorrectionFactor2Xml, CorrectionFactor3Xml, CorrectionFactor4Xml, AlphaXml, UseProductObservedDensityXml, UseHydrometerCorrectionXml, ForceVcfTo4DigitsXml, CorrectionMethodTypeXml, CorrectionMethodSpecificXml FROM @VcfModuleXmlData

OPEN ProductVcfModuleSettingCursor
FETCH NEXT FROM ProductVcfModuleSettingCursor INTO @ProductIndex, @ProductID, @SiteIndex, @ProductGuid, @SiteGuid, @DensityPressureXml, @AlternateTemperatureXml, @BaseTemperatureXml, @AlternateBasePressureXml, @CorrectionFactor0Xml, @CorrectionFactor1Xml, @CorrectionFactor2Xml, @CorrectionFactor3Xml, @CorrectionFactor4Xml, @AlphaXml, @UseProductObservedDensityXml, @UseHydrometerCorrectionXml, @ForceVcfTo4DigitsXml, @CorrectionMethodTypeXml, @CorrectionMethodSpecificXml
WHILE @@FETCH_STATUS=0
BEGIN
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@DensityPressureXml") as last into (/VcfModuleSettings/DensityPressure)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@AlternateTemperatureXml") as last into (/VcfModuleSettings/AlternateTemperature)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@BaseTemperatureXml") as last into (/VcfModuleSettings/BaseTemperature)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@AlternateBasePressureXml") as last into (/VcfModuleSettings/AlternateBasePressure)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionFactor0Xml") as last into (/VcfModuleSettings/K)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionFactor1Xml") as last into (/VcfModuleSettings/K)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionFactor2Xml") as last into (/VcfModuleSettings/K)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionFactor3Xml") as last into (/VcfModuleSettings/K)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionFactor4Xml") as last into (/VcfModuleSettings/K)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@AlphaXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@UseProductObservedDensityXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@UseHydrometerCorrectionXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@ForceVcfTo4DigitsXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionMethodTypeXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;
	UPDATE [dbo].[tblProducts] SET VcfModuleSettings.modify(''insert sql:variable("@CorrectionMethodSpecificXml") as last into (/VcfModuleSettings)[1]'') WHERE ProductIndex = @ProductIndex;

	FETCH NEXT FROM ProductVcfModuleSettingCursor INTO @ProductIndex, @ProductID, @SiteIndex, @ProductGuid, @SiteGuid, @DensityPressureXml, @AlternateTemperatureXml, @BaseTemperatureXml, @AlternateBasePressureXml, @CorrectionFactor0Xml, @CorrectionFactor1Xml, @CorrectionFactor2Xml, @CorrectionFactor3Xml, @CorrectionFactor4Xml, @AlphaXml, @UseProductObservedDensityXml, @UseHydrometerCorrectionXml, @ForceVcfTo4DigitsXml, @CorrectionMethodTypeXml, @CorrectionMethodSpecificXml
END
CLOSE ProductVcfModuleSettingCursor
DEALLOCATE ProductVcfModuleSettingCursor

DELETE FROM @VcfModuleXmlData

PRINT ''Completed successfully''
GO

-- update product tracking guids.
DECLARE @ProductIndex INT 
 
DECLARE load_cursor CURSOR FOR 
    SELECT ProductIndex
    FROM dbo.tblProducts 
 
OPEN load_cursor 
FETCH NEXT FROM load_cursor INTO @ProductIndex 
 
WHILE @@FETCH_STATUS = 0 
BEGIN 
    BEGIN 
		--PRINT (@ProductIndex);
		--SELECT ProductIndex, ProductGuid, TrackingProductIndex, TrackingProductGuid, (SELECT tp.ProductGuid FROM dbo.tblProducts tp WHERE tblProducts.TrackingProductIndex = tp.ProductIndex) from tblProducts where ProductIndex = @ProductIndex
		UPDATE dbo.tblProducts SET TrackingProductGuid = (SELECT tp.ProductGuid FROM dbo.tblProducts tp WHERE tblProducts.TrackingProductIndex = tp.ProductIndex) WHERE ProductIndex = @ProductIndex;
    END 

    FETCH NEXT FROM load_cursor INTO @ProductIndex 
END 
 
CLOSE load_cursor 
DEALLOCATE load_cursor 
PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00071-10 Update Map and Split Tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00071-10 Update Map and Split Tables', 
		@step_id=31, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	First, If the QC Date on the Equipment matches the CreatedDate, NULL out the QC Date to avoid an initial problem with v9.x trying to convert
	the DateTimeOffset value to a Date.  This will be correctly later by changing the DataType of the column.

	Second, Identify, Log and Remove Equipment Records that appear to have a real QCDate (QCDate <> CreatedDate (minus time element)).
	This is done by simply cursoring through them and writing out the key bits of information necessary to capture the lost QCDate value.
*/
SET NOCOUNT ON;

BEGIN TRANSACTION
UPDATE [dbo].[tblEquipment] SET QCDate = NULL, UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = ''V9 Upgrade. AAC'' WHERE DATEADD(dd, 0, DATEDIFF(dd, 0, CreatedDate)) = DATEADD(dd, 0, DATEDIFF(dd, 0, QCDate)) AND QCDate IS NOT NULL

PRINT ''There may be equipment records that have a valid QC Date.  To avoid losing this information, output''
PRINT ''any remaining records with a QCDate to the migration output log and then NULL them out to avoid the application error.''
PRINT ''''
DECLARE @EquipmentGuid uniqueidentifier, @EquipmentID nvarchar(255), @QCDate datetimeoffset(7), @CreatedDate datetimeoffset(7)

DECLARE LostEquipmentQCDates CURSOR FOR
		SELECT EquipmentGuid, ID, QCDate, CreatedDate FROM [dbo].[tblEquipment] WHERE QCDate IS NOT NULL
PRINT ''EquipmentGuid, ID, QCDate, CreatedDate''

OPEN LostEquipmentQCDates
FETCH NEXT FROM LostEquipmentQCDates INTO @EquipmentGuid, @EquipmentID, @QCDate, @CreatedDate
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT CONVERT(nvarchar(64), @EquipmentGuid) + '','''''' + @EquipmentID + '''''','''''' + CONVERT(varchar(25), COALESCE(@QCDate,''NULL'')) + '''''','''''' + CASE WHEN @CreatedDate IS NULL THEN ''NULL'' ELSE CONVERT(varchar(64), @CreatedDate) END
	UPDATE [dbo].[tblEquipment] SET QCDate = NULL, UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = ''V9 Upgrade. AAC'' WHERE EquipmentGuid = @EquipmentGuid
	
	FETCH NEXT FROM LostEquipmentQCDates INTO @EquipmentGuid, @EquipmentID, @QCDate, @CreatedDate
END
CLOSE LostEquipmentQCDates
DEALLOCATE LostEquipmentQCDates

COMMIT TRANSACTION

SET NOCOUNT OFF;

-- THE ID column is a new field so it needs to be populated, combine the two type names to build a unique string
UPDATE dbo.tblListViews SET ID = newID.ID FROM (
	SELECT lv.[Index] ''ListViewIndex'', CONCAT(lvst.[ListViewStandardTypeName], ''-'', lvt.[ListViewTypeName]) ''ID''
		FROM dbo.tblListViews lv 
			INNER JOIN lookup.tblListViewType lvt 
				ON lv.[Type] = lvt.ListViewTypeIndex
			INNER JOIN lookup.tblListViewStandardType lvst
				ON lv.[TypeIndex] = lvst.ListViewStandardTypeIndex 
	) newID
	WHERE [Index] = newID.ListViewIndex

ALTER TABLE dbo.tblListViews
ALTER COLUMN ID NVARCHAR(50) NOT NULL
GO

-- THE ListViewID column is a new field so it needs to be populated, combine the ListViewID and the Field Number to build a unique string
UPDATE dbo.tblListViewFields SET ListViewID = newFieldID.ListViewID FROM (
	SELECT lvf.[Index] ''ListViewFieldIndex'', CONCAT(CONVERT(nvarchar(50), lv.[ID]), ''-'', CONVERT(nvarchar(20), ColumnOrder)) ''ListViewID''
	FROM dbo.tblListViews lv
		INNER JOIN dbo.tblListViewFields lvf
			ON lv.[Index] = lvf.[ListViewIndex]
		) newFieldID
	WHERE [Index] = newFieldID.ListViewFieldIndex

ALTER TABLE dbo.tblListViewFields
ALTER COLUMN ListViewID NVARCHAR(50) NOT NULL
GO

-- The Contrec1010_RA and VARECDET preset types are swapped in the enumeration between 7.5 and 9.x.
-- The below case statement fixes that position swap while leaving the others unchanged.
UPDATE dbo.tblLoadArms 
SET LookupPresetTypeIndex = CASE PresetType 
								WHEN 15 THEN 16
								WHEN 16 THEN 15
								ELSE PresetType
							END
WHERE LookupPresetTypeIndex IS NULL

ALTER TABLE dbo.tblLoadArms
ALTER COLUMN LookupPresetTypeIndex INT NOT NULL
GO

UPDATE dbo.tblProducts SET AutomaticCloseout = 0 WHERE AutomaticCloseout IS NULL

ALTER TABLE dbo.tblProducts
ALTER COLUMN AutomaticCloseout BIT NOT NULL
GO

UPDATE dbo.tblQueryStorage SET SystemQuery = 0 WHERE SystemQuery IS NULL

ALTER TABLE dbo.tblQueryStorage
ALTER COLUMN SystemQuery BIT NOT NULL
GO

UPDATE dbo.tblSites SET Enterprise = 0 WHERE Enterprise IS NULL

ALTER TABLE dbo.tblSites
ALTER COLUMN Enterprise BIT NOT NULL
GO

UPDATE dbo.tblSites SET GlobalAccessToPersonnel = 0 WHERE GlobalAccessToPersonnel IS NULL

ALTER TABLE dbo.tblSites
ALTER COLUMN [GlobalAccessToPersonnel] BIT NOT NULL 
GO

UPDATE dbo.tblSites SET GlobalAccessToEquipment = 0 WHERE GlobalAccessToEquipment IS NULL

ALTER TABLE dbo.tblSites
ALTER COLUMN [GlobalAccessToEquipment] BIT NOT NULL
GO

UPDATE dbo.tblSites SET OperateTabGroups = 1 WHERE OperateTabGroups IS NULL

ALTER TABLE dbo.tblSites
ALTER COLUMN [OperateTabGroups] BIT NOT NULL
GO

UPDATE dbo.tblStations SET PromptForGravityCaptured = 0 WHERE PromptForGravityCaptured IS NULL

ALTER TABLE dbo.tblStations
ALTER COLUMN [PromptForGravityCaptured] BIT NOT NULL
GO

UPDATE dbo.tblStations SET PromptForTemperatureCaptured = 0 WHERE PromptForTemperatureCaptured IS NULL

ALTER TABLE dbo.tblStations
ALTER COLUMN [PromptForTemperatureCaptured] BIT NOT NULL
GO

UPDATE dbo.tblStations SET LookupStationTypeIndex = [Type] WHERE LookupStationTypeIndex IS NULL

ALTER TABLE dbo.tblStations
ALTER COLUMN [LookupStationTypeIndex] INT NOT NULL
GO

UPDATE dbo.tblStations SET LookupStationInterfaceTypeIndex = [InterfaceType] WHERE LookupStationInterfaceTypeIndex IS NULL

ALTER TABLE dbo.tblStations
ALTER COLUMN [LookupStationInterfaceTypeIndex] INT NOT NULL
GO

UPDATE dbo.tblTankQualityTagLog SET QualityTagGuid = qtrec.QualityTagGuid FROM (
	SELECT tqtl.TankQualityTagLogIndex, qt.QualityTagGuid
		FROM dbo.tblTankQualityTagLog tqtl
			INNER JOIN dbo.tblQualityTags qt 
				ON tqtl.[QualityTagIndex] = qt.[QualityTagIndex]
	) qtrec
	WHERE dbo.tblTankQualityTagLog.[TankQualityTagLogIndex] = qtrec.[TankQualityTagLogIndex]

ALTER TABLE dbo.tblTankQualityTagLog
ALTER COLUMN [QualityTagGuid] uniqueidentifier NOT NULL
GO

-- 8.0.5.22-0048 WI-40151 Record Versioning - Prepare Entity Tables for Record Versioning - TransactionAlias
UPDATE dbo.tblTransactionAliases
SET _MasterRecordGUID = TransactionAliasGuid
WHERE _MasterRecordGuid IS NULL


ALTER TABLE dbo.tblTransactionAliases
ALTER COLUMN _MasterRecordGuid uniqueidentifier NOT NULL

GO


-- 8.0.5.22-0096 WI-40124 ADF New Security Right
IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightName = ''PERFORM_FORMAT_CONFIGURATION'')
BEGIN
	INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
	VALUES ( 162, ''PERFORM_FORMAT_CONFIGURATION'', ''PERFORM_FORMAT_CONFIGURATION'')
END

-- 8.0.5.22-0098 WI-40139 Dispatch - Fix problems in tblDispatchGridColumnType
-- The DataField of the Radio field is not correct. It should be RadioNumber, not Radio
IF EXISTS (SELECT * FROM lookup.tblDispatchGridColumnType WHERE ID = ''Radio'')
BEGIN
	UPDATE lookup.tblDispatchGridColumnType SET DataField = ''RadioNumber'' WHERE ID = ''Radio''
END

--The DataField of the Aircraft ID field is not correct. It should be DestinationRegistrationID, not DestinationRegistrationID1
IF EXISTS (SELECT * from lookup.tblDispatchGridColumnType WHERE DataField = ''DestinationRegistrationID1'')
BEGIN
	UPDATE lookup.tblDispatchGridColumnType SET DataField = ''DestinationRegistrationID'' WHERE DataField =''DestinationRegistrationID1''
END

-- The ProductID column is the same as the Grade column. It can be removed.
IF EXISTS (SELECT * from lookup.tblDispatchGridColumnType WHERE ID = ''ProductID'') 
BEGIN
	DELETE FROM tblDispatchGridColumn WHERE tblDispatchGridColumn.LookupDispatchGridColumnTypeIndex IN 
		(SELECT lookup.tblDispatchGridColumnType.DispatchGridColumnTypeIndex FROM lookup.tblDispatchGridColumnType WHERE ID = ''ProductID'')

	DELETE FROM lookup.tblDispatchGridColumnType WHERE ID = ''ProductID''
END

-- 8.0.5.22-0099 WI-38378 Record Versioning - tblEntityPersonnelToSite Changes to add AssignedFromSiteGuid

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = ''tblEntityPersonnelToSite'' AND COLUMN_NAME = ''AssignedFromSiteGuid'')
BEGIN

	ALTER TABLE [map].tblEntityPersonnelToSite ADD AssignedFromSiteGuid uniqueidentifier NULL
END

GO

EXEC sp_executesql
        N''UPDATE [map].tblEntityPersonnelToSite SET [map].tblEntityPersonnelToSite.AssignedFromSiteGuid =  (SELECT SiteGuid FROM [dbo].tblPersonnel WHERE PersonnelGuid = [map].tblEntityPersonnelToSite.PersonnelGuid )''

EXEC sp_executesql
        N''ALTER TABLE [map].tblEntityPersonnelToSite ALTER COLUMN AssignedFromSiteGuid uniqueidentifier NOT NULL''

-- 8.0.5.22-0100 WI-38378 Record Versioning - tblPersonnel Changes to add MasterRecordGuid
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = ''tblPersonnel'' AND COLUMN_NAME = ''_MasterRecordGuid'')
BEGIN

	ALTER TABLE [dbo].tblPersonnel ADD _MasterRecordGuid uniqueidentifier NULL
END
GO

EXEC sp_executesql
        N''UPDATE [dbo].tblPersonnel SET [_MasterRecordGuid] = [PersonnelGuid]''

EXEC sp_executesql
        N''ALTER TABLE [dbo].tblPersonnel ALTER COLUMN _MasterRecordGuid uniqueidentifier NOT NULL''

-- 8.0.5.22-0157 WI-40104 Synchronization - Create default Client and Server Synchronization Settings
/* {CheckPoint: Created Default Client and Server Configuration Records } */
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblSyncClientConfiguration])
BEGIN
	INSERT [dbo].[tblSyncClientConfiguration] ([SyncClientConfigurationGuid], [RootSiteID], [EnterpriseURL], [SuspendSynchronizationFlag], [ServerAuthUserName], [ServerAuthPassword], [ServerAuthDomain], [ServerAuthClientCertificate], [FMAuthUserName], [FMAuthPassword], [FMAuthClientCertificate], [MessageSecuritySigningCertificate], [MessageSecurityOfflineEncryptionCertificate], [MessageSecurityOfflineDecryptionCertificate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N''6a75fb31-7de8-414a-a32d-045a84482622'', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N''administrator'', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N''administrator'')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblSyncServerConfiguration])
BEGIN
	INSERT [dbo].[tblSyncServerConfiguration] ([SyncServerConfigurationGuid], [AllowSynchronizationFlag], [AcceptFMUserAuthenticationFlag], [AcceptClientCertificateAuthenticationFlag], [ClientSignatureRequiredForMessagesFlag], [ClientEncryptionRequiredForMessagesFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N''19d46ab0-6701-41ee-91ca-686dc3be7b90'', 0, 1, 0, 0, 0, CAST(0x0744E6D73EBFEF360B10FF AS DateTimeOffset), N''administrator'', CAST(0x0744E6D73EBFEF360B10FF AS DateTimeOffset), N''administrator'')
END
GO


PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00073 Create Records in New Tables and Update Mapping Data]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00073 Create Records in New Tables and Update Mapping Data', 
		@step_id=32, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	Table variables used to build up new Meter Records.
*/
DECLARE @tblStationList TABLE
(
	[StationGuid] [uniqueidentifier]
	,[SiteGuid] [uniqueidentifier]
)

DECLARE @tblLoadArmList TABLE
(
	[LoadArmGuid] [uniqueidentifier]
	,[SiteGuid] [uniqueidentifier]
)

DECLARE @tblMeterList TABLE
(
	[MeterID] [nvarchar] (50)
	,[SiteGuid] [uniqueidentifier]
)

/*
	Created Meter Records for [dbo].[tblStations] (in v7.5.x, the Station ID also represented the MeterID)  AssignedMeterGuid was added in FM10
	Stations are always Site Owned so the new Meter records will belong to the Site
*/
-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT convert(uniqueidentifier, --ok
				hashbytes(''md5'', 
						(convert(varchar(36), st.StationGuid)+ --
						convert(varchar(36), st.SiteGuid)))) AS [MeterGuid]
           ,st.SiteGuid AS [SiteGuid]
           ,st.ID AS [MeterID]
           ,8 AS [NumberOfDigits]
           ,0 AS [RotatesBackwardsFlag]
           ,0 AS [ReceiptMeterFlag]
		   ,SYSDATETIMEOFFSET() AS [CreatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		   ,SYSDATETIMEOFFSET() AS [UpdatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
		FROM [dbo].[tblStations] st
		WHERE st.[LookupStationTypeIndex] = 7 -- Bulk Additive Station

-- Update Stations with AssignedMeterGuid
UPDATE [dbo].[tblStations] SET AssignedMeterGuid = meters.MeterGuid FROM 
	(SELECT m.MeterGuid, m.MeterID 
		FROM [dbo].[tblMeter] m 
			INNER JOIN [dbo].[tblStations] s
				ON s.[ID] = m.[MeterID]
		WHERE s.[LookupStationTypeIndex] = 7
	) meters
	WHERE [dbo].[tblStations].ID = meters.MeterID


/*
	Created Meter Records for Product Mapping Tables - MeterID still exists in FM10 as well as AssignedToMeterGuid so we need to populate both.
	These records are always Site Owned so the new Meter records will belong to the Site
	Type 14
*/
INSERT INTO @tblStationList
	SELECT [dbo].[tblStations].[StationGuid],[dbo].[tblStations].[SiteGuid] FROM [dbo].[tblStations]

-- LoadArm belong to a site and are associated with a Station.  Stations are assigned to a Site indirectly 
--
INSERT INTO @tblLoadArmList
    SELECT [dbo].[tblLoadArms].[LoadArmGuid],data.[SiteGuid]
        FROM [dbo].[tblLoadArms]
            INNER JOIN (SELECT [StationGuid],[SiteGuid] FROM @tblStationList) data
                ON [dbo].[tblLoadArms].[BayAStationGuid] = data.[StationGuid]
        WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NOT NULL)
                AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NULL)
    UNION
    SELECT [dbo].[tblLoadArms].[LoadArmGuid],data1.[SiteGuid]
        FROM [dbo].[tblLoadArms]
            INNER JOIN (SELECT [StationGuid],[SiteGuid] FROM @tblStationList) data1
                ON [dbo].[tblLoadArms].[BayBStationGuid] = data1.[StationGuid]
        WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NULL) 
                AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NOT NULL)
    UNION
    SELECT [dbo].[tblLoadArms].[LoadArmGuid],data.[SiteGuid]
        FROM [dbo].[tblLoadArms]
            INNER JOIN (SELECT [StationGuid],[SiteGuid] FROM @tblStationList) data
                ON [dbo].[tblLoadArms].[BayAStationGuid] = data.[StationGuid]
            INNER JOIN (SELECT [StationGuid],[SiteGuid] FROM @tblStationList) data1
                ON [dbo].[tblLoadArms].[BayBStationGuid] = data1.[StationGuid]
        WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NOT NULL)
                AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NOT NULL)

-- Remove previous list of meters so we don''t insert duplicates
DELETE FROM @tblMeterList

INSERT INTO @tblMeterList 
	SELECT map.MeterID, arms.SiteGuid
	FROM [map].[tblProductToPresetComponentTankOrTankGroup] map
			INNER JOIN @tblLoadArmList arms
				ON arms.LoadArmGuid = map.AssignedToLoadArmGuid
	WHERE map.MeterID IS NOT NULL AND map.MeterID <> '''' AND map.MeterID NOT IN (SELECT MeterID FROM [dbo].[tblMeter])
	GROUP BY map.MeterID, arms.SiteGuid

-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT newid() AS [MeterGuid]
        ,ml.SiteGuid AS [SiteGuid]
        ,ml.MeterID AS [MeterID]
        ,8 AS [NumberOfDigits]
        ,0 AS [RotatesBackwardsFlag]
        ,0 AS [ReceiptMeterFlag]
		,SYSDATETIMEOFFSET() AS [CreatedDate]
		,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		,SYSDATETIMEOFFSET() AS [UpdatedDate]
		,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
	FROM @tblMeterList ml

/*
	Created Meter Records for [map].[tblProductToPresetFlowControlledAdditive] - MeterID still exists in FM10 as well as AssignedToMeterGuid so we need to populate both.
	These records are always Site Owned so the new Meter records will belong to the Site
	Type 14
*/
-- Remove previous list of meters so we don''t insert duplicates
DELETE FROM @tblMeterList

INSERT INTO @tblMeterList 
	SELECT map.MeterID, arms.SiteGuid
		FROM [map].[tblProductToPresetFlowControlledAdditive] map
				INNER JOIN @tblLoadArmList arms
					ON arms.LoadArmGuid = map.AssignedToLoadArmGuid
		WHERE map.MeterID IS NOT NULL AND map.MeterID <> '''' AND map.MeterID NOT IN (SELECT MeterID FROM [dbo].[tblMeter])
	GROUP BY map.MeterID, arms.SiteGuid

-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT newid() AS [MeterGuid]
           ,ml.SiteGuid AS [SiteGuid]
           ,ml.MeterID AS [MeterID]
           ,8 AS [NumberOfDigits]
           ,0 AS [RotatesBackwardsFlag]
           ,0 AS [ReceiptMeterFlag]
		   ,SYSDATETIMEOFFSET() AS [CreatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		   ,SYSDATETIMEOFFSET() AS [UpdatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
	FROM @tblMeterList ml

-- Update tblProductToPresetFlowControlledAdditive with AssignedToMeterGuid
UPDATE [map].[tblProductToPresetFlowControlledAdditive] SET AssignedToMeterGuid = meters.MeterGuid FROM 
	(SELECT m.MeterGuid, m.MeterID, m.SiteGuid
		FROM [dbo].[tblMeter] m 
			INNER JOIN [map].[tblProductToPresetFlowControlledAdditive] map
				ON map.[MeterID] = m.[MeterID]
	) meters
	WHERE [map].[tblProductToPresetFlowControlledAdditive].MeterID = meters.MeterID

/*
	Created Meter Records for [map].[tblProductToOffloadExternalMeter] - MeterID still exists in FM10 as well as AssignedToMeterGuid so we need to populate both.
	These records are always Site Owned so the new Meter records will belong to the Site
	Type 15
	Set ReceiptMeterFlag = 1
*/
-- Remove previous list of meters so we don''t insert duplicates
DELETE FROM @tblMeterList

INSERT INTO @tblMeterList 
	SELECT map.MeterID, arms.SiteGuid
		FROM [map].[tblProductToOffloadExternalMeter] map
				INNER JOIN @tblLoadArmList arms
					ON arms.LoadArmGuid = map.AssignedToLoadArmGuid
		WHERE map.MeterID IS NOT NULL AND map.MeterID <> '''' AND map.MeterID NOT IN (SELECT MeterID FROM [dbo].[tblMeter])
	GROUP BY map.MeterID, arms.SiteGuid

-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT newid() AS [MeterGuid]
           ,ml.SiteGuid AS [SiteGuid]
           ,ml.MeterID AS [MeterID]
           ,8 AS [NumberOfDigits]
           ,0 AS [RotatesBackwardsFlag]
           ,1 AS [ReceiptMeterFlag]
		   ,SYSDATETIMEOFFSET() AS [CreatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		   ,SYSDATETIMEOFFSET() AS [UpdatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
	FROM @tblMeterList ml

-- Update tblProductToOffloadExternalMeter with AssignedToMeterGuid
UPDATE [map].[tblProductToOffloadExternalMeter] SET AssignedToMeterGuid = meters.MeterGuid FROM 
	(SELECT m.MeterGuid, m.MeterID, m.SiteGuid
		FROM [dbo].[tblMeter] m 
			INNER JOIN [map].[tblProductToOffloadExternalMeter] map
				ON map.[MeterID] = m.[MeterID]
	) meters
	WHERE [map].[tblProductToOffloadExternalMeter].MeterID = meters.MeterID


/*
	Created Meter Records for [map].[tblProductToPresetInjector] - MeterID still exists in FM10 as well as AssignedToMeterGuid so we need to populate both.
	These records are always Site Owned so the new Meter records will belong to the Site
	Type 10 and 7
*/
-- Remove previous list of meters so we don''t insert duplicates
DELETE FROM @tblMeterList

INSERT INTO @tblMeterList 
	SELECT map.MeterID, arms.SiteGuid
		FROM [map].[tblProductToPresetInjector] map
				INNER JOIN @tblLoadArmList arms
					ON arms.LoadArmGuid = map.AssignedToLoadArmGuid
		WHERE map.MeterID IS NOT NULL AND map.MeterID <> '''' AND map.MeterID NOT IN (SELECT MeterID FROM [dbo].[tblMeter])
	GROUP BY map.MeterID, arms.SiteGuid

-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT newid() AS [MeterGuid]
           ,ml.SiteGuid AS [SiteGuid]
           ,ml.MeterID AS [MeterID]
           ,8 AS [NumberOfDigits]
           ,0 AS [RotatesBackwardsFlag]
           ,0 AS [ReceiptMeterFlag]
		   ,SYSDATETIMEOFFSET() AS [CreatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		   ,SYSDATETIMEOFFSET() AS [UpdatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
	FROM @tblMeterList ml

-- Update tblProductToPresetInjector with AssignedToMeterGuid
UPDATE [map].[tblProductToPresetInjector] SET AssignedToMeterGuid = meters.MeterGuid FROM 
	(SELECT m.MeterGuid, m.MeterID, m.SiteGuid
		FROM [dbo].[tblMeter] m 
			INNER JOIN [map].[tblProductToPresetInjector] map
				ON map.[MeterID] = m.[MeterID]
	) meters
	WHERE [map].[tblProductToPresetInjector].MeterID = meters.MeterID

/*
	Created Meter Records for [map].[tblProductToPresetComponentTankOrTankGroup] - MeterID still exists in FM10 as well as AssignedToMeterGuid so we need to populate both.
	These records are always Site Owned so the new Meter records will belong to the Site
	Type 10 and 7
*/
-- Remove previous list of meters so we don''t insert duplicates
DELETE FROM @tblMeterList

INSERT INTO @tblMeterList 
	SELECT map.MeterID, arms.SiteGuid
		FROM [map].[tblProductToPresetComponentTankOrTankGroup] map
				INNER JOIN @tblLoadArmList arms
					ON arms.LoadArmGuid = map.AssignedToLoadArmGuid
		WHERE map.MeterID IS NOT NULL AND map.MeterID <> '''' AND map.MeterID NOT IN (SELECT MeterID FROM [dbo].[tblMeter])
	GROUP BY map.MeterID, arms.SiteGuid

-- Create Meter Records
INSERT INTO [dbo].[tblMeter]
           ([MeterGuid]
           ,[SiteGuid]
           ,[MeterID]
           ,[NumberOfDigits]
           ,[RotatesBackwardsFlag]
           ,[ReceiptMeterFlag]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT newid() AS [MeterGuid]
           ,ml.SiteGuid AS [SiteGuid]
           ,ml.MeterID AS [MeterID]
           ,8 AS [NumberOfDigits]
           ,0 AS [RotatesBackwardsFlag]
           ,0 AS [ReceiptMeterFlag]
		   ,SYSDATETIMEOFFSET() AS [CreatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [CreatedBy]
		   ,SYSDATETIMEOFFSET() AS [UpdatedDate]
		   ,''v9.7 Upgrade. AAC'' AS [UpdatedBy]
	FROM @tblMeterList ml

-- Update tblProductToPresetComponentTankOrTankGroup with AssignedToMeterGuid
UPDATE [map].[tblProductToPresetComponentTankOrTankGroup] SET AssignedToMeterGuid = meters.MeterGuid FROM 
	(SELECT m.MeterGuid, m.MeterID, m.SiteGuid
		FROM [dbo].[tblMeter] m 
			INNER JOIN [map].[tblProductToPresetComponentTankOrTankGroup] map
				ON map.[MeterID] = m.[MeterID]
	) meters
	WHERE [map].[tblProductToPresetComponentTankOrTankGroup].MeterID = meters.MeterID
GO


PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00074 Record Versioning Creation]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00074 Record Versioning Creation', 
		@step_id=33, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	ADD UPDATE SCRIPT FOR RECORD VERSION
*/

/* UPDATE FOR UNOBTAINABLE with record versioning*/

/******* UNOBTAINABLE WAS SPECIFIC TO BSM-E BUT IT REMAINS HERE IN CASE TAS NEEDS TO DO SOMETHING SIMILAR LATER *********/
/* 
DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(300)
	,	@Column NVARCHAR(500)
	,	@Sql NVARCHAR(max)
SELECT s.SiteGuid, p.ProductGuid, a.Unobtainable, p.SiteGuid ''ProductSiteGuid'' INTO #tmpProductVal
FROM [dbo].[tblProductValuesBySite] a
INNER JOIN [dbo].[tblProducts] p on a.ProductIndex = p.ProductIndex
INNER JOIN [dbo].[tblSites] s on s.SiteIndex = a.SiteIndex

--update existing products
UPDATE  p SET p.UserData5 = pv.Unobtainable, p.updateddate = SYSDATETIMEOFFSET(), p.updatedby = ''v9 Upgrade''
FROM [dbo].[tblProducts] p INNER JOIN #tmpProductVal pv 
on p.SiteGuid = pv.SiteGuid
	and pv.productGuid = p.productguid
	and pv.SiteGuid = pv.ProductSiteGuid

--create rows for products overriding the master record
SELECT p.*, pv.siteGuid ''newsiteguid'',pv.unobtainable INTO #newProductRow
FROM [dbo].[tblProducts] p INNER JOIN #tmpProductVal pv 
on p.ProductGuid = pv.ProductGuid
	AND pv.SiteGuid <> pv.ProductSiteGuid

UPDATE a
SET a._MasterRecordGuid = a.ProductGuid, a.ProductGuid = newid(), 
a.UserData5 = a.Unobtainable, a.siteGuid = a.newsiteguid,
a.createddate = SYSDATETIMEOFFSET(), a.createdby = ''v9 Upgrade'',
a.updateddate = SYSDATETIMEOFFSET(), a.updatedby = ''v9 Upgrade''
FROM #newProductRow a 


--build insert statment
	--DECLARE @Sql NVARCHAR(max),	@Column NVARCHAR(500)
	DECLARE @columnList nvarchar(max)
	SELECT @Sql = ''INSERT INTO dbo.tblProducts ( '', @columnList = ''''
	
	DECLARE TableCur CURSOR FOR
			SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=''DBO'' AND TABLE_NAME = ''tblProducts'' and COLUMN_NAME not in (''_RowVersion'', ''ProductIndex'')

	OPEN TableCur
	FETCH NEXT FROM TableCur INTO @Column
	WHILE @@FETCH_STATUS=0
	BEGIN		
		SET @columnList += @Column
		FETCH NEXT FROM TableCur INTO @Column
		if @@FETCH_STATUS = 0
		BEGIN
			set @columnList += '',''
		END
	END
	CLOSE TableCur
	DEALLOCATE TableCur

	set @Sql += @columnList + '') SELECT '' + @columnList + '' FROM #newProductRow''
	print @sql


	EXEC sp_executesql @statement=@Sql


DROP TABLE #newProductRow
DROP TABLE #tmpProductVal

IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblUserDataFieldProduct] WHERE DisplayName =''Unobtainable''))
BEGIN
	INSERT INTO [dbo].[tblUserDataFieldProduct] (
	[UserDataFieldProductGuid]
		  ,[TransactionAliasGuid]
		  ,[SiteGuid]
		  ,[Number]
		  ,[DisplayOrder]
		  ,[DisplayName]
		  ,[LookupUserDataTypeIndex]
		  ,[Required]
		  ,[UserGroupGuid]
		  ,[CreatedDate]
		  ,[CreatedBy]
		  ,[UpdatedDate]
		  ,[UpdatedBy]
		  ,[DispatchField]
		  ,[ClearOnNew])
	  values (CONVERT(uniqueidentifier, ''2697AAE2-15E9-4443-85A2-FF81C59F5827''), null,CONVERT(uniqueidentifier, ''00000000-0000-0000-0000-000000000001''),4,0,''Unobtainable'',0,0,NULL, ''2009-09-08 17:14:45.0000000 +00:00'', ''Administrator'', ''2009-09-08 17:14:45.0000000 +00:00'', ''Administrator'',0,0)
END
*/

/* END UNOBTAINABLE UPDATE */


/******* Looks like this is creating Child Record Versions of Product Records owned by a SiteGroup. *******/
/**
INSERT INTO [dbo].tblproducts
(
	[ProductID]
      ,[Description]
      ,[GenericType]
      ,[StockResetDate]
      ,[StockTrack]
      ,[DensityHighLimit]
      ,[DensityLowLimit]
      ,[DensityDeadband]
      ,[ApplyDensityLimits]
      ,[TemperatureHiHiLimit]
      ,[TemperatureHighLimit]
      ,[TemperatureLowLimit]
      ,[TemperatureLoLoLimit]
      ,[TemperatureDeadband]
      ,[ApplyTemperatureLimits]
      ,[Bonded]
      ,[LowStockWarning]
      ,[GroundFuel]
      ,[ProductCode]
      ,[Price]
      ,[AviationFuelFlag]
      ,[StandardDensity]
      ,[ApplyVolumeCorrection]
      ,[TemperatureUnitIndex]
      ,[DensityUnitIndex]
      ,[TemperatureDecimalPlaces]
      ,[DensityDecimalPlaces]
      ,[Capitalize]
      ,[OctaneNumber]
      ,[ReidVaporPressure]
      ,[HazardousMaterial]
      ,[RegulatoryClass]
      ,[LoadRackDisplayText]
      ,[ComponentTolerance]
      ,[VaporRecovery]
      ,[LockedOut]
      ,[LockedOutReason]
      ,[LockedOutDate]
      ,[VarianceTolerance]
      ,[LoadByWeight]
      ,[ContaminationPromptLoadRackText]
      ,[InhibitAccounting]
      ,[UserData1]
      ,[UserData2]
      ,[UserData3]
      ,[UserData4]
      ,[UserData5]
      ,[UserData6]
      ,[UserData7]
      ,[UserData8]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[PIDXFamilyCode]
      ,[ApplyStandardDensity]
      ,[VolumeUnitIndex]
      ,[VolumeDecimalPlaces]
      ,[DielectricTolerance]
      ,[PIDXCode]
      ,[MassUnitIndex]
      ,[LevelUnitIndex]
      ,[FlowUnitIndex]
      ,[PressureUnitIndex]
      ,[MassDecimalPlaces]
      ,[LevelDecimalPlaces]
      ,[FlowDecimalPlaces]
      ,[PressureDecimalPlaces]
      ,[VolumePackageSize]
      ,[MassPackageSize]
      ,[ProductGuid]
      ,[SiteGuid]
	  ,[SiteIndex]
      ,[LookupProductTypeIndex]
      ,[TrackingProductGuid]
      ,[TaxCode]
      ,[VcfModuleSettings]
      ,[ProductColor]
      ,[PatternColor]
      ,[PatternNumber]
      ,[_MasterRecordGuid]
      ,[HiddenDate]
      ,[AutomaticCloseout]
      ,[CorrectionFactor0]
      ,[CorrectionFactor1]
      ,[CorrectionFactor2]
      ,[CorrectionFactor3]
      ,[CorrectionFactor4]
)
SELECT p.[ProductID]
      ,p.[Description]
      ,p.[GenericType]
      ,p.[StockResetDate]
      ,p.[StockTrack]
      ,p.[DensityHighLimit]
      ,p.[DensityLowLimit]
      ,p.[DensityDeadband]
      ,p.[ApplyDensityLimits]
      ,p.[TemperatureHiHiLimit]
      ,p.[TemperatureHighLimit]
      ,p.[TemperatureLowLimit]
      ,p.[TemperatureLoLoLimit]
      ,p.[TemperatureDeadband]
      ,p.[ApplyTemperatureLimits]
      ,p.[Bonded]
      ,p.[LowStockWarning]
      ,p.[GroundFuel]
      ,p.[ProductCode]
      ,p.[Price]
      ,p.[AviationFuelFlag]
      ,p.[StandardDensity]
      ,p.[ApplyVolumeCorrection]
      ,p.[TemperatureUnitIndex]
      ,p.[DensityUnitIndex]
      ,p.[TemperatureDecimalPlaces]
      ,p.[DensityDecimalPlaces]
      ,p.[Capitalize]
      ,p.[OctaneNumber]
      ,p.[ReidVaporPressure]
      ,p.[HazardousMaterial]
      ,p.[RegulatoryClass]
      ,p.[LoadRackDisplayText]
      ,p.[ComponentTolerance]
      ,p.[VaporRecovery]
      ,p.[LockedOut]
      ,p.[LockedOutReason]
      ,p.[LockedOutDate]
      ,p.[VarianceTolerance]
      ,p.[LoadByWeight]
      ,p.[ContaminationPromptLoadRackText]
      ,p.[InhibitAccounting]
      ,p.[UserData1]
      ,p.[UserData2]
      ,p.[UserData3]
      ,p.[UserData4]
      ,p.[UserData5]
      ,p.[UserData6]
      ,p.[UserData7]
      ,p.[UserData8]
      ,p.[CreatedDate]
      ,''V9 Upgrade. AAC'' AS [CreatedBy]
      ,p.[UpdatedDate]
      ,p.[UpdatedBy]
      ,p.[PIDXFamilyCode]
      ,p.[ApplyStandardDensity]
	  ,p.[VolumeUnitIndex]
      ,p.[VolumeDecimalPlaces]
      ,p.[DielectricTolerance]
      ,p.[PIDXCode]
      ,p.[MassUnitIndex]
      ,p.[LevelUnitIndex]
      ,p.[FlowUnitIndex]
      ,p.[PressureUnitIndex]
      ,p.[MassDecimalPlaces]
      ,p.[LevelDecimalPlaces]
      ,p.[FlowDecimalPlaces]
      ,p.[PressureDecimalPlaces]
      ,p.[VolumePackageSize]
      ,p.[MassPackageSize]
      ,convert(uniqueidentifier, --ok
				hashbytes(''md5'',	
						(
						convert(varchar(36), p.ProductGuid)+ --
						convert(varchar(36), m.SiteGuid)))) as [ProductGuid]
      ,m.[SiteGuid]
	  ,0 AS SiteIndex --dummy value
      ,p.[LookupProductTypeIndex]
      ,p.[TrackingProductGuid]
      ,p.[TaxCode]
      ,p.[VcfModuleSettings]
      ,p.[ProductColor]
      ,p.[PatternColor]
      ,p.[PatternNumber]
      ,p.[_MasterRecordGuid]
      ,p.[HiddenDate]
      ,p.[AutomaticCloseout]
	  ,p.[CorrectionFactor0]
      ,p.[CorrectionFactor1]
      ,p.[CorrectionFactor2]
      ,p.[CorrectionFactor3]
      ,p.[CorrectionFactor4]
  FROM   (SELECT * FROM dbo.tblProducts p WHERE SiteGuid=''00000000-0000-0000-0000-000000000001'') p 
		JOIN map.tblEntityProductToSite m  ON p._MasterRecordGuid=m.ProductGuid
		LEFT JOIN dbo.tblProducts x ON x._MasterRecordguid=m.productguid AND m.SiteGuid=x.SiteGuid 
			WHERE x.SiteGuid IS NULL AND
			NOT EXISTS(SELECT TOP 1 1 FROM  dbo.tblProducts e WHERE p.ProductID=e.ProductID AND e._MasterRecordGuid<>p._MasterRecordGuid AND e._MasterRecordGuid=e.ProductGuid)
**/

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00075-0 Identify, Log and Remove Records with NON NULLable columns]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00075-0 Identify, Log and Remove Records with NON NULLable columns', 
		@step_id=34, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	Identify, Log and Remove Records with NON NULLable columns that should be archived.
	Some dbo.tblTransactionPIDX records reference LoadIDs that have been removed from the Company Map tables.  We need to audit the fact that the migration script
	will be skipping these records in Step 33.
*/

SET NOCOUNT ON;

PRINT ''The following records in dbo.tblTransactionPIDX are mapped to LoadIDs that no longer exist in the database.''
PRINT ''These records WILL NOT be migrated to the the new version of FuelsManager because of the missing Company Map records.''
PRINT ''''
DECLARE @AuthorizationIndex bigint, @TransID varchar(64), @AuthorizationNumber varchar(16), @PIDXProfileIndex int, @LoadIDCompanyMapIndex int, @SentFlag bit, @DateSent datetime, @CreatedBy nvarchar(50), @CreatedDate datetime, @UpdatedBy nvarchar(50), @UpdatedDate datetime, @BrokenBlend bit, @BOLVersion int

DECLARE UnmappedTransPIDX CURSOR FOR
		SELECT AuthorizationIndex, TransID, AuthorizationNumber, PIDXProfileIndex, LoadIDCompanyMapIndex, SentFlag, DateSent, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, BrokenBlend, BOLVersion FROM [dbo].[tblTransactionPIDX] WHERE LoadIDCompanyMapIndex NOT IN (SELECT _LegacyCompanyMapIndex FROM [map].[tblCompanyPersonnelToShipToBillTo])
PRINT ''AuthorizationIndex, TransID, AuthorizationNumber, PIDXProfileIndex, LoadIDCompanyMapIndex, SentFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, BrokenBlend, BOLVersion''

OPEN UnmappedTransPIDX
FETCH NEXT FROM UnmappedTransPIDX INTO @AuthorizationIndex, @TransID, @AuthorizationNumber, @PIDXProfileIndex, @LoadIDCompanyMapIndex, @SentFlag, @DateSent, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @BrokenBlend, @BOLVersion
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT CONVERT(varchar(16), @AuthorizationIndex) + '','''''' + @TransID + '''''','''''' + COALESCE(@AuthorizationNumber,''NULL'') + '''''','' + CONVERT(varchar(64), @PIDXProfileIndex) + '','' + CONVERT(varchar(64), @LoadIDCompanyMapIndex) + '','' + CONVERT(varchar(8), @SentFlag) + '','''''' + CASE WHEN @DateSent IS NULL THEN ''NULL'' ELSE CONVERT(varchar(64), @DateSent) END + '''''','''''' + @CreatedBy + '''''','''''' + CASE WHEN @CreatedDate IS NULL THEN ''NULL'' ELSE CONVERT(varchar(64), @CreatedDate) END + '''''','''''' + @UpdatedBy + '''''','''''' + CASE WHEN @UpdatedDate IS NULL THEN ''NULL'' ELSE CONVERT(varchar(64), @UpdatedDate) END + '''''','' + CASE WHEN @BrokenBlend IS NULL THEN ''0'' ELSE CONVERT(varchar(8), @BrokenBlend) END + '','' + CASE WHEN @BOLVersion IS NULL THEN ''0'' ELSE CONVERT(varchar(64), @BOLVersion) END

	FETCH NEXT FROM UnmappedTransPIDX INTO @AuthorizationIndex, @TransID, @AuthorizationNumber, @PIDXProfileIndex, @LoadIDCompanyMapIndex, @SentFlag, @DateSent, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @BrokenBlend, @BOLVersion
END
CLOSE UnmappedTransPIDX
DEALLOCATE UnmappedTransPIDX

PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00075-1 Update Remaining Empty Lookup Indexs and Foreign Keys]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00075-1 Update Remaining Empty Lookup Indexs and Foreign Keys', 
		@step_id=35, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	Transfer enumeration values from obsolete columns to new LookupType columns
*/
SET NOCOUNT ON;

UPDATE tb1
SET tb1.SiteGuid = tb2.SiteGuid
FROM [tblArchivedUsers] tb1
LEFT JOIN tblSites tb2 on tb2.siteindex=tb1.siteindex
LEFT JOIN tblUsers tb3 on tb3.UserIndex=tb1.UserIndex

UPDATE tb1
SET tb1.EquipmentGuid=tb2.EquipmentGuid
FROM [tblEquipmentMaintenanceLog] tb1
INNER JOIN tblEquipment tb2 ON tb2.[Index]=tb1.[EquipmentIndex]


UPDATE tb1
SET tb1.TankGuid=tb2.TankGuid
FROM [tblTankMaintenanceLog] tb1
INNER JOIN tblTanks tb2 ON tb2.[TankIndex]=tb1.[TankIndex]

UPDATE tb1
SET tb1.EquipmentGuid=tb2.EquipmentGuid
,	tb1.QualityTagGuid=tb3.QualityTagGuid
FROM [dbo].[tblEquipmentQualityTagLog] tb1
LEFT JOIN tblEquipment tb2  on tb2.[Index]=tb1.EquipmentIndex
LEFT JOIN tblQualityTags tb3 on tb3.QualityTagIndex=tb1.QualityTagIndex

UPDATE tb1
SET tb1.TankGuid=tb2.TankGuid
,	tb1.QualityTagGuid=tb3.QualityTagGuid
FROM [dbo].[tblTankQualityTagLog] tb1
LEFT JOIN tblTanks tb2  on tb2.[TankIndex]=tb1.TankIndex
LEFT JOIN tblQualityTags tb3 on tb3.QualityTagIndex=tb1.QualityTagIndex

UPDATE tblEquipmentTypes SET [AllowFuelingByWeight] =1 where [AllowFuelingByWeight]  is null

update e set e.VolumeUnits = s.VolumeUnitIndex, e.MassUnits = s.MassUnitIndex from tblequipmenttypes e inner join tblsites s on e.SiteGuid = s.SiteGuid 

-- THESE TABLES GET SKIPPED AND ARE HANDLED LATER IN THE MIGRATION DURING STEP 33.
--update tb1
--set tb1.[ExportResultGuid] =tb2.[ExportResultGuid] 
--from [tblExportResultDetails] tb1
--inner join tblExportResults tb2 on tb2.[Index]=tb1.[ExportResultIndex]

update tb1
set tb1.ListViewGuid=tb2.ListViewGuid
from tblListViewFields tb1
inner join tblListViews tb2 on tb2.[Index]=tb1.ListViewIndex

UPDATE  [dbo].[tblChangesQueue]
SET [LookupChangeQueueRecordTypeIndex]=RecordType

UPDATE [dbo].[tblListViewFields] 
SET [LookupListViewFieldTypeIndex]=[Type]

PRINT ''Updating TransactionAliasGuid on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET TransactionAliasGuid =
	(SELECT TransactionAliasGuid
		FROM dbo.tblTransactionAliases
		WHERE TypeIndex = dbo.tblTransactionAliases.AliasID)
WHERE LookupListViewFieldTypeIndex = 1

PRINT ''Updating TransactionAliasFieldGuid on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET TransactionAliasFieldGuid =
	(SELECT TransactionAliasFieldGuid
		FROM dbo.tblTransactionAliasFields
		WHERE TypeIndex = dbo.tblTransactionAliasFields.[Index])
WHERE LookupListViewFieldTypeIndex = 2

PRINT ''Updating UserDataFieldTransactionAliasGuid on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET UserDataFieldTransactionAliasGuid =
	(SELECT UserDataFieldTransactionAliasGuid
		FROM dbo.tblUserDataFieldTransactionAlias
		WHERE TypeIndex = dbo.tblUserDataFieldTransactionAlias._LegacyIndex)
WHERE LookupListViewFieldTypeIndex = 3

PRINT ''Updating UserDataFieldTransactionAliasLineItemGuid on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET UserDataFieldTransactionAliasLineItemGuid =
	(SELECT UserDataFieldTransactionAliasLineItemGuid
		FROM dbo.tblUserDataFieldTransactionAliasLineItem
		WHERE TypeIndex = dbo.tblUserDataFieldTransactionAliasLineItem._LegacyIndex)
WHERE LookupListViewFieldTypeIndex = 5

/*******Vivian commented out for review - Index does not exist in  dbo.tblLedgerAggregateColumns consolidateddb
PRINT ''Updating LedgerAggregateColumnGuid on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET LedgerAggregateColumnGuid =
	(SELECT LedgerAggregateColumnGuid
		FROM dbo.tblLedgerAggregateColumns
		WHERE TypeIndex = dbo.tblLedgerAggregateColumns.[Index])
WHERE LookupListViewFieldTypeIndex = 6
*****************************/


PRINT ''Updating LookupStandardFieldTypeIndex on tblListViewFields...''
UPDATE [dbo].[tblListViewFields]
SET LookupStandardFieldTypeIndex = TypeIndex
WHERE LookupListViewFieldTypeIndex = 4


UPDATE [dbo].[tblListViews] 
SET [LookupListViewTypeIndex] =[Type]

PRINT ''Updating TransactionAliasGuid on tblListViews...''

-- 8.0.5.0-380 WI-25764 DB Revision - Add TypeGuid to tblListViews and tblListViewFields

UPDATE [dbo].[tblListViews]
SET TransactionAliasGuid =
	(SELECT TransactionAliasGuid
		FROM dbo.tblTransactionAliases
		WHERE TypeIndex = dbo.tblTransactionAliases.AliasID)
WHERE LookupListViewTypeIndex = 1


/*******Vivian commented out for review - Index does not exist in  dbo.tblLedgerAggregateColumns consolidateddb
PRINT ''Updating LedgerAggregateColumnGuidLedgerAggregateColumnGuid on tblListViews...''
UPDATE [dbo].[tblListViews]
SET LedgerAggregateColumnGuid =
	(SELECT LedgerAggregateColumnGuid
		FROM dbo.tblLedgerAggregateColumns
		WHERE TypeIndex = dbo.tblLedgerAggregateColumns.[Index])
WHERE LookupListViewTypeIndex = 3
************************************/

PRINT ''Updating LookupListViewStandardTypeIndex on tblListViews...''
UPDATE [dbo].[tblListViews]
SET LookupListViewStandardTypeIndex = TypeIndex
WHERE LookupListViewTypeIndex = 2


UPDATE [dbo].[tblQualifications] 
SET [LookupQualificationTypeIndex]=[Type]

--UPDATE [dbo].[tblReportApprovals]  
--SET [LookupReportApprovalStateIndex]=[ApprovalState]
--GO

UPDATE tb1
SET tb1.[LookupVesselTypeIndex] =tb1.[VesselTypeIndex]
,	tb1.[TankGuid]=tb2.[TankGuid]
,	tb1.OperatorPersonnelGuid=tb3.PersonnelGuid
FROM [dbo].[tblTankMaintenanceLog] tb1
LEFT JOIN [dbo].[tblTanks] tb2 on tb1.TankIndex=tb2.TankIndex
LEFT JOIN [dbo].[tblPersonnel] tb3 on tb1.OperatorIndex=tb3.PersonIndex

UPDATE tblSites
SET [AllowUseOfSpecialChars] = (CASE WHEN [AllowUseOfSpecialChars] IS NULL THEN 1 ELSE [AllowUseOfSpecialChars] END)
,	[EnablePeriodicSyncFlag] = (CASE WHEN [EnablePeriodicSyncFlag] IS NULL THEN 0 ELSE [EnablePeriodicSyncFlag] END)
,	[PeriodicSyncIntervalMinutes] = (CASE WHEN [PeriodicSyncIntervalMinutes] IS NULL THEN 0 else [PeriodicSyncIntervalMinutes] END)
,	[UseTankReconciliation] =  (CASE WHEN [UseTankReconciliation] IS NULL THEN 0 else [UseTankReconciliation] END)
,	[MeterReconciliationToleranceIsPercent]  =  (CASE WHEN [MeterReconciliationToleranceIsPercent] IS NULL THEN 0 else [MeterReconciliationToleranceIsPercent] END)

UPDATE tb1
SET tb1.[OwnerSiteGuid] =tb2.Siteguid
FROM [tblTestDefinitions] tb1
INNER JOIN tblSites tb2 on tb2.SiteIndex=tb1.OwnerSiteIndex

UPDATE tb1
SET tb1.[LookupTestSetStatusIndex] = tb1.[Status]
,	tb1.[TestSetEquipmentResultGuid] = tb2.[TestSetEquipmentResultGuid]
FROM [tblTestEquipmentResults] tb1
LEFT JOIN tblTestSetEquipmentResults tb2 ON tb2.TestSetEquipmentResultIndex=tb1.TestSetEquipmentResultIndex

UPDATE tb1
SET tb1.[LookupTestSetStatusIndex] = tb1.[Status]
,	tb1.[TestSetTankResultGuid] = tb2.[TestSetTankResultGuid]
FROM [tblTestTankResults] tb1
LEFT JOIN tblTestSetTankResults tb2 ON tb2.TestSetTankResultIndex=tb1.TestSetTankResultIndex

UPDATE tb1
SET tb1.OwnerSiteGuid=SiteGuid
FROM [tblTestSetDefinitions] tb1
INNER JOIN tblSites tb2 on tb2.SiteIndex=tb1.OwnerSiteIndex

UPDATE tb1
set tb1.[EquipmentGuid] = tb2.[EquipmentGuid] 
,	tb1.[LookupTestSetStatusIndex] =tb1.[Status]
from tblTestSetEquipmentResults tb1
LEFT join tblEquipment tb2 on tb2.[index]=tb1.EquipmentIndex

UPDATE tb1
set tb1.[TankGuid] = tb2.[TankGuid] 
,	tb1.[LookupTestSetStatusIndex] =tb1.[Status]
from tblTestSetTankResults tb1
LEFT join tblTanks tb2 on tb2.tankindex=tb1.TankIndex

UPDATE [tblTransactionAliases]
SET [IncludeInDispatch]=(CASE WHEN [IncludeInDispatch] IS NULL THEN 0 ELSE [IncludeInDispatch] END)
,	[LookupDefaultStatusIndex] = [DefaultStatus]
,	[LookupTransTypeIndex] =  [TransTypeID]

UPDATE tb1
SET tb1.[ClearOnNew] = (case when [ClearOnNew] is null then 0 ELSE [ClearOnNew] end)
,	tb1.[LookupTransactionFieldTypeIndex]  = tb1.[Type]
,	tb1.[TransactionAliasGuid] = tb2.TransactionAliasGuid
FROM tblTransactionAliasFields tb1
LEFT JOIN tblTransactionAliases tb2 ON tb2.AliasID=tb1.AliasID

UPDATE tb1
SET tb1.EquipmentGuid=tb2.EquipmentGuid
FROM [dbo].[tblTestSetEquipmentResults] tb1
INNER JOIN [dbo].[tblEquipment] tb2 ON tb2.[Index]=tb1.EquipmentIndex

UPDATE tb1
SET tb1.TankGuid=tb2.TankGuid
FROM [dbo].[tblTestSetTankResults] tb1
INNER JOIN [dbo].[tblTanks] tb2 ON tb2.[TankIndex]=tb1.TankIndex

UPDATE	tblTransactionAliases
SET		LookupTransTypeIndex = TransTypeID

PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00075-2 Record Version Update AssignedFromSiteGuid Columns]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00075-2 Record Version Update AssignedFromSiteGuid Columns', 
		@step_id=36, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
	ADD UPDATE SCRIPT FOR RECORD VERSION
	-- Script to update tables Assigned Sites for Record Version. This script is based on original script "8.0.5.26-0001 WI-41901 CascadingAssignments - AddAssignedFromField".
	-- which had to be split to fit the migration process. Step 00071 added the new column on target database however the below code could not be executed at the same time
	-- because Index were not coverted into GUID until script 00074
*/

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAdditiveProfileToSite] a
INNER JOIN tblAdditiveProfiles b
ON b.AdditiveProfileGuid = a.AdditiveProfileGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAlarmAndEventCategoryToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAlarmAndEventToSite] a
INNER JOIN tblSites b
ON b.SiteGuid = a.OwnerSiteGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAlarmPriorityToSite] a
INNER JOIN tblAlarmPriorities b
ON b.AlarmPriorityGuid = a.AlarmPriorityGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAllocationGroupToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentEquipmentToSite] a
INNER JOIN tblAppointmentEquipment b
ON b.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentPersonnelToSite] a
INNER JOIN tblAppointmentPersonnel b
ON b.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAppointmentTankToSite] a
INNER JOIN tblAppointmentTank b
ON b.AppointmentTankGuid = a.AppointmentTankGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAutoDistributionReasonCodeToSite] a
INNER JOIN tblAutoDistributionReasonCodes b
ON b.AutoDistributionReasonCodeGuid = a.AutoDistributionReasonCodeGuid
WHERE a.AssignedFromSiteGuid =''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityAutoDistributionRuleToSite] a
INNER JOIN tblAutoDistributionRule b
ON b.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each Company. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyToSite] a
INNER JOIN tblCompanies b
ON b.CompanyGuid = a.CompanyGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyCertificateAndPermitToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyGroupToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityCompanyTypeToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityDataDictionaryToSite] a
INNER JOIN tblSites b
ON b.SiteGuid = a.OwnerSiteGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityDispatchConfigurationToSite] a
INNER JOIN tblDispatchConfiguration b
ON b.DispatchConfigurationGuid = a.DispatchConfigurationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityDotHazardousMessagesToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEmailAddressToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEmailGroupToSite] a
INNER JOIN tblEmailGroups b
ON b.EmailGroupGuid = a.EmailGroupGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEntryMessageToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each equipment. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEquipmentToSite] a
INNER JOIN tblEquipment b
ON b.EquipmentGuid = a.EquipmentGuid


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEquipmentTagAndLicenseToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEquipmentTestAndInspectionToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityEquipmentTypeToSite] a
INNER JOIN tblEquipmentTypes b
ON b.EquipmentTypeGuid = a.EquipmentTypeGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityExitMessageToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityFootNoteToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityFuelCardToSite] a
INNER JOIN tblFuelCards b
ON b.FuelCardGuid = a.FuelCardGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityIATACodeToSite] a
INNER JOIN tblIATA b
ON b.IATAGuid = a.IATAGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityLedgerAggregateColumnToSite] a
INNER JOIN tblLedgerAggregateColumns b
ON b.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityLedgerViewToSite] a
INNER JOIN tblListViews b
ON b.ListViewGuid = a.ListViewGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityLedgerViewToSite] a
INNER JOIN tblListViews b
ON b.ListViewGuid = a.ListViewGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityListViewToSite] a
INNER JOIN tblListViews b
ON b.ListViewGuid = a.ListViewGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityMobileDeviceProfileToSite] a
INNER JOIN tblMobileDeviceProfile b
ON b.MobileDeviceProfileGuid = a.MobileDeviceProfileGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityPersonnelToSite] a
INNER JOIN tblPersonnel b
ON b.PersonnelGuid = a.PersonnelGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityPersonnelLicenseToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityPersonnelQualificationToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityPersonnelTrainingToSite] a
INNER JOIN tblQualifications b
ON b.QualificationGuid = a.QualificationGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityProcessVariableMessageToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each Product. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityProductToSite] a
INNER JOIN tblProducts b
ON b.ProductGuid = a.ProductGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityProductGroupToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityProductMessageToSite] a
INNER JOIN tblApplicationString b
ON b.ApplicationStringGuid = a.ApplicationStringGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid = b.SiteGuid
FROM  [map].[tblEntityQualityTagToSite] a
INNER JOIN tblQualityTags b
ON b.QualityTagGuid = a.QualityTagGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityReportConfigurationSettingsToSite] a
INNER JOIN tblSites b
ON b.SiteGuid = a.SiteGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityReportConfigurationSettingsToSite] a
INNER JOIN tblSites b
ON b.SiteGuid = a.SiteGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityStandingOfferToSite] a
INNER JOIN tblStandingOffers b
ON b.StandingOfferGuid = a.StandingOfferGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.OwnerSiteGuid
FROM  [map].[tblEntityTestSetToSite] a
INNER JOIN tblTestSetDefinitions b
ON b.TestSetDefinitionGuid = a.TestSetDefinitionGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.OwnerSiteGuid
FROM  [map].[tblEntityTestToSite] a
INNER JOIN tblTestDefinitions b
ON b.TestDefinitionGuid = a.TestDefinitionGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each TransactionAlias. Before Record Versioning, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityTransactionAliasToSite] a
INNER JOIN tblTransactionAliases b
ON b.TransactionAliasGuid = a.TransactionAliasGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityUserDataToSite] a
INNER JOIN tblSites b
ON b.SiteGuid = a.OwnerSiteGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

/* Remove any tblEntityUserGroupToSite mappings created earlier by the Migration Script if it was received by the Enterprise */
DELETE FROM [map].[tblEntityUserGroupToSite] WHERE UserGroupToSiteGuid IN ( SELECT map.UserGroupToSiteGuid
																				FROM [map].[tblEntityUserGroupToSite] map 
																					INNER JOIN (SELECT GroupGuid, SiteGuid 
																						FROM [map].[tblEntityUserGroupToSite] 
																						GROUP BY GroupGuid, SiteGuid 
																						HAVING COUNT(*) > 1
																					) dup
																					ON dup.GroupGuid = map.GroupGuid
																					AND dup.SiteGuid = map.SiteGuid
																			WHERE map.GroupGuid = dup.GroupGuid 
																			AND map.SiteGuid = dup.SiteGuid
																			AND map.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''
																	)
/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityUserGroupToSite] a
INNER JOIN tblGroups b
ON b.GroupGuid = a.GroupGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''


/* Remove any tblEntityUserToSite mappings created earlier by the Migration Script if it was received by the Enterprise */
-- Uncomment the following during 2nd Merge into Enteprise
DELETE FROM [map].[tblEntityUserToSite] WHERE UserToSiteGuid IN ( SELECT map.UserToSiteGuid
																	FROM [map].[tblEntityUserToSite] map 
																		INNER JOIN (SELECT UserGuid, SiteGuid 
																					FROM [map].[tblEntityUserToSite] 
																					GROUP BY UserGuid, SiteGuid 
																					HAVING COUNT(*) > 1
																				) dup
																			ON dup.UserGuid = map.UserGuid
																				AND dup.SiteGuid = map.SiteGuid
																	WHERE map.UserGuid = dup.UserGuid 
																		AND map.SiteGuid = dup.SiteGuid
																		AND map.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''
																)

/* Set the AssignedFrom field value to be the owner site for each entity. Before Cascading Assignments, entities could only be assigned from the owner sitegroup. */
UPDATE a
SET a.AssignedFromSiteGuid =  b.SiteGuid
FROM  [map].[tblEntityUserToSite] a
INNER JOIN tblUsers b
ON b.UserGuid = a.UserGuid
WHERE a.AssignedFromSiteGuid = ''00000000-0000-0000-0000-000000000000''

PRINT ''Completed successfully''
GO', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00076-1 DateTimeConvert special tables - Pass 1]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00076-1 DateTimeConvert special tables - Pass 1', 
		@step_id=37, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'PRINT ''''

--setup indexs we need to generate tables from pass 1 -- these are cleaned up in a different script

/********** Vivian  Modified script to add the check if index exists - This indexes should have been dropped at Step 3 but are not  *********************/
IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblSites'') AND name=''ix_migrate_tblSites'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblSites] ON [dbo].[tblSites] ([SiteIndex] ASC ) INCLUDE ( [SiteGuid])

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblEquipment'') AND name=''ix_migrate_tblEquipment'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblEquipment] ON [dbo].[tblEquipment] ([Index] ASC ) INCLUDE ( [EquipmentGuid] )

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblCompanies'') AND name=''ix_migrate_tblCompanies'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblCompanies] ON [dbo].[tblCompanies] ([CompanyIndex] ASC ) INCLUDE ( 	[CompanyGuid] )

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblProducts'') AND name=''ix_migrate_tblProducts'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblProducts] ON [dbo].[tblProducts] ([ProductIndex] ASC ) INCLUDE ( 	[ProductGuid] )

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblTanks'') AND name=''ix_migrate_tblTanks'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblTanks] ON [dbo].[tblTanks] ([TankIndex] ASC ) INCLUDE ( 	[TankGuid] )

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblFuelCards'') AND name=''ix_migrate_tblFuelCards'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblFuelCards] ON [dbo].[tblFuelCards] ([FuelCardIndex] ASC ) INCLUDE ( 	[FuelCardGuid] )

IF NOT EXISTS (SELECT * FROM sysindexes WHERE id=object_id(''DBO.tblAdditiveProfiles'') AND name=''ix_migrate_tblAdditiveProfiles'')
CREATE UNIQUE NONCLUSTERED INDEX [ix_migrate_tblAdditiveProfiles] ON [dbo].[tblAdditiveProfiles] ([Index] ASC ) INCLUDE ( 	[AdditiveProfileGuid] )
GO


DECLARE @SchemaName NVARCHAR(300)
	,	@ObjectName NVARCHAR(400)
	,	@TableName NVARCHAR(500)
	,	@MigrateTableName NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@ColumnName NVARCHAR(500)
	,	@FromType NVARCHAR(500)
	,	@ToType NVARCHAR(500)
	,	@SiteIndexLevel TINYINT
	,	@ProcessOrder TINYINT
	,	@MigrationTableSuffix nvarchar(20)

SET @MigrationTableSuffix = ''_DateMigrate_p1''

DECLARE @ConvertDate TABLE(
--CREATE TABLE #ConvertDate(
		RowNumber INT IDENTITY
	,	TableSchema NVARCHAR(200)
	,	TableName	NVARCHAR(500)
	,	ColumnName	NVARCHAR(500)
	,	FromDataType NVARCHAR(500)
	,	ToDataType NVARCHAR(500)
	,	SiteIndexLevel TINYINT DEFAULT 0
	)

INSERT INTO @ConvertDate(TableSchema,TableName,ColumnName,FromDataType,ToDataType)
SELECT	c1.table_schema
	,	c1.table_name
	,	c1.column_name
	,	c2.data_type as FromDataType
	,	c1.data_type as ToDataType
FROM FuelsManagerDB_Template.information_schema.columns c1
INNER JOIN ConsolidatedDB.information_schema.columns c2 ON
	(c2.table_schema=c1.table_schema AND c2.table_name=c1.table_name and c2.column_name=c1.column_name)
WHERE c2.data_type<>c1.data_type AND c1.DATA_TYPE = ''datetimeoffset'' and c1.TABLE_NAME not like ''%''+@MigrationTableSuffix
ORDER BY c1.table_schema,c1.table_name,c1.column_name


DECLARE DateCursor SCROLL CURSOR  FOR
	SELECT DISTINCT TableSchema,TableName--,ColumnName
	FROM @ConvertDate
	WHERE ToDataType=''datetimeoffset''
	
	ORDER BY TableSchema,TableName

OPEN DateCursor
FETCH NEXT FROM DateCursor INTO @SchemaName,@TableName
WHILE @@FETCH_STATUS=0
BEGIN
	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema=@SchemaName AND table_name= @TableName AND Column_Name=''SiteIndex'')
	BEGIN
		UPDATE @ConvertDate
		SET SiteIndexLevel = 1
		WHERE TableSchema=@SchemaName
		AND TableName= @TableName
	END
	
	-- LEVEL 2: Any transaction derived table as they all link with transaction table to find the SiteIndex
	UPDATE @ConvertDate
	SET SiteIndexLevel = 2
	WHERE LEFT(TableName,14) = ''tblTransaction''
	AND TableName NOT IN(''tblTransactionLineItemUserData'')
	AND SiteIndexLevel = 0

	--LEVEL 3: Remaining Tables
	UPDATE @ConvertDate
	SET SiteIndexLevel = 3
	WHERE TableName NOT IN(''tblExportResultDetails'',''tblAllocationLineItems'',''tblArchivedUsers'',''tblChangesQueue'')
	AND SiteIndexLevel = 0

	FETCH NEXT FROM DateCursor INTO @SchemaName,@TableName

END
CLOSE DateCursor
DEALLOCATE DateCursor

DECLARE TableCursor SCROLL CURSOR FOR
	SELECT DISTINCT TableSchema,TableName,SiteIndexLevel, CASE TableName WHEN ''tblExportResults'' THEN 3 WHEN ''tblExportInterfaceResult'' THEN 3 WHEN ''tblTransactions'' THEN 1 WHEN ''tblTransactionLineItems'' THEN 2 WHEN ''tblExportResultDetails'' THEN 4 ELSE 5  END as ProcessOrder
	FROM @ConvertDate
	ORDER BY ProcessOrder,TableSchema,TableName
OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel, @ProcessOrder
WHILE @@FETCH_STATUS=0
BEGIN

	declare 
			@IsNewColumn bit
		,	@SqlInsertColumns NVARCHAR(max)
		,	@SqlInsertVals NVARCHAR(max)
		,	@SqlTdzJoins NVARCHAR(max)
		,	@SqlGuidJoins NVARCHAR(max)
		,	@tzdCount INT

		SET @SqlInsertColumns = '''';
		SET @SqlInsertVals = '''';
		SET @SqlTdzJoins = '''';
		SET @SqlGuidJoins = '''';
		SET @tzdCount = 0;
		SET @MigrateTableName = @TableName + @MigrationTableSuffix

		-- create new table structure to match
		SET @Sql = ''SELECT *  INTO [''+@SchemaName+''].[''+@MigrateTableName+'']  FROM FuelsManagerDB_Template.[''+@SchemaName+''].[''+@TableName+''] WHERE 1=0''
		PRINT @Sql
		EXEC sp_executesql @statment=@Sql

		---***************************************************
		--  Alter tables here to save time later on (Adding columns that exist in consoildated db, they will be dropped later)
		---***************************************************
				DECLARE @AlterColumn NVARCHAR(500)
			,	@AlterType NVARCHAR(500)
			,	@AlterDefault NVARCHAR(2000)
			,	@AlterNullable VARCHAR(50)
			,	@AlterMaxLength INT
			,	@AlterPrecision INT
			,	@AlterPrecisionRadix INT
			,	@AlterSql NVARCHAR(max)

		DECLARE ColCursor SCROLL CURSOR FOR
			SELECT	x.COLUMN_NAME
				,	x.DATA_TYPE
				,	x.COLUMN_DEFAULT
				,	x.IS_NULLABLE
				,	x.CHARACTER_MAXIMUM_LENGTH
				,	x.NUMERIC_PRECISION
				,	x.NUMERIC_PRECISION_RADIX
			FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS x
			WHERE 
			x.Table_Name = @TableName and x.Table_Schema = @SchemaName
			AND 
			NOT EXISTS
			(	SELECT 1 
				FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS c
				WHERE	c.TABLE_SCHEMA=x.TABLE_SCHEMA
				AND		c.TABLE_NAME=x.TABLE_NAME
				AND		c.COLUMN_NAME=x.COLUMN_NAME
			)
			ORDER BY x.TABLE_SCHEMA,x.TABLE_NAME,x.ORDINAL_POSITION

		OPEN ColCursor
		FETCH NEXT FROM ColCursor
		INTO @AlterColumn,@AlterType,@AlterDefault,@AlterNullable,@AlterMaxLength,@AlterPrecision,@AlterPrecisionRadix

		WHILE @@FETCH_STATUS=0
		BEGIN
			SET @AlterSql = ''ALTER TABLE [''+@SchemaName+''].[''+@MigrateTableName+''] ''
			SET @AlterSql+= ''ADD [''+@AlterColumn+''] ''+@AlterType
			SET @AlterSql+= CASE	
							WHEN (@AlterMaxLength IS NOT NULL AND @AlterMaxLength > -1 AND @AlterMaxLength < 2147483647) 
								THEN ''(''+CAST(@AlterMaxLength AS NVARCHAR(100))+'') '' 
							WHEN @AlterMaxLength = -1
								THEN ''(MAX)''
							WHEN @AlterMaxLength = 2147483647
								THEN '' ''
							ELSE '' '' 
						END
	
			PRINT @AlterSql
			EXEC sp_executesql @statement=@Altersql

			FETCH NEXT FROM ColCursor
			INTO @AlterColumn,@AlterType,@AlterDefault,@AlterNullable,@AlterMaxLength,@AlterPrecision,@AlterPrecisionRadix
		END
		CLOSE ColCursor
		DEALLOCATE ColCursor


		--add special index column for joining in pass 2
		
		IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema=@SchemaName AND table_name=@MigrateTableName AND Column_Name=''SiteIndex'')
		BEGIN
			SET @AlterSql = ''ALTER TABLE [''+@SchemaName+''].[''+@MigrateTableName+''] ADD [SiteIndex] int''
			PRINT @AlterSql
			EXEC sp_executesql @statement=@Altersql

		END

		declare @IsNullable bit
		-- ADJUST DATA TYPES WHCIH DID NOT GET CREATED BASED ON USER DEFINED DATA TYPE

		DECLARE ObjCursor CURSOR FOR
			SELECT cl1.name as ColumnName,tp1.name as UserType,cl1.IS_NULLABLE AS IsNullable
			FROM FuelsManagerDB_Template.sys.tables tb1
			INNER JOIN FuelsManagerDB_Template.sys.columns cl1 on cl1.object_id=tb1.object_id
			INNER JOIN FuelsManagerDB_Template.sys.schemas sc1 on sc1.schema_id=tb1.schema_id
			INNER JOIN FuelsManagerDB_Template.sys.types tp1 on cl1.user_type_id=tp1.user_type_id
			WHERE cl1.system_type_id<> cl1.user_type_id
				AND tb1.name = @TableName AND sc1.name = @SchemaName
			ORDER BY cl1.name

		OPEN ObjCursor
		FETCH NEXT FROM ObjCursor INTO @AlterColumn,@AlterType, @IsNullable
		WHILE @@FETCH_STATUS=0
		BEGIN
			SET @AlterSql=''ALTER TABLE [''+@SchemaName+''].[''+@MigrateTableName+''] ALTER COLUMN [''+@AlterColumn+''] ''+@AlterType+'' '' + iif(@IsNullable = 1, '' NULL '', '' NOT NULL '') + '';''
			PRINT @AlterSql
			EXEC sp_executesql @statement=@AlterSql

			FETCH NEXT FROM ObjCursor INTO @AlterColumn,@AlterType,@IsNullable
		END
		CLOSE ObjCursor
		DEALLOCATE ObjCursor

		---***************************************************
		--- END ALTER new MIGRATION TABLE
		---***************************************************

		--- GENERATE DATA FOR COLUMNS
		DECLARE ColumnCursor SCROLL CURSOR FOR
		SELECT	c2.column_name
			--,	case when c2.data_type = ''datetimeoffset'' and c2.DATA_TYPE <> c1.DATA_TYPE then 1 else 0 end
			, case when c1.COLUMN_NAME IS NULL THEN 1 ELSE 0 END
		FROM ConsolidatedDB.information_schema.columns c2 -- newly built table with all columns
			LEFT JOIN ConsolidatedDB.information_schema.columns c1 --old table that might need conversion
				ON c1.TABLE_SCHEMA = c2.TABLE_SCHEMA AND c1.TABLE_NAME +@MigrationTableSuffix = c2.TABLE_NAME  and c2.column_name=c1.column_name
		WHERE c2.TABLE_SCHEMA = @SchemaName and c2.TABLE_NAME = @MigrateTableName
			AND c2.column_name <> ''_Rowversion''
		ORDER BY c1.column_name
		OPEN ColumnCursor
		FETCH NEXT FROM ColumnCursor INTO @ColumnName, @IsNewColumn
		WHILE @@FETCH_STATUS=0
		BEGIN

			BEGIN
				DECLARE @CurInsertVal nvarchar(max)
				SET @CurInsertVal = null; --''tb1.['' + @ColumnName + ''],'';

				IF @TableName = ''tblTransactions'' 
				BEGIN
					IF @ColumnName = ''TransactionGuid''				BEGIN	SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteGuid''						BEGIN	SET @CurInsertVal = ''s.SiteGuid,'' END
					IF @ColumnName = ''LookupTransTypeIndex''			BEGIN	SET @CurInsertVal = ''tb1.TransTypeID,'' END
					IF @ColumnName = ''TransactionStatus'' OR @ColumnName = ''LookupTransactionStatusIndex'' 
																	BEGIN
																		SET @CurInsertVal = ''tb1.TransactionStatus,''
																	END
					IF @ColumnName = ''OriginApplication'' OR @ColumnName = ''LookupOriginApplicationIndex''
																	BEGIN
																		SET @CurInsertVal = ''CASE 
																							WHEN tb1.OriginApplication = 1 THEN 1 -- base import to base accounting
																							WHEN tb1.OriginApplication = 2 THEN 2 -- TerminalAutomationService
																							ELSE tb1.OriginApplication END, -- anthing else,
																							''
																	END
					IF @ColumnName = ''TransactionAliasGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 ta.TransactionAliasGuid FROM [dbo].[tblTransactionAliases] ta where tb1.AliasIndex = ta.AliasID  ),'' END
					IF @ColumnName = ''Source1EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 se1.EquipmentGuid FROM [dbo].[tblEquipment] se1 where tb1.SourceEquipmentIndex1 = se1.[Index] ),'' END
					IF @ColumnName = ''Source2EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 se2.EquipmentGuid FROM [dbo].[tblEquipment] se2 where tb1.SourceEquipmentIndex2 = se2.[Index] ),'' END
					IF @ColumnName = ''Source3EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 se3.EquipmentGuid FROM [dbo].[tblEquipment] se3 where tb1.SourceEquipmentIndex3 = se3.[Index] ),'' END
					IF @ColumnName = ''Destination1EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 de1.EquipmentGuid FROM [dbo].[tblEquipment] de1 where tb1.DestinationEquipmentIndex1 = de1.[Index] ),'' END
					IF @ColumnName = ''Destination2EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 de2.EquipmentGuid FROM [dbo].[tblEquipment] de2 where tb1.DestinationEquipmentIndex2 = de2.[Index] ),'' END
					IF @ColumnName = ''Destination3EquipmentGuid''	BEGIN SET @CurInsertVal = ''(Select top 1 de3.EquipmentGuid FROM [dbo].[tblEquipment] de3 where tb1.DestinationEquipmentIndex3 = de3.[Index] ),'' END
					IF @ColumnName = ''FinalStationIATAGuid''		BEGIN SET @CurInsertVal = ''ia4.[IATAGuid],''	END
					IF @ColumnName = ''NextStationIATAGuid''		BEGIN SET @CurInsertVal = ''ia1.[IATAGuid],''	END
					IF @ColumnName = ''OriginStationIATAGuid''		BEGIN SET @CurInsertVal = ''ia2.[IATAGuid],''	END
					IF @ColumnName = ''PreviousStationIATAGuid''	BEGIN SET @CurInsertVal = ''ia3.[IATAGuid],''	END
					IF @ColumnName = ''FinalStationIATAID''			BEGIN SET @CurInsertVal = ''[FinalStation],''	END
					IF @ColumnName = ''NextStationIATAID''			BEGIN SET @CurInsertVal = ''[NextStation],''	END
					IF @ColumnName = ''OriginStationIATAID''		BEGIN SET @CurInsertVal = ''[OriginStation],''	 END
					IF @ColumnName = ''PreviousStationIATAID''		BEGIN SET @CurInsertVal = ''[PreviousStation],'' END
					IF @ColumnName = ''FuelCardGuid''				BEGIN SET @CurInsertVal = '' (Select top 1 fc.FuelCardGuid FROM [dbo].[tblFuelCards] fc where fc.FuelCardIndex = tb1.FuelCardIndex ),'' END
					IF @ColumnName = ''BillToCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 cbt.CompanyGuid FROM [dbo].[tblCompanies] cbt where tb1.BillToIndex = cbt.CompanyIndex ),'' END
					IF @ColumnName = ''ManagerCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 cm.CompanyGuid FROM [dbo].[tblCompanies] cm where cm.CompanyIndex = tb1.ManagerIndex ),'' END
					IF @ColumnName = ''OwnerCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 co.CompanyGuid FROM [dbo].[tblCompanies] co where co.CompanyIndex = tb1.OwnerIndex ),''	END
					IF @ColumnName = ''ShipperCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 sc.CompanyGuid FROM [dbo].[tblCompanies] sc where tb1.ShipperIndex = sc.CompanyIndex ),'' END
					IF @ColumnName = ''SupplierCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 supc.CompanyGuid FROM [dbo].[tblCompanies] supc where tb1.SupplierIndex = supc.CompanyIndex ),'' END
					IF @ColumnName = ''ShipToCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 stc.CompanyGuid FROM [dbo].[tblCompanies] stc where tb1.ShipToIndex = stc.CompanyIndex ),'' END		
					IF @ColumnName = ''CarrierCompanyGuid''			BEGIN SET @CurInsertVal = ''(Select top 1 cc.CompanyGuid FROM [dbo].[tblCompanies] cc where tb1.CarrierIndex = cc.CompanyIndex ),'' END	
					IF @ColumnName = ''OperatorPersonnelGuid''		BEGIN SET @CurInsertVal = '' (Select top 1 pp.PersonnelGuid FROM [dbo].[tblPersonnel] pp where tb1.OperatorIndex = pp.PersonIndex ),'' END	
					IF @ColumnName = ''SubmittedToAccounting''		BEGIN SET @CurInsertVal = ''1,'' END
					IF @ColumnName = ''Flag03''						BEGIN SET @CurInsertVal = ''NULL,'' END
					IF @ColumnName = ''FuelAdditiveFlag''			BEGIN SET @CurInsertVal = ''0,''	END
				END
				ELSE IF @TableName = ''tblTransactionLineItems'' 
				BEGIN
					IF @ColumnName = ''TransactionLineItemGuid''				BEGIN SET @CurInsertVal = ''newid(),''	END
					IF @ColumnName = ''LookupQualityIndex''						BEGIN SET @CurInsertVal = ''tb1.Quality,''END
					IF @ColumnName = ''LookupTransactionStatusIndex''			BEGIN SET @CurInsertVal = ''tb1.TransactionStatus,'' END
					IF @ColumnName = ''TransactionGuid''						BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,'' END
					IF @ColumnName = ''MeterGuid''								BEGIN SET @CurInsertVal = ''(SELECT top 1 MeterGuid FROM [dbo].[tblMeter] m WHERE m.MeterID = tb1.MeterID ),'' END
					IF @ColumnName = ''ProductGuid''							BEGIN SET @CurInsertVal = ''(SELECT top 1 ProductGuid FROM tblproducts p where P.ProductIndex = tb1.ProductIndex ),'' END
					IF @ColumnName = ''StorageLocationTankGuid''				BEGIN SET @CurInsertVal = ''(SELECT top 1 tk.TankGuid FROM tblTanks tk where tk.tankindex = tb1.StorageLocationIndex),'' END
					IF @ColumnName = ''AdditiveProfileGuid''					BEGIN SET @CurInsertVal = ''(SELECT top 1 ap.[AdditiveProfileGuid] FROM tblAdditiveProfiles ap where tb1.AdditiveProfileIndex = ap.[Index]),'' END
					IF @ColumnName = ''DestinationEquipmentGuid''				BEGIN SET @CurInsertVal = ''(SELECT top 1 de.EquipmentGuid FROM [dbo].[tblEquipment] de where tb1.DestinationEquipmentIndex = de.[Index]),'' END
					IF @ColumnName = ''SourceEquipmentGuid''					BEGIN SET @CurInsertVal = ''(SELECT top 1 se.EquipmentGuid FROM [dbo].[tblEquipment] se where tb1.SourceEquipmentIndex = se.[Index]),'' END
					IF @ColumnName = ''OperatorPersonnelGuid''					BEGIN SET @CurInsertVal = ''(Select top 1 pp.PersonnelGuid FROM [dbo].[tblPersonnel] pp where tb1.OperatorIndex = pp.PersonIndex  ),'' END
					IF @ColumnName = ''LoadingLocationStationGuid''				BEGIN SET @CurInsertVal = ''(SElect top 1 ll.StationGuid from [dbo].[tblStations] ll Where tb1.LoadingLocationIndex = ll.[Index]),'' END
					IF @ColumnName = ''OrderReferenceTransactionLineItemGuid''	BEGIN SET @CurInsertVal = ''null,'' END
					IF @ColumnName = ''SiteIndex''								BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionLineItemUserData'' 
				BEGIN
					IF @ColumnName = ''TransactionLineItemGuid''				BEGIN SET @CurInsertVal = ''tb2.TransactionLineItemGuid,''	END
					IF @ColumnName = ''TransactionLineItemUserDataGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''							BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionLinks'' 
				BEGIN
					IF @ColumnName = ''TransactionLinkGuid''				BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteGuid''							BEGIN SET @CurInsertVal = ''s.SiteGuid,''	END
					IF @ColumnName = ''LinkedTransactionLineItemGuid''	BEGIN SET @CurInsertVal = ''li1.TransactionLineItemGuid,''	END
					IF @ColumnName = ''TransactionLineItemGuid''			BEGIN SET @CurInsertVal = ''li2.TransactionLineItemGuid,''	END
				END
				ELSE IF @TableName = ''tblTransactionNotes'' 
				BEGIN
					IF @ColumnName = ''TransactionNoteGuid''			BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''TransactionGuid''				BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,''	END
					IF @ColumnName = ''SiteIndex''					BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionPIDX'' 
				BEGIN
					IF @ColumnName = ''TransactionPIDXGuid''						BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''TransactionGuid''							BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,''	END
					IF @ColumnName = ''PIDXProfileGuid''							BEGIN SET @CurInsertVal = ''pp.PIDXProfileGuid,''	END
					IF @ColumnName = ''CompanyPersonnelToShipToBillToGuid''		BEGIN SET @CurInsertVal = ''mp.CompanyPersonnelToShipToBillToGuid,''	END
					IF @ColumnName = ''SiteIndex''								BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionSignature'' 
				BEGIN
					IF @ColumnName = ''TransactionGuid''				BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,''	END
					IF @ColumnName = ''TransactionSignatureGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''						BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionSubLineItems'' 
				BEGIN
					IF @ColumnName = ''TransactionLineItemGuid''			BEGIN SET @CurInsertVal = ''tb2.TransactionLineItemGuid,''	END
					IF @ColumnName = ''TransactionSubLineItemGuid''			BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''TransactionGuid''					BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,'' END
					IF @ColumnName = ''MeterGuid''							BEGIN SET @CurInsertVal = ''m.MeterGuid,'' END
					IF @ColumnName = ''ProductGuid''						BEGIN SET @CurInsertVal = ''p.ProductGuid,'' END
					IF @ColumnName = ''LookupQualityIndex''					BEGIN SET @CurInsertVal = ''tb1.Quality,''END
					IF @ColumnName = ''LookupTransactionStatusIndex''		BEGIN SET @CurInsertVal = ''tb1.TransactionStatus,'' END
					IF @ColumnName = ''StorageLocationTankGuid''			BEGIN SET @CurInsertVal = ''tk.TankGuid,'' END
					IF @ColumnName = ''SiteIndex''							BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionTransportLineItems'' 
				BEGIN
					IF @ColumnName = ''TransactionGuid''						BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,''	END
					IF @ColumnName = ''TransactionTransportLineItemGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''							BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionUserData'' 
				BEGIN
					IF @ColumnName = ''TransactionGuid''				BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,''	END
					IF @ColumnName = ''TransactionUserDataGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''					BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblTransactionWeightReadings'' 
				BEGIN
					IF @ColumnName = ''FuelsManagerVersionNumber''		BEGIN SET @CurInsertVal = ''1,'' END
					IF @ColumnName = ''TransactionGuid''					BEGIN SET @CurInsertVal = ''tb2.TransactionGuid,'' END
					IF @ColumnName = ''HistoricalFlag''					BEGIN SET @CurInsertVal = ''0,'' END
					IF @ColumnName = ''TransactionWeightReadingGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''						BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END
				ELSE IF @TableName = ''tblExportResults'' 
				BEGIN
					IF @ColumnName = ''ExportResultGuid''				BEGIN SET @CurInsertVal = ''newid(),''	END
					IF @ColumnName = ''SiteGuid''						BEGIN SET @CurInsertVal = ''s.SiteGuid,''	END
					IF @ColumnName = ''LookupExportResultTypeIndex''	BEGIN SET @CurInsertVal = ''tb1.[Type],''	END
				END
				ELSE IF @TableName = ''tblExportResultDetails''			
				BEGIN 
					IF @ColumnName = ''ExportResultGuid''				BEGIN SET @CurInsertVal = ''tb2.ExportResultGuid,'' END
					IF @ColumnName = ''ExportResultDetailGuid''		BEGIN SET @CurInsertVal = ''newid(),'' END
					IF @ColumnName = ''SiteIndex''					BEGIN SET @CurInsertVal = ''tb2.SiteIndex,'' END -- to keep from joining to tbltransaction in second pass
				END

				IF @ColumnName = ''_ClusterIdx'' BEGIN SET @CurInsertVal = ''ROW_NUMBER() over(order by (Select 1)),'' END

				--print @columnname + '':'' +
				-- check if the column is not in the old table
				IF @CurInsertVal IS NULL AND @IsNewColumn = 1		SET @CurInsertVal = ''null,''
				IF @CurInsertVal IS NULL AND @IsNewColumn = 0		SET @CurInsertVal = ''tb1.['' + @ColumnName + ''],''

				--select @columnname, @CurInsertVal, @IsNewColumn
				
				SET @SqlInsertVals = @SqlInsertVals + @CurInsertVal
				--print @sqlinsertvals
			END	
			if (@SqlInsertColumns is null or @ColumnName is null )
			begin
				select @SqlInsertColumns, @ColumnName
			end
			SET @SqlInsertColumns = @SqlInsertColumns + ''['' + @ColumnName + ''],'';
			

			FETCH NEXT FROM ColumnCursor INTO @ColumnName, @IsNewColumn
		END
			
		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor

				
		DECLARE @siteQualifiedColumn nvarchar(50), @caseJoin nvarchar(max)
		IF @SiteIndexLevel > 0
		BEGIN
			IF @SiteIndexLevel=1
			BEGIN
			
				SET @siteQualifiedColumn = ''tb1.SiteIndex ''
				SET @caseJoin  = ''''
			END
			IF @SiteIndexLevel=2
			BEGIN
				SET @siteQualifiedColumn = ''tb2.SiteIndex ''
				SET @caseJoin = '' INNER MERGE JOIN dbo.tblTransactions''+@MigrationTableSuffix+'' tb2 ON tb2.TransID=tb1.TransID ''
			END
			IF @TableName = ''tblExportResultDetails''
			BEGIN
				SET @siteQualifiedColumn = ''tb2.SiteIndex ''
				SET @caseJoin = '' INNER MERGE JOIN dbo.tblExportResults''+@MigrationTableSuffix+'' tb2 ON tb2.[Index]=tb1.[ExportResultIndex] ''
			END
					
			-- tblTransactionLineItemUserData
			ELSE IF @TableName = ''tblTransactionLineItemUserData'' OR @TableName = ''tblTransactionSubLineItems''
			BEGIN
				SET @siteQualifiedColumn = ''tb2.SiteIndex ''
				SET @caseJoin = '' INNER MERGE JOIN dbo.tblTransactionLineItems'' + @MigrationTableSuffix + '' tb2 ON tb2.[TransLineItemID]=tb1.[TransLineItemID] ''
			END
			
			IF @TableName = ''tblTransactions''
			BEGIN
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblTransactionAliases] ta ON tb1.AliasIndex = ta.AliasID ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] de1 ON tb1.DestinationEquipmentIndex1 = de1.[Index] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] de2 ON tb1.DestinationEquipmentIndex2 = de2.[Index] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] de3 ON tb1.DestinationEquipmentIndex3 = de3.[Index] ''
				SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblIATA] ia1 ON tb1.[NextStation] = ia1.[IATAID] ''
				SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblIATA] ia2 ON tb1.[OriginStation] = ia2.[IATAID] ''
				SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblIATA] ia3 ON tb1.[PreviousStation] = ia3.[IATAID] ''
				SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblIATA] ia4 ON tb1.[FinalStation] = ia4.[IATAID] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] se1 ON tb1.SourceEquipmentIndex1 = se1.[Index] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] se2 ON tb1.SourceEquipmentIndex2 = se2.[Index] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblEquipment] se3 ON tb1.SourceEquipmentIndex3 = se3.[Index] ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] cbt ON tb1.BillToIndex = cbt.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] cm ON tb1.ManagerIndex = cm.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] co ON tb1.OwnerIndex = co.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] sc ON tb1.ShipperIndex = sc.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] stc ON tb1.ShipToIndex = stc.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] cc ON tb1.CarrierIndex = cc.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblCompanies] supc ON tb1.SupplierIndex = supc.CompanyIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblFuelCards] fc ON fc.FuelCardIndex = tb1.FuelCardIndex ''
			--	SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblPersonnel] pp ON tb1.OperatorIndex = pp.PersonIndex ''
			END

			IF @TableName = ''tblTransactionLinks''
			BEGIN
				SET @SqlGuidJoins += '' LEFT JOIN tblTransactionLineItems''+@MigrationTableSuffix+'' li1 ON li1.TransLineItemID=tb1.LineItemIndex ''
				SET @SqlGuidJoins += '' LEFT JOIN tblTransactionLineItems''+@MigrationTableSuffix+'' li2 ON li2.TransLineItemID=tb1.LinkedLineItemIndex ''			
			END

			-- WE DO NOT WANT TO BRING OVER ANY dbo.tblTransactionPIDX record that is referencing a DELETED Company Map record (LoadID).
			-- WE HAVE ALREADY LOGGED THE RECORDS THAT WILL BE SKIPPED DURING STEP 31
			IF @TableName = ''tblTransactionPIDX''
			BEGIN
				SET @SqlGuidJoins += '' LEFT JOIN dbo.tblPIDXProfiles pp ON pp.[Index]=tb1.PIDXProfileIndex ''
				SET @SqlGuidJoins += '' INNER JOIN map.tblCompanyPersonnelToShipToBillTo mp ON mp._LegacyCompanyMapIndex=tb1.LoadIDCompanyMapIndex ''
			END

			IF @TableName = ''tblTransactionSubLineItems''
			BEGIN
				SET @SqlGuidJoins += '' LEFT JOIN dbo.tblProducts p ON p.[ProductIndex]=tb1.ProductIndex ''							
				SET @SqlGuidJoins += '' LEFT JOIN tblTanks tk ON tb1.StorageLocationIndex = tk.TankIndex ''	
				SET @SqlGuidJoins += '' LEFT JOIN [dbo].[tblMeter] m ON m.MeterID = tb1.MeterID ''
			END
					

			--select @tablename, @SqlInsertColumns
					
			--remove trailing commas
			SET @SqlInsertColumns = LEFT(@SqlInsertColumns, len(@SqlInsertColumns) -1);
			SET @SqlInsertVals = LEFT(@SqlInsertVals, len(@SqlInsertVals) - 1);
			SET @Sql = ''''
			DECLARE @hasIdentity bit
			SET @hasIdentity = 0

			IF EXISTS( select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = @SchemaName and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, ''IsIdentity'') = 1 AND TABLE_NAME = @MigrateTableName)
			BEGIN
				SET @hasIdentity = 1
			END

			IF (@hasIdentity = 1)
			BEGIN
				SET @Sql = ''SET IDENTITY_INSERT [''+@SchemaName+''].[''+@MigrateTableName+''] ON ''
			END

			SET @Sql = @Sql + ''
				INSERT INTO [''+@SchemaName+''].[''+@MigrateTableName+''] WITH (TABLOCK) ('' + @SqlInsertColumns + '')
				SELECT '' + @SqlInsertVals + '' FROM [''+@SchemaName+''].[''+@TableName+''] tb1 '' 
				+ @caseJoin + 
				+ @SqlGuidJoins 
				IF @SiteIndexLevel=1
				BEGIN
			
					SET @sql += '' INNER JOIN [dbo].tblSites s on s.SiteIndex = ''+@siteQualifiedColumn --+ ''
				END
				
				--IF @TableName = ''tblTransactions''
				--BEGIN
				--	SET @Sql += '' WHERE tb1.InventoryDate >= dateadd(month,-18,''''9/1/2015'''')''
				--END
				--	inner join #tblTimeZones tz on  s.TimeZone = tz.Name '' +  @SqlTdzJoins 
						
			IF (@hasIdentity = 1)
			BEGIN
				SET @Sql = @Sql + ''
					SET IDENTITY_INSERT [''+@SchemaName+''].[''+@MigrateTableName+''] OFF ''
			END
			
			PRINT cast(@Sql as ntext)
			EXEC sp_executesql @statment=@Sql

			IF @TableName = ''tblTransactions''
			BEGIN
				SET @Sql = ''CREATE INDEX [ix_migrate_''+@TableName+''] ON [''+@SchemaName+''].[''+@MigrateTableName+''] (TransID) INCLUDE (TransactionGuid, SiteIndex)''
				print @sql
				EXEC sp_executesql @statment=@Sql
			END
			ELSE IF @TableName = ''tblTransactionsLineItems''
			BEGIN
				SET @Sql = ''CREATE INDEX [ix_migrate_''+@TableName+''] ON [''+@SchemaName+''].[''+@MigrateTableName+''] (TranLineItemID) INCLUDE (TransactionGuid, TransactionLineItemGuid, SiteIndex)''
				print @sql
				EXEC sp_executesql @statment=@Sql

			END
			ELSE IF @TableName = ''tblExportResults''
			BEGIN
				SET @Sql = ''CREATE INDEX [ix_migrate_''+@TableName+''] ON [''+@SchemaName+''].[''+@MigrateTableName+''] ([Index]) INCLUDE (SiteIndex)''
				print @sql
				EXEC sp_executesql @statment=@Sql
			END

			SET @Sql = ''CREATE INDEX [ix_migrate_siteindex_''+@TableName+''] ON [''+@SchemaName+''].[''+@MigrateTableName+''] (SiteIndex)''

			print @sql
			EXEC sp_executesql @statment=@Sql
		END

	FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel, @ProcessOrder
END
CLOSE TableCursor
DEALLOCATE TableCursor


PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00076-2 DateTimeConvert special tables - Pass 2]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00076-2 DateTimeConvert special tables - Pass 2', 
		@step_id=38, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'PRINT ''''
PRINT ''************************************''
PRINT ''** MODIFY DATETIME COLUMNS TO DATETIMEOFFSET''
PRINT ''************************************''

DECLARE @SchemaName NVARCHAR(300)
	,	@ObjectName NVARCHAR(400)
	,	@TableName NVARCHAR(500)
	,	@MigrateTableName NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@ColumnName NVARCHAR(500)
	,	@FromType NVARCHAR(500)
	,	@ToType NVARCHAR(500)
	,	@SiteIndexLevel TINYINT
	,	@ProcessOrder TINYINT
	,	@MigrationPreviousTableSuffix nvarchar(20)
	,	@MigrationTableSuffix nvarchar(20)

--DECLARE DateCursor SCROLL CURSOR FOR
SELECT @MigrationPreviousTableSuffix = ''_datemigrate_p1'', @MigrationTableSuffix =''_datemigrate_p2''

DECLARE @ConvertDate TABLE(
--CREATE TABLE #ConvertDate(
		RowNumber INT IDENTITY
	,	TableSchema NVARCHAR(200)
	,	TableName	NVARCHAR(500)
	,	ColumnName	NVARCHAR(500)
	,	FromDataType NVARCHAR(500)
	,	ToDataType NVARCHAR(500)
	,	SiteIndexLevel TINYINT DEFAULT 0
	)

INSERT INTO @ConvertDate(TableSchema,TableName,ColumnName,FromDataType,ToDataType)
SELECT	c1.table_schema
	,	c1.table_name
	,	c1.column_name
	,	c2.data_type as FromDataType
	,	c1.data_type as ToDataType
FROM ConsolidatedDB.information_schema.columns c1
INNER JOIN ConsolidatedDB.information_schema.columns c2 ON
	(c2.table_schema=c1.table_schema AND c2.table_name+@MigrationPreviousTableSuffix=c1.table_name and c2.column_name=c1.column_name)
WHERE c2.data_type<>c1.data_type AND c1.DATA_TYPE = ''datetimeoffset'' and 
c2.table_name not like ''%'' + @MigrationPreviousTableSuffix

--and c1.TABLE_NAME like ''tbltransactions_date%''

ORDER BY c1.table_schema,c1.table_name,c1.column_name


DECLARE TableCursor SCROLL CURSOR FOR
	SELECT DISTINCT TableSchema,TableName,SiteIndexLevel, CASE TableName WHEN ''tblExportResults'' THEN 3 WHEN ''tblExportInterfaceResult'' THEN 3  WHEN ''tblTransactions'' THEN 1 WHEN ''tblTransactionLineItems'' THEN 2 WHEN ''tblExportResultDetails'' THEN 4 ELSE 5  END as ProcessOrder
	FROM @ConvertDate
	ORDER BY ProcessOrder,TableSchema,TableName
OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel, @ProcessOrder
WHILE @@FETCH_STATUS=0
BEGIN

	declare @isnewdateoffsetcol bit
		,	@IsNewColumn bit
		,	@SqlInsertColumns NVARCHAR(max)
		,	@SqlInsertVals NVARCHAR(max)
		,	@SqlTdzJoins NVARCHAR(max)
		,	@tzdCount INT

		SET @SqlInsertColumns = '''';
		SET @SqlInsertVals = '''';
		SET @SqlTdzJoins = '''';
		SET @isnewdateoffsetcol = 0;
		SET @tzdCount = 0;
		SET @MigrateTableName = replace(@TableName,@MigrationPreviousTableSuffix,'''') + @MigrationTableSuffix

		-- create new table structure to match
		SET @Sql = ''IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME=''''''+ @MigrateTableName + '''''' AND TABLE_SCHEMA='''''' + @SchemaName + '''''') DROP TABLE ['' + @SchemaName + ''].['' + @MigrateTableName + ''] 
					SELECT *  INTO [''+@SchemaName+''].[''+@MigrateTableName+'']  FROM FuelsManagerDB_Template.[''+@SchemaName+''].[''+replace(@TableName, @MigrationPreviousTableSuffix, '''')+''] WHERE 1=0''
		PRINT @Sql
		EXEC sp_executesql @statment=@Sql

		-----***************************************************
		----alter tables here to save time later on (Adding columns that exist in consoildated db, they will be dropped later)
		-----***************************************************
				DECLARE @AlterColumn NVARCHAR(500)
			,	@AlterType NVARCHAR(500)
			,	@AlterDefault NVARCHAR(2000)
			,	@AlterNullable VARCHAR(50)
			,	@AlterMaxLength INT
			,	@AlterPrecision INT
			,	@AlterPrecisionRadix INT
			,	@AlterSql NVARCHAR(max)



			-- ADJUST DATA TYPES WHCIH DID NOT GET CREATED BASED ON USER DEFINED DATA TYPE

		declare @IsNullable bit
			-- ADJUST DATA TYPES WHCIH DID NOT GET CREATED BASED ON USER DEFINED DATA TYPE

			DECLARE ObjCursor CURSOR FOR
				SELECT cl1.name as ColumnName,tp1.name as UserType,cl1.IS_NULLABLE AS IsNullable
				FROM FuelsManagerDB_Template.sys.tables tb1
				INNER JOIN FuelsManagerDB_Template.sys.columns cl1 on cl1.object_id=tb1.object_id
				INNER JOIN FuelsManagerDB_Template.sys.schemas sc1 on sc1.schema_id=tb1.schema_id
				INNER JOIN FuelsManagerDB_Template.sys.types tp1 on cl1.user_type_id=tp1.user_type_id
				WHERE cl1.system_type_id<> cl1.user_type_id
					AND tb1.name = @TableName AND sc1.name = @SchemaName
				ORDER BY cl1.name

			OPEN ObjCursor
			FETCH NEXT FROM ObjCursor INTO @AlterColumn,@AlterType, @IsNullable
			WHILE @@FETCH_STATUS=0
			BEGIN
				SET @AlterSql=''ALTER TABLE [''+@SchemaName+''].[''+@MigrateTableName+''] ALTER COLUMN [''+@AlterColumn+''] ''+@AlterType+'' '' + iif(@IsNullable = 1, '' NULL '', '' NOT NULL '') + '';''
				PRINT @AlterSql
				EXEC sp_executesql @statment=@Sql

				FETCH NEXT FROM ObjCursor INTO @AlterColumn,@AlterType,@IsNullable
			END
			CLOSE ObjCursor
			DEALLOCATE ObjCursor


			---***************************************************
			--- END ALTER new MIGRATION TABLE
			---***************************************************

		--- GENERATE DATA FOR COLUMNS
		DECLARE ColumnCursor SCROLL CURSOR FOR
		SELECT	c2.column_name
			,	case when c2.data_type = ''datetimeoffset'' and c2.DATA_TYPE <> c1.DATA_TYPE then 1 else 0 end
			, case when c1.COLUMN_NAME IS NULL THEN 1 ELSE 0 END
		FROM ConsolidatedDB.information_schema.columns c2 -- newly built table with all columns
			LEFT JOIN ConsolidatedDB.information_schema.columns c1 --old table that might need conversion
				ON c1.TABLE_SCHEMA = c2.TABLE_SCHEMA AND c1.TABLE_NAME +@MigrationTableSuffix = c2.TABLE_NAME  and c2.column_name=c1.column_name
		WHERE c2.TABLE_SCHEMA = @SchemaName and c2.TABLE_NAME = @MigrateTableName
			AND c2.column_name <> ''_Rowversion''
		ORDER BY c1.column_name
		OPEN ColumnCursor
		FETCH NEXT FROM ColumnCursor INTO @ColumnName,@isnewdateoffsetcol, @IsNewColumn
		WHILE @@FETCH_STATUS=0
		BEGIN

			--print @tablename+''.''+@columnname 

			IF @isnewdateoffsetcol = 1
			BEGIN

				---- alter new table 
				--SET @Sql = ''ALTER TABLE [''+@SchemaName+''].[''+@TableName+''_DateMigrate] ALTER COLUMN [''+@ColumnName+''] datetimeoffset(7);''
				--PRINT @Sql
				--EXEC sp_executesql @statment=@Sql

				declare @tdzAlias nvarchar(10)
				SET @tzdCount = @tzdCount + 1;
				SET @tdzAlias = ''tdz'' + cast (@tzdCount as nvarchar(5));

				-- Learning from BSM-E, older versions of FuelsManager stored everything in UTC/GMT so just leave the offset at +0.00 and DO NOT adjust the DateTime to the Site timezone.  It will happen
				-- automatically.
				--SET @SqlInsertVals = @SqlInsertVals + '' TODATETIMEOFFSET(tb1.[''+@ColumnName+''],case when '' + @tdzAlias + '' .Offset is null or tz.AdjustForDaylightSavings = 0 then tz.UTCOffSet else tz.utcoffset + '' + @tdzAlias + ''.Offset end), ''

				-- Keep the original UTC offset of 0
				SET @SqlInsertVals = @SqlInsertVals + '' TODATETIMEOFFSET(tb1.[''+@ColumnName+''], 0),''

				SET @SqlTdzJoins = @SqlTdzJoins + '' left join tbltimezonedstoffsets '' +@tdzAlias + '' on tb1.[''+ @ColumnName +''] IS NOT NULL AND tz.[Index] = '' +@tdzAlias + ''.[TimeZoneIndex] AND tb1.[''+ @ColumnName +''] between '' +@tdzAlias + ''.StartTime AND '' +@tdzAlias + ''.EndTime ''
			END
			ELSE
			BEGIN
				DECLARE @CurInsertVal nvarchar(max)
				SET @CurInsertVal = ''tb1.['' + @ColumnName + ''],''

				SET @SqlInsertVals = @SqlInsertVals + @CurInsertVal
			END	
			
			SET @SqlInsertColumns = @SqlInsertColumns + ''['' + @ColumnName + ''],'';
			

			FETCH NEXT FROM ColumnCursor INTO @ColumnName,@isnewdateoffsetcol, @IsNewColumn
		END
			
		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor

				
		DECLARE @siteQualifiedColumn nvarchar(50), @caseJoin nvarchar(max)

				
			--remove trailing commas
			SET @SqlInsertColumns = LEFT(@SqlInsertColumns, len(@SqlInsertColumns) -1);
			SET @SqlInsertVals = LEFT(@SqlInsertVals, len(@SqlInsertVals) - 1);
			SET @Sql = ''''
			DECLARE @hasIdentity bit
			SET @hasIdentity = 0

			IF EXISTS( select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = @SchemaName and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, ''IsIdentity'') = 1 AND TABLE_NAME = @MigrateTableName)
			BEGIN
				SET @hasIdentity = 1
			END

			IF (@hasIdentity = 1)
			BEGIN
				SET @Sql = ''SET IDENTITY_INSERT [''+@SchemaName+''].[''+@MigrateTableName+''] ON ''
			END

			SET @Sql = @Sql + ''
				INSERT INTO [''+@SchemaName+''].[''+@MigrateTableName+''] WITH (TABLOCK) ('' + @SqlInsertColumns + '')
				SELECT '' + @SqlInsertVals + '' FROM [''+@SchemaName+''].[''+@TableName+''] tb1 '' 
				+ ''	inner '' 
				
			if @tablename <> ''tblTransactionLineItems''
			BEGIN
				set @sql += '' MERGE ''
			END

			SET @sql += '' join tblSiteTimeZones tz on  tb1.SiteIndex = tz.SiteIndex '' +  @SqlTdzJoins 
						
			IF (@hasIdentity = 1)
			BEGIN
				SET @Sql = @Sql + ''
					SET IDENTITY_INSERT [''+@SchemaName+''].[''+@MigrateTableName+''] OFF ''
			END
			
			PRINT cast(@Sql as ntext)
			EXEC sp_executesql @statment=@Sql


			SET @Sql = ''
				DROP TABLE [''+@SchemaName+''].[''+replace(@TableName,@MigrationPreviousTableSuffix,'''')+'']
				EXEC sp_rename ''''[''+@SchemaName+''].[''+@MigrateTableName+'']'''', ''''''+replace(@TableName,@MigrationPreviousTableSuffix,'''')+''''''''
			print @sql
			EXEC sp_executesql @statment=@Sql


	FETCH NEXT FROM TableCursor INTO @SchemaName,@TableName,@SiteIndexLevel, @ProcessOrder
END
CLOSE TableCursor
DEALLOCATE TableCursor


PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00077 Cleanup Special Tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00077 Cleanup Special Tables', 
		@step_id=39, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
THIS FILE MUST BE RECREATED AFTER ANY DATABASE UPDATES OR PRIOR TO DEPLOYMENT

*/
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblSites'') DROP INDEX [ix_migrate_tblSites] ON [dbo].[tblSites] 
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblEquipment'') DROP INDEX [ix_migrate_tblEquipment] ON [dbo].[tblEquipment] 
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblCompanies'') DROP INDEX [ix_migrate_tblCompanies] ON [dbo].[tblCompanies]
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblProducts'') DROP INDEX [ix_migrate_tblProducts] ON [dbo].[tblProducts]
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblTanks'') DROP INDEX [ix_migrate_tblTanks] ON [dbo].[tblTanks] 
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblFuelCards'') DROP INDEX [ix_migrate_tblFuelCards] ON [dbo].[tblFuelCards] 
IF EXISTS(select TOP 1 1 from sys.indexes where name=''ix_migrate_tblAdditiveProfiles'') DROP INDEX [ix_migrate_tblAdditiveProfiles] ON [dbo].[tblAdditiveProfiles] 
GO

-- create defaults for our special tables since they were left out before
DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(500)
	,	@DefaultName NVARCHAR(1000)
	,	@Definition NVARCHAR(max)
	,	@Column NVARCHAR(1000)
	,	@Sql NVARCHAR(max)

DECLARE ObjCursor CURSOR FOR
	SELECT	sch.name as SchemaName
		,	obj.name as TableName
		,	col.name as ColumnName
		,	tb1.name as DefaultName
		,	tb1.[Definition]
	FROM FuelsManagerDB_Template.sys.default_constraints tb1
	INNER JOIN FuelsManagerDB_Template.sys.objects obj ON obj.object_id=tb1.parent_object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sch ON obj.schema_id=sch.schema_id
	INNER JOIN FuelsManagerDB_Template.sys.columns col ON (col.object_id=tb1.parent_object_id AND col.column_id=tb1.parent_column_id)
	where not exists(
		SELECT 1 
		FROM sys.default_constraints tb2
		--INNER JOIN sys.objects obj2 ON obj2.object_id=tb2.object_id
		--INNER JOIN FuelsManagerDB_Template.sys.schemas sch2 ON obj2.schema_id=sch2.schema_id
		--INNER JOIN FuelsManagerDB_Template.sys.columns col2 ON (col2.object_id=tb2.parent_object_id AND col.column_id=tb2.parent_column_id)
		WHERE tb2.name=tb1.name)
		--WHERE obj2.name=obj.name
		--AND sch2.name=sch.name
		--AND col2.name=col.name
		--AND tb1.definition=tb2.definition)
		--vivian added
	AND NOT  (tb1.name LIKE  ''DF__TT_PointT__Silen%'')
	ORDER BY sch.name,obj.name,col.name

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@DefaultName,@Definition
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql= ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD CONSTRAINT [''+@DefaultName+''] DEFAULT ''+@Definition+'' FOR [''+@Column+''] ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM ObjCursor INTO @Schema,@Table,@Column,@DefaultName,@Definition
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


PRINT ''Completed successfully''
GO


PRINT ''''
PRINT ''************************************''
PRINT ''** PROCESS COMPLETE''
PRINT ''************************************''	

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00080 Drop Obsolete Tables And Columns]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00080 Drop Obsolete Tables And Columns', 
		@step_id=40, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
/*
BSM-E 8.0 SP4 Database Upgrade Process
Script TAS UPG 00080 Drop Obsolete Tables
*/
SET NOCOUNT ON;

DECLARE @Schema NVARCHAR(200)
	,	@Table NVARCHAR(300)
	,	@Sql NVARCHAR(max)
	,	@Column NVARCHAR(300)
	,	@Index NVARCHAR(500)
	,	@Type INT

-- DROP REMAINING INDEXES AND PK CONSTRAINTS
DECLARE DropCursor CURSOR FOR
	SELECT	s.name as SchemaName
		,	o.name As TableName
		,	i.name As IndexName
		,	i.[Type] AS IndexType
	FROM sys.indexes i
	INNER JOIN sys.tables o on o.object_id=i.object_id
	INNER JOIN sys.schemas s ON s.schema_id=o.schema_id
	WHERE o.name is not null
	and i.type NOT IN(0)
	ORDER BY s.name,o.name,i.name

OPEN DropCursor
FETCH NEXT FROM DropCursor INTO @Schema,@Table,@Index,@Type
WHILE @@FETCH_STATUS=0
BEGIN
	IF @Type = 1
		SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP CONSTRAINT [''+@Index+''];''
	ELSE
		SET @Sql=''DROP INDEX [''+@Schema+''].[''+@Table+''].[''+@Index+'']; ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM DropCursor INTO @Schema,@Table,@Index,@Type
END
CLOSE DropCursor
DEALLOCATE DropCursor

-- DROP OBSOLETE TABLES
DECLARE DropCursor CURSOR FOR
	SELECT	Table_Schema, Table_Name
	FROM	ConsolidatedDB.INFORMATION_SCHEMA.TABLES s
	WHERE	NOT EXISTS
		(	SELECT 1
			FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.TABLES t
			WHERE	t.Table_Name=s.Table_Name
			AND		t.Table_Schema=s.Table_Schema
		)
	ORDER BY s.Table_Schema,s.Table_Name
	
OPEN DropCursor
FETCH NEXT FROM DropCursor INTO @Schema,@Table
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql = ''DROP TABLE [''+@Schema+''].[''+@Table+''];''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM DropCursor INTO @Schema,@Table
END
CLOSE DropCursor
DEALLOCATE DropCursor

-- DROP OBSOLETE COLUMNS
DECLARE DropCursor CURSOR FOR
	SELECT Table_Schema,Table_Name,Column_Name
	FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS s
	WHERE NOT EXISTS
	(	SELECT 1
		FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS t
		WHERE t.Table_Schema=s.Table_Schema
		AND	t.Table_Name=s.Table_Name
		AND	t.Column_Name=s.Column_Name
	)
	ORDER BY Table_Schema,Table_Name,Column_Name

OPEN DropCursor
FETCH NEXT FROM DropCursor INTO @Schema,@Table,@Column
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column+'']; ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM DropCursor INTO @Schema,@Table,@Column
END
CLOSE DropCursor
DEALLOCATE DropCursor

PRINT ''Completed successfully''
GO


', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00085 Adjust Non Nullable Columns]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00085 Adjust Non Nullable Columns', 
		@step_id=41, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
--
-- ADJUST NON-NULLABLE COLUMNS
--

UPDATE dbo.tblSites SET UseTankReconciliation = 0 where UseTankReconciliation IS NULL
UPDATE dbo.tblSites SET MeterReconciliationToleranceIsPercent = 0 where MeterReconciliationToleranceIsPercent IS NULL
UPDATE dbo.tblSites SET AllowUseOfSpecialChars = 1 where AllowUseOfSpecialChars IS NULL
UPDATE dbo.tblSites SET EnablePeriodicSyncFlag = 0 where EnablePeriodicSyncFlag IS NULL
UPDATE dbo.tblSites SET PeriodicSyncIntervalMinutes = 0 where PeriodicSyncIntervalMinutes IS NULL
UPDATE dbo.tblTransactionAliases SET IncludeInDispatch = 0 where IncludeInDispatch IS NULL
UPDATE dbo.tblTransactionAliasFields SET ClearOnNew = 0 where ClearOnNew IS NULL

UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 2 WHERE CompanyBillToToShipperGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 0 WHERE CompanyLoadOwnerToManagerGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 14 WHERE CompanyOffLoadOwnerToManagerGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 1 WHERE CompanyShipperToOwnerGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 3 WHERE CompanyShipToToBillToGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 11 WHERE CompanySupplierToOwnerGuid IS NOT NULL
UPDATE [dbo].[tblAllocations] SET LookupCompanyMapTypeIndex = 10 WHERE CompanyBillToToShipperGuid IS NULL AND CompanyLoadOwnerToManagerGuid IS NULL AND CompanyOffLoadOwnerToManagerGuid IS NULL AND CompanyShipperToOwnerGuid IS NULL AND CompanyShipToToBillToGuid IS NULL AND CompanySupplierToOwnerGuid IS NULL

UPDATE [dbo].[tblAllocationLineItems] SET LookupAllocationTypeIndex = 2 WHERE LookupAllocationTypeIndex IS NULL
UPDATE [dbo].[tblAllocationLineItems] SET LookupResetMethodIndex = 4 WHERE LookupResetMethodIndex IS NULL
UPDATE [dbo].[tblAllocationLineItems] SET LookupResetPeriodIndex = 3 WHERE LookupResetPeriodIndex IS NULL

UPDATE dbo.tblArchivedUsers
SET SiteGuid=''00000000-0000-0000-0000-000000000001''
WHERE SiteGuid IS NULL

UPDATE dbo.tblChangesQueue
SET SiteGuid=''00000000-0000-0000-0000-000000000001''
WHERE SiteGuid IS NULL

UPDATE [dbo].[tblArchivedUsers]
SET [UserGuid]=NEWID()
WHERE [UserGuid] IS NULL

UPDATE [dbo].[tblAuditLog]
SET [AuditedDate]=[CreatedDate]
WHERE [AuditedDate] IS NULL

-- sp_help ''erv.tblTempVersionSpecificField''
DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(500)
	,	@Column NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@Count INT
	,	@DataType NVARCHAR(500)
	,	@MaxLen INT
	,	@IsNullable nvarchar(5)
--
-- ADJUST NON-NULLABLE COLUMNS

SELECT	distinct tmp.TABLE_SCHEMA as SchemaName
	,	tmp.TABLE_NAME as TableName
	,	tmp.column_NAME as ColumnName
	,	tmp.DATA_TYPE as DataType
	,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
	,	tmp.IS_NULLABLE AS IsNullable
into #Clusteredidx
FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
where tmp.column_NAME like  ''%_ClusterIdx'' and tmp.TABLE_NAME  NOT LIKE ''VW_%''


--select * from #Clusteredidx
--SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME LIKE ''%_CLUSTERIDX''

DECLARE ColumnCursor CURSOR FOR
	SELECT	tmp.TABLE_SCHEMA as SchemaName
		,	tmp.TABLE_NAME as TableName
		,	tmp.column_NAME as ColumnName
		,	tmp.DATA_TYPE as DataType
		,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
		,	tmp.IS_NULLABLE AS IsNullable
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
	--INNER JOIN INFORMATION_SCHEMA.COLUMNS upg 
	--	ON(		upg.TABLE_SCHEMA=tmp.TABLE_SCHEMA
	--		AND	upg.TABLE_NAME=tmp.TABLE_NAME
	--		AND	upg.COLUMN_NAME=tmp.COLUMN_NAME)
	--WHERE tmp.IS_NULLABLE<>upg.IS_NULLABLE
	--/** Vivian added the filter out for _clusteridx because you cannnot alter the column with existing nullable data to an identity field***/
	--/** Note - This does not resolve the ommitted fields == look at later ***/
WHERE  tmp.column_NAME LIKE ''%_ClusterIdx''
	--,''ID'',''LookupPresetTypeIndex'',''AutomaticCloseout'',''SystemQuery'',''Enterprise'',''GlobalAccessToEquipment'',''GlobalAccessToPersonnel'',''OperateTabGroups'',''LookupStationInterfaceTypeIndex'',''LookupStationTypeIndex'',''PromptForGravityCaptured'',''PromptForTemperatureCaptured'')
	and ISNULL(tmp.CHARACTER_MAXIMUM_LENGTH,0)<>-1
	and NOT (tmp.TABLE_NAME = ''tblAlarmAndEventLog'' and tmp.COLUMN_NAME = ''AssociatedData'')
	AND NOT (tmp.table_schema = ''dbo'' and tmp.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY tmp.TABLE_SCHEMA,tmp.TABLE_NAME,tmp.column_NAME
	OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
WHILE @@FETCH_STATUS=0
BEGIN



--SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  COLUMN_NAME 	LIKE ''%_ClusterIdx'' and table_schema like ''dbo'' order by 3 asc

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  COLUMN_NAME =@Column
    AND TABLE_NAME = @Table
    AND TABLE_SCHEMA = @Schema
	and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, ''IsIdentity'')<>1 and TABLE_NAME  NOT LIKE ''VW_%'' )
	BEGIN
	
		SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
			PRINT @Sql
		EXEC sp_executesql @Statement=@Sql
		--	SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
END	
		--PRINT @Sql
		--EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor


DECLARE @Schema1 NVARCHAR(100)
	,	@Table1 NVARCHAR(500)
	,	@Column1 NVARCHAR(500)
	,	@Sql1 NVARCHAR(max)
	,	@Count1 INT
	,	@DataType1 NVARCHAR(500)
	,	@MaxLen1 INT
	,	@IsNullable1 nvarchar(5)


DECLARE ColumnCursor CURSOR FOR
SELECT	tmp.SchemaName
		,	tmp.TableName
		,	tmp.ColumnName
		,	tmp.DataType
		,	tmp.MaxLen
		,	tmp.IsNullable

FROM    #Clusteredidx tmp
LEFT JOIN INFORMATION_SCHEMA.COLUMNS upg 
		ON(	tmp.SchemaName=upg.TABLE_SCHEMA
			AND		tmp.TableName=upg.TABLE_NAME
			AND	tmp.ColumnName=upg.COLUMN_NAME)
	--WHERE COLUMNPROPERTY(object_id(upg.TABLE_NAME), upg.COLUMN_NAME, ''IsIdentity'')<>1  
	ORDER BY tmp.SchemaName,tmp.TABLENAME,tmp.columnNAME
	OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema1,@Table1,@Column1,@DataType1,@MaxLen1, @IsNullable1
WHILE @@FETCH_STATUS=0
BEGIN

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  COLUMN_NAME =@Column1
    AND TABLE_NAME = @Table1
    AND TABLE_SCHEMA = @Schema1
	--and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, ''IsIdentity'')<>1  
	)
	BEGIN
	
		SET @SQL1 = ''ALTER TABLE [''+@Schema1+''].[''+@Table1+''] ADD [''+@Column1+''] INT IDENTITY ''
			PRINT @Sql1
		EXEC sp_executesql @Statement=@Sql1
		--	SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
END	


--BEGIN
--IF @Column LIKE ''%_ClusterIdx''
--		--SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
--		--	PRINT @Sql
--		--EXEC sp_executesql @Statement=@Sql
--			SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
		
--		PRINT @Sql
--		EXEC sp_executesql @Statement=@Sql
FETCH NEXT FROM ColumnCursor INTO @Schema1,@Table1,@Column1,@DataType1,@MaxLen1, @IsNullable1
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor

DECLARE ColumnCursor CURSOR FOR
	SELECT	tmp.TABLE_SCHEMA as SchemaName
		,	tmp.TABLE_NAME as TableName
		,	tmp.column_NAME as ColumnName
		,	tmp.DATA_TYPE as DataType
		,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
		,	tmp.IS_NULLABLE AS IsNullable
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
	INNER JOIN INFORMATION_SCHEMA.COLUMNS upg 
		ON(		upg.TABLE_SCHEMA=tmp.TABLE_SCHEMA
			AND	upg.TABLE_NAME=tmp.TABLE_NAME
			AND	upg.COLUMN_NAME=tmp.COLUMN_NAME)
	WHERE tmp.IS_NULLABLE<>upg.IS_NULLABLE
	/** Vivian added the filter out for _clusteridx because you cannnot alter the column with existing nullable data to an identity field***/
	/** Note - This does not resolve the ommitted fields == look at later ***/
	AND tmp.column_NAME NOT IN (''%_ClusterIdx'',''ListViewID'',''ID'',''LookupPresetTypeIndex'',''AutomaticCloseout'',''SystemQuery'',''Enterprise'',''GlobalAccessToEquipment'',''GlobalAccessToPersonnel'',''OperateTabGroups'',''LookupStationInterfaceTypeIndex'',''LookupStationTypeIndex'',''PromptForGravityCaptured'',''PromptForTemperatureCaptured'')
	and ISNULL(tmp.CHARACTER_MAXIMUM_LENGTH,0)<>-1
	and NOT (tmp.TABLE_NAME = ''tblAlarmAndEventLog'' and tmp.COLUMN_NAME = ''AssociatedData'')
	AND NOT (tmp.table_schema = ''dbo'' and tmp.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY tmp.TABLE_SCHEMA,tmp.TABLE_NAME,tmp.column_NAME

OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
WHILE @@FETCH_STATUS=0
BEGIN
	IF (@Column=''CreatedBy'') OR (@Column=''UpdatedBy'')
	BEGIN
		SET @DataType=''UdtUserID'';
		SET @MaxLen=NULL;
	END

	IF @Column= ''SiteGuid''
	BEGIN
		SET @Sql=''UPDATE  [''+@Schema+''].[''+@Table+''] SET [''+@Column+''] = ''''00000000-0000-0000-0000-000000000001'''' WHERE [''+@Column+''] IS NULL''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

	END 

	--/***  VIVIAN ADDED THIS PORTION FOR CLUSTERIDX COLUMNS ****/
	--	IF @Column LIKE ''%_ClusterIdx''
	--BEGIN
		
	--	SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
	--		PRINT @Sql
	--	EXEC sp_executesql @Statement=@Sql
	--		SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
		
	--	PRINT @Sql
	--	EXEC sp_executesql @Statement=@Sql

	--END


	IF @Column = ''PinNumber'' and @Table = ''tblPersonnel''
	BEGIN
		
		SET @SQL = ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''_Temp] ''+@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''')
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql
		
		SET @SQL = '' UPDATE [''+@Schema+''].[''+@Table+''] SET [''+@Column+''_Temp] = CONVERT('' +@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''') + '', ['' + @Column + '']); ''
		SET @SQL = @SQL + '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

		SET @SQL = '' EXEC sp_rename '''''' + @Table + ''.'' + @Column+''_Temp'''', '''''' + @Column + '''''', ''''COLUMN''''; ''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

	END

	SET @Sql = ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ALTER COLUMN [''+@Column+''] ''+@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''') + iif(@IsNullable = ''YES'', '' NULL '', '' NOT NULL '')
	
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor

-- EXCEPTION: THE ABOVE SCRIPT FAILED TO CHANGE THE BELOW COLUMN BECAUSE IT DOES NOT HANDLE "MAX" ON NVARCHAR COLUMN
PRINT ''ALTER TABLE [dbo].[tblAlarmAndEventLog] ALTER COLUMN [AssociatedData] nvarchar(MAX) NOT NULL''

ALTER TABLE [dbo].[tblAlarmAndEventLog] ALTER COLUMN [AssociatedData] nvarchar(MAX) NOT NULL 
GO

-- Fix OnFileSignature column of personnel table; needs to be converted from image to varbinary(max)
PRINT ''ALTER TABLE [dbo].[tblPersonnel] ALTER COLUMN [OnFileSignature] varbinary(max) NULL''

ALTER TABLE [dbo].[tblPersonnel] ALTER COLUMN [OnFileSignature] varbinary(max) NULL 
GO

PRINT ''Completed successfully''


GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00088 Perform Data Modifications]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00088 Perform Data Modifications', 
		@step_id=42, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
-- Remove the rights related to end of month approval from any group that currently has them.
-- The code has been changed so that these rights should no longer be available for assignment in the GUI.
DELETE FROM map.tblGroupToRight WHERE map.tblGroupToRight.LookupRightIndex IN 
(SELECT lookup.tblRight.RightIndex 
FROM lookup.tblRight 
WHERE lookup.tblRight.RightName LIKE ''EOM_APPROVAL_%'')


-- Remove certain rights from any group that currently has them.
-- The code has been changed so that these rights should no longer be available for assignment in the GUI.
DELETE FROM [map].[tblGroupToRight] WHERE [map].[tblGroupToRight].[LookupRightIndex] IN 
(SELECT [lookup].[tblRight].[RightIndex] 
    FROM [lookup].[tblRight] 
        WHERE [lookup].[tblRight].[RightCode] IN (''PERFORM_PRODUCT_UPDATE''
                                                  ,''MODIFY_ORDERS''
                                                  ,''VIEW_ORDERS''
                                                  ,''CREATE_ORDERS''
                                                  ,''MODIFY_PAYMENT_DATA''
                                                  ,''VIEW_RECOVERY_DATA''
                                                  ,''MODIFY_RECOVERY_DATA''
                                                  ,''VIEW_SUPPLY_ORDERS''
                                                  ,''CREATE_SUPPLY_ORDERS''
                                                  ,''MODIFY_SUPPLY_ORDERS''
                                                  ,''CREATE_ADJUSTMENT''
                                                  ,''MODIFY_ADJUSTMENT''))


 -- 53408 - Password Complexity error incorrect
 UPDATE [dbo].[tblSites] SET [StrongPwdUse]=2, [CheckForPreviousPwd]=1, UpdatedDate = SYSDATETIMEOFFSET() WHERE StrongPwdUse <> 2 OR CheckForPreviousPwd <> 1
 UPDATE dbo.tblsites set NumberPrefix = ''E'' + numberprefix from tblsites WHERE NumberPrefix not like ''E%'' --enterprise doc numbers (Value not synced)

 --WI 54115
DELETE FROM [map].[tblGroupToRight] 
WHERE LookupRightIndex in ( 
116,117,	-- incoming truck data
154, 155,	-- price list
33, 34,		-- ticketing data
101, 102,	-- Wac
146, 147,	-- field level configurations
7			-- installed module status
)

-- 53240 - Receive Column does not appear within the ledger view
PRINT ''Adding Receive and Shipment column to Ledger for Aviation Products and DOD Standard''
DELETE FROM [dbo].[tblListViewFields] WHERE LookupListViewFieldTypeIndex=1 AND TransactionAliasGuid = (SELECT TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE aliasname=''Request'')

INSERT INTO [dbo].[tblListViewFields]
(ColumnOrder, ListViewID, ListViewFieldGuid, lookuplistviewfieldtypeindex, listviewguid, LedgerAggregateColumnGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
SELECT 2, v.ID as listviewid, convert(uniqueidentifier, 
							hashbytes(''md5'', --ok
							convert(varchar(36), 6) +					-- [LookupListViewFieldTypeIndex]
							''00000000-0000-0000-0000-000000000000'' +	-- [TransactionAliasGuid] used when LookupListViewFieldTypeIndex =1
							''00000000-0000-0000-0000-000000000000'' +	-- [TransactionAliasFieldGuid] used when LookupListViewFieldTypeIndex =2
							''00000000-0000-0000-0000-000000000000'' +	-- UserDataFieldTransactionAliasGuid used when LookupListViewFieldTypeIndex=3 
							convert(varchar(36), 0) +					-- [LookupStandardViewFieldTypeIndex] used then LookupListViewFieldTypeIndex=4
							''00000000-0000-0000-0000-000000000000'' +	-- UserDataFieldTransactionAliasLineItemGuid used when LookupListViewFieldTypeIndex=5 
							convert(varchar(36), a.LedgerAggregateColumnGuid) +	-- LedgerAggregateColumnGuid used when LookupListViewFieldTypeIndex=6 
							convert(varchar(36), v.ListViewGuid)
							))	AS [ListViewFieldGuid],  
6 as lookuplistviewfieldtypeindex, listviewguid, a.LedgerAggregateColumnGuid, SYSDATETIMEOFFSET(), ''Varec'' , SYSDATETIMEOFFSET(), ''Varec'' 
FROM dbo.tblListViews v, dbo.tblLedgerAggregateColumns a WHERE a.ID IN (''Shipment'', ''Receive'') and v.ID IN (''Aviation Products'',''DOD Standard'')
AND NOT EXISTS(SELECT * FROM [dbo].[tblListViewFields] l JOIN dbo.tblLedgerAggregateColumns c ON c.LedgerAggregateColumnGuid=l.LedgerAggregateColumnGuid
WHERE a.LedgerAggregateColumnGuid = c.LedgerAggregateColumnGuid AND v.Id=l.ListViewID) 


-- WI 53036 Unobtainable within Product Configuration
Print ''Allow Unobtainable to be editable in Product Configuration.''


BEGIN TRANSACTION
DECLARE @T TABLE (EntitySegmentTemplateGuid UNIQUEIDENTIFIER, SiteGroupGuid UNIQUEIDENTIFIER)

INSERT INTO @T (EntitySegmentTemplateGuid, SiteGroupGuid)
SELECT EntitySegmentTemplateGuid, SiteGuid
	FROM dbo.tblSites s, erv.tblEntitySegmentTemplate e
	WHERE s.SiteGroupFlag=1 AND e.EntityTypeId=''Product''
	AND NOT EXISTS(SELECT TOP 1 1 FROM  [erv].[tblEntityRecordVersioningFieldConfig] t WHERE
		t.EntitySegmentTemplateGuid=e.EntitySegmentTemplateGuid AND s.SiteGuid=t.SiteGroupGuid 
		AND TargetField=''UserData5'') 

INSERT INTO [erv].[tblEntityRecordVersioningFieldConfig]
(FieldConfigGuid, EntitySegmentTemplateGuid, SiteGroupGuid, TargetField, IsExternalAttribute, ForwardControlMode,
CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT convert(uniqueidentifier, --ok
				hashbytes(''md5'',	
						(''UserData5''+ --TargetField = Product Unobtainable
						convert(varchar(36), SiteGroupGuid)+ --
						convert(varchar(36), EntitySegmentTemplateGuid)))), 
						EntitySegmentTemplateGuid, 
						SiteGroupGuid, ''UserData5'', 0, ''VersionSpecific'', 
						SYSDATETIMEOFFSET(), ''V9 Upgrade. AAC'',  SYSDATETIMEOFFSET(), ''V9 Upgrade. AAC''
	FROM @T 

DECLARE initCur CURSOR FOR SELECT EntitySegmentTemplateGuid, SiteGroupGuid    FROM  @T 

DECLARE @EntitySegmentTemplateGuid UNIQUEIDENTIFIER
DECLARE @SiteGroupGuid UNIQUEIDENTIFIER
OPEN initCur
FETCH FROM initCur INTO @EntitySegmentTemplateGuid, @SiteGroupGuid
WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC FuelsManagerDB_Template.[erv].[usp_EnforceFLCChangesOnProductRecordVersioning]	@EntitySegmentTemplateGuid, @SiteGroupGuid, ''V9 Upgrade. AAC'', ''OFF_TO_ON''
	FETCH FROM initCur INTO @EntitySegmentTemplateGuid, @SiteGroupGuid
END
CLOSE initCur
DEALLOCATE initCur
COMMIT TRANSACTION


--this is breaking the contraint on tblpersonnel (TAS will not see this since we migrate individual terminals, we will clean up collisions later on in the migration)
--update tblpersonnel set shortcardnumber = null --where shortcardnumber in (1,2,3,0)
--update tblpersonnel set cardnumber = null 


-- v7.5 DID NOT REQUIRE THE EQUIPMENT ID TO BE UNIQUE SO MOST OF THE COMPARTMENT RECORDS USED THE SAME Format of Compartment 1, Compartment 2, Compartment x..  v10.0 REQUIRES A UNIQUE EQUIPMENT ID SO WE
-- WILL MAKE ALL COMPARTMENT RECORDS USE THE FOLLOWING NAMING CONVENTION: ParentEquipmentID_EquipmentSequence
BEGIN TRANSACTION	
UPDATE [dbo].[tblEquipment] SET ID = el.E2ID FROM 
	(SELECT e2.[EquipmentGuid] ''E2Guid'', e1.ID + ''_'' + e2.EquipmentSequence ''E2ID'' 
		FROM [dbo].[tblEquipment] e1 
			INNER JOIN [dbo].[tblEquipment] e2
				ON e1.[EquipmentGuid] = e2.ParentEquipmentGuid
	) el
	WHERE [EquipmentGuid] = el.E2Guid

COMMIT TRANSACTION

-- v7.5 DID NOT REQUIRE THE QUALIFICATION ID TO BE UNIQUE SO THIS SCRIPT WILL APPEND A NUMBER TO ANY DUPLICATE IDs
BEGIN TRANSACTION
UPDATE dbo.tblQualifications SET ID = renamed.ID FROM
(SELECT CONCAT(duplicate.ID,''_'',CONVERT(varchar(8), duplicate.row+1)) AS ''ID'', duplicate.QualificationGuid 
	FROM (SELECT ID, QualificationGuid, row_number() OVER (ORDER BY ID) ''row'' FROM dbo.tblQualifications WHERE LookupQualificationTypeIndex IN (1,2) AND QualificationGuid IN (SELECT QualificationGuid FROM map.tblEntityEquipmentTestAndInspectionToSite)) main
	INNER JOIN (SELECT ID, QualificationGuid, row_number() OVER (PARTITION BY ID ORDER BY ID) ''row'' FROM dbo.tblQualifications WHERE LookupQualificationTypeIndex IN (1,2) AND QualificationGuid NOT IN (SELECT QualificationGuid FROM map.tblEntityEquipmentTestAndInspectionToSite)) duplicate
		ON main.ID = duplicate.ID
) renamed
WHERE dbo.tblQualifications.QualificationGuid = renamed.QualificationGuid;
COMMIT TRANSACTION


-- Clean up personnel records that reference themselves as their own supervisor 
BEGIN TRANSACTION
UPDATE dbo.tblPersonnel SET SupervisorPersonnelGuid = NULL WHERE PersonnelGuid = SupervisorPersonnelGuid
COMMIT TRANSACTION


PRINT ''Completed successfully''
GO


', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00090 Create Primary Keys]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00090 Create Primary Keys', 
		@step_id=43, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET NOCOUNT ON;


DECLARE @Schema NVARCHAR(300)
	,	@Table NVARCHAR(500)
	,	@Constraint NVARCHAR(1000)
	,	@Column NVARCHAR(500)
	,	@ColumnList NVARCHAR(MAX)
	,	@Sql NVARCHAR(MAX)
	,	@IdxType NVARCHAR(100)

DECLARE TableCursor CURSOR FOR
	SELECT	DISTINCT c.Table_Schema
		,	c.Table_Name
		,	c.Constraint_Name
		,	c.Column_Name
		,	ix.type_desc
	FROM  FuelsManagerDB_Template.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE c
	INNER JOIN FuelsManagerDB_Template.sys.key_constraints k on k.name=c.constraint_name
	INNER JOIN FuelsManagerDB_Template.sys.indexes ix ON ix.[name]=k.[name]
	WHERE k.[type]=''PK''
	AND c.table_name NOT IN (''tblCompanyCrossReference'',''tblCompanyCrossReferenceMap'', ''tblIMTankData'')
	ORDER BY c.Table_Schema,c.Table_Name,c.Column_Name
OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @Schema,@Table,@Constraint,@Column,@IdxType
WHILE @@FETCH_STATUS=0
BEGIN
	IF(SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@Schema AND TABLE_NAME=@Table AND COLUMN_NAME=@COLUMN)=''YES''
	BEGIN

			SET @Sql=''UPDATE [''+@Schema+''].[''+@Table+''] SET [''+@Column+'']=NEWID() WHERE [''+@Column+''] IS NULL;''
			PRINT @Sql
			EXEC sp_executesql @statement=@Sql
		
			SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] ALTER COLUMN [''+@Column+''] UNIQUEIDENTIFIER NOT NULL;''
			PRINT @Sql
			EXEC sp_executesql @statement=@Sql
		
	END
/************** Vivian added verification for existing Primary Keys... alter statement fails if PK exists so do we want to drop and create or leave as is??? *****/
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE  CONSTRAINT_TYPE = ''PRIMARY KEY''
 AND TABLE_NAME = @Table
 AND TABLE_SCHEMA = @Schema)
	BEGIN
	SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD CONSTRAINT [''+@Constraint+''] PRIMARY KEY ''+@IdxType+''([''+@Column+'']);''
	PRINT @Sql
	EXEC sp_executesql @statement=@sql
	END
	FETCH NEXT FROM TableCursor INTO @Schema,@Table,@Constraint,@Column,@IdxType
END
CLOSE TableCursor
DEALLOCATE TableCursor

PRINT ''--> Add contraint for table tblIMTankData that does not follow database recomendation for Primary key implementation''

/***** Vivian commented out because tblIMTankData is not in consolidateddb
--/****** Object:  Index [PK_tblIMTankData]    Script Date: 8/16/2013 11:34:32 PM ******/
--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblIMTankData]'') AND name = N''PK_tblIMTankData'')
--ALTER TABLE [dbo].[tblIMTankData] DROP CONSTRAINT [PK_tblIMTankData]
--GO


--/****** Object:  Index [PK_tblIMTankData]    Script Date: 8/16/2013 11:34:32 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblIMTankData]'') AND name = N''PK_tblIMTankData'')
--ALTER TABLE [dbo].[tblIMTankData] ADD  CONSTRAINT [PK_tblIMTankData] PRIMARY KEY CLUSTERED 
--(
--	[SiteGuid] ASC,
--	[SubSite] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
--GO


*****/

--PRINT ''--> Add contraint for table tblCompanyCrossReference that does not follow database recomendation for Primary key implementation''


--/****** Object:  Index [PK_tblCompanyCrossReference]    Script Date: 8/16/2013 11:34:32 PM ******/
--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblCompanyCrossReference]'') AND name = N''PK_tblCompanyCrossReference'')
--ALTER TABLE [dbo].[tblCompanyCrossReference] DROP CONSTRAINT [PK_tblCompanyCrossReference]
--GO


--/****** Object:  Index [PK_tblCompanyCrossReference]    Script Date: 8/16/2013 11:34:32 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblCompanyCrossReference]'') AND name = N''PK_tblCompanyCrossReference'')
--ALTER TABLE [dbo].[tblCompanyCrossReference] ADD  CONSTRAINT [PK_tblCompanyCrossReference] PRIMARY KEY CLUSTERED 
--(
--	[KeyName] ASC,
--	[ReferenceName] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
--GO


--PRINT ''--> Add contraint for table tblCompanyCrossReferenceMap that does not follow database recomendation for Primary key implementation''
--/****** Object:  Index [PK_tblCompanyCrossReferenceMap]    Script Date: 8/16/2013 11:35:17 PM ******/
--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblCompanyCrossReferenceMap]'') AND name = N''PK_tblCompanyCrossReferenceMap'')
--ALTER TABLE [dbo].[tblCompanyCrossReferenceMap] DROP CONSTRAINT [PK_tblCompanyCrossReferenceMap]
--GO

--/****** Object:  Index [PK_tblCompanyCrossReferenceMap]    Script Date: 8/16/2013 11:35:17 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[tblCompanyCrossReferenceMap]'') AND name = N''PK_tblCompanyCrossReferenceMap'')
--ALTER TABLE [dbo].[tblCompanyCrossReferenceMap] ADD  CONSTRAINT [PK_tblCompanyCrossReferenceMap] PRIMARY KEY CLUSTERED 
--(
--	[CompanyCrossReferenceMapGuid] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
--GO


PRINT ''Completed successfully''


GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00800 Initialize Default Synchronization State]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00091 Initialize Default Synchronization State', 
		@step_id=44, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'PRINT ''Creating Default Synchronization State.''
PRINT CHAR(13) + CHAR(10)

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
		,@SqlPKColumns nvarchar(4000)
		,@SqlPKData nvarchar(4000)
		,@Sql nvarchar(MAX)
		,@ProcessStartTime datetime2
		,@StepStartTime datetime2
		,@StepEndTime datetime2
		,@AffectedRecords int
		,@hasSiteGuid bit
		,@hasCreatedDate bit
		,@hasUpdatedDate bit
		,@specialForeignKey nvarchar(500)

DECLARE @PrimaryKeyColumnList AS TABLE
(
	SchemaName nvarchar(200)
	,TableName nvarchar(512)
	,PKColumnName nvarchar(384)
)

SET @AffectedRecords = 0
SET @ProcessStartTime = GETDATE()
SET @LastSchema = NULL
SET @LastTable = NULL

PRINT ''Initializing Tracking Tables''
PRINT CHAR(13) + CHAR(10)
PRINT ''*** Process started on ''+CAST(GETDATE() AS nvarchar(50))

PRINT ''*** Identifying primary keys...''
INSERT INTO @PrimaryKeyColumnList
	SELECT DISTINCT sch.name AS SchemaName
					,tab.name AS TableName
					,col.name AS PKColumnName
		FROM ConsolidatedDB.sys.tables tab
			INNER JOIN ConsolidatedDB.sys.schemas sch ON sch.schema_id=tab.schema_id
			INNER JOIN ConsolidatedDB.sys.columns col ON col.object_id=tab.object_id
			INNER JOIN ConsolidatedDB.sys.types typ ON typ.user_type_id=col.user_type_id
			LEFT JOIN ConsolidatedDB.sys.indexes idx ON idx.object_id=tab.object_id
			LEFT JOIN ConsolidatedDB.sys.index_columns icl ON (icl.object_id=idx.object_id AND icl.index_id=idx.index_id and icl.column_id=col.column_id)
		WHERE icl.object_id IS NOT NULL
			AND idx.is_primary_key=1
			AND sch.name not in (''fmaudit'', ''sync'', ''track'')
		ORDER BY sch.name,tab.name,col.name

PRINT ''*** Processing table metadata...''

DECLARE @TableCount int
DECLARE @CurrentCount int
SELECT @TableCount = COUNT(*) FROM sync.tblSyncTableToScopeMap stts 
								INNER JOIN sync.tblSyncTable st 
									ON stts.SyncTableGuid = st.SyncTableGuid
SET @CurrentCount = 1

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
			,CASE WHEN pk.PKColumnName IS NULL THEN 0 ELSE 1 END AS ''IsPrimaryKey''
			,specialkey.ParentForeignKeyColumnName AS ''ForeignKey''
	FROM ConsolidatedDB.INFORMATION_SCHEMA.COLUMNS s
		INNER JOIN (SELECT PARSENAME(st.TableName, 2) ''SchemaName'', PARSENAME(st.TableName, 1) ''TableName''
						FROM sync.tblSyncTableToScopeMap stts 
							INNER JOIN sync.tblSyncTable st 
								ON stts.SyncTableGuid = st.SyncTableGuid) tables
			ON tables.[SchemaName] = s.TABLE_SCHEMA AND tables.[TableName] = s.TABLE_NAME
		LEFT OUTER JOIN (SELECT SchemaName, TableName, PKColumnName FROM @PrimaryKeyColumnList) pk
			ON s.TABLE_SCHEMA = pk.[SchemaName] AND s.TABLE_NAME = pk.[TableName] AND s.COLUMN_NAME = pk.[PKColumnName]
		LEFT JOIN (select parsename(tablename, 2) ''schemaname'', parsename(tablename,1) ''tablename'', ParentForeignKeyColumnName from sync.tblSyncTable where ParentSyncTableGuid IS NOT NULL AND ParentForeignKeyColumnName IS NOT NULL) as specialkey
			ON s.TABLE_SCHEMA = specialkey.schemaname and s.TABLE_NAME = specialkey.tablename and s.COLUMN_NAME = specialkey.ParentForeignKeyColumnName
	ORDER BY s.TABLE_SCHEMA
			,s.TABLE_NAME
			,s.ORDINAL_POSITION

OPEN TableInfoCursor
FETCH NEXT FROM TableInfoCursor INTO 
	@Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix,@IsPKColumn,@specialForeignKey

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @StepStartTime = GETDATE()

	IF (@LastTable IS NULL)
	BEGIN
		SET @CurrentCount = 1;
		SET @LastSchema = @Schema
		SET @LastTable = @Table;
		SET @hasSiteGuid = 0;
		SET @hasCreatedDate = 0;
		SET @hasUpdatedDate = 0;

		SET @SqlPKColumns = N'''';
		SET @SqlPKData = N'''';

		PRINT ''>>> Step started at ''+CAST(@StepStartTime AS nvarchar(50)) + '' <<<''
		PRINT ''>>> Table '' + CAST(@CurrentCount AS nvarchar(12)) + '' of '' + CAST(@TableCount AS nvarchar(12)) + '' <<<''
	END
	ELSE IF (@LastTable <> @Table)
	BEGIN
		SET @Sql = ''TRUNCATE TABLE [track].[''+ @LastTable + ''];''
		SET @Sql+= ''INSERT INTO [track].[''+ @LastTable + ''] WITH (TABLOCK) (''
		SET @Sql+= ''InsertedDate,InsertedContext,InsertedRowVersion,UpdatedDate,UpdatedContext,UpdatedRowVersion,DeletedDate,DeletedContext,DeletedRowVersion,CurrentSiteGuid,PreviousSiteGuid''
		SET @Sql+= @SqlPKColumns
		SET @Sql+= '') SELECT ''

		IF (@hasCreatedDate = 1)
		BEGIN
			SET @Sql+= ''CASE WHEN CreatedDate IS NOT NULL THEN CreatedDate ELSE CAST(''''1/1/1990''''AS DateTimeOffset(7)) END ''''InsertedDate''''''
		END
		ELSE
		BEGIN
			SET @Sql+= ''CAST(''''1/1/1990''''AS DateTimeOffset(7)) ''''InsertedDate''''''
		END
		SET @Sql+= '',NULL ''''InsertedContext''''''
		SET @Sql+= '',_RowVersion ''''InsertedRowVersion''''''
		IF (@hasUpdatedDate = 1)
		BEGIN
			SET @Sql+= '',CASE WHEN UpdatedDate IS NOT NULL THEN UpdatedDate ELSE CAST(''''1/1/1990''''AS DateTimeOffset(7)) END ''''UpdatedDate''''''
		END
		ELSE
		BEGIN
			IF (@hasCreatedDate = 1)
			BEGIN
				SET @Sql+= '',CASE WHEN CreatedDate IS NOT NULL THEN CreatedDate ELSE CAST(''''1/1/1990''''AS DateTimeOffset(7)) END ''''UpdatedDate''''''
			END
			ELSE
			BEGIN
				SET @Sql+= '',CAST(''''1/1/1990''''AS DateTimeOffset(7)) ''''UpdatedDate''''''
			END
		END
		SET @Sql+= '',NULL ''''UpdatedContext''''''
		SET @Sql+= '',_RowVersion ''''UpdatedRowVersion''''''
		SET @Sql+= '',NULL ''''DeletedDate''''''
		SET @Sql+= '',NULL ''''DeletedContext''''''
		SET @Sql+= '',NULL ''''DeleteRowVersion''''''
		IF (@hasSiteGuid = 1)
		BEGIN
			SET @Sql+= '',SiteGuid ''''CurrentSiteGuid''''''
		END
		ELSE
		BEGIN
			SET @Sql+= '',NULL ''''CurrentSiteGuid''''''
		END
		SET @Sql+= '',NULL ''''PreviousSiteGuid''''''

		SET @Sql+= @SqlPKData
		SET @Sql+= '' FROM ['' + @LastSchema +''].['' + @LastTable + ''] ''

		EXEC sp_executesql @statment=@Sql

		print @sql

		SET @hasSiteGuid = 0;
		SET @hasCreatedDate = 0;
		SET @hasUpdatedDate = 0;
		SET @SqlPKColumns = N'''';
		SET @SqlPKData = N'''';

		SET @StepEndTime=GETDATE()
		PRINT CHAR(13) + CHAR(10)
		PRINT ''>>> Step finished on ''+CAST(@StepEndTime AS nvarchar(50)) + '' <<<''
		PRINT ''>>> Affected Records: ''+CAST (@@ROWCOUNT AS nvarchar(50)) + '' <<<''
		PRINT ''>>> Step elapse time in seconds: ''+ CAST(DATEDIFF(ss,@StepStartTime,@StepEndTime) AS nvarchar(50)) + '' <<<''
		PRINT ''>>> Step elapse time in minutes: ''+ CAST(DATEDIFF(mi,@StepStartTime,@StepEndTime) AS nvarchar(50)) + '' <<<''
		SET @AffectedRecords += @@ROWCOUNT

		SET @currentCount=@currentCount+1

		PRINT ''''
		PRINT ''>>> Step started at ''+CAST(@StepStartTime AS nvarchar(50)) + '' <<<''
		PRINT ''>>> Table '' + CAST(@CurrentCount AS nvarchar(12)) + '' of '' + CAST(@TableCount AS nvarchar(12)) + '' <<<''
		
		SET @LastSchema = @Schema
		SET @LastTable = @Table
	END

	IF (@Column = ''SiteGuid'') SET @hasSiteGuid = 1;
	IF (@Column = ''CreatedDate'') SET @hasCreatedDate = 1;
	IF (@Column = ''UpdatedDate'') SET @hasUpdatedDate = 1;

	IF (@IsPKColumn = 1)
	BEGIN
		SET @SqlPKColumns+= '',PK_'' + @Column;
		SET	@SqlPKData+= '','' + @Column + '' ''''PK_''+ @Column + '''''''';
	END

	IF (@specialForeignKey IS NOT NULL)
	BEGIN
		SET @SqlPKColumns+= '', FK_ParentPK''
		SET @SqlPKData += '',''+@specialForeignKey
	END

	FETCH NEXT FROM TableInfoCursor INTO
		@Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix,@IsPKColumn,@specialForeignKey
END

PRINT ''*** Process finished on ''+CAST(@StepEndTime AS nvarchar(50))
PRINT ''*** Total Number of Affected Records: '' + CAST(@AffectedRecords AS nvarchar(50))
PRINT ''*** Process elapse time in minutes: ''+ CAST(DATEDIFF(mi,@ProcessStartTime,@StepEndTime) AS nvarchar(50))
PRINT CHAR(13) + CHAR(10)
PRINT ''Process Complete.''
CLOSE TableInfoCursor
DEALLOCATE TableInfoCursor

PRINT CHAR(13) + CHAR(10)
PRINT ''Default Synchronization State Initialized.''

PRINT ''Completed successfully''


GO', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00095 Create Unique Indexes]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00095 Create Unique Indexes', 
		@step_id=45, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'-- Vivian added this unique indexes to resolve issues where candidate key or primary key were missing in step 41

CREATE UNIQUE INDEX IX_CLUSTERIDX
 ON [dbo].[tblOpcUaSubscription] ([_ClusterIdx])
GO  

 
 CREATE UNIQUE INDEX IX_tblEntityTransactionAliasToSite_TransactionAliasGuid_SiteGuid
 ON  [map].[tblEntityTransactionAliasToSite]([TransactionAliasGuid],[SiteGuid])

  CREATE UNIQUE INDEX IX_tblEntityIATACodeToSite_IATAGuid_SiteGuid
ON [map].[tblEntityIATACodeToSite]([IATAGuid],[SiteGuid]);

 CREATE UNIQUE INDEX IX_tblSRMAdaptorFilterType_SRMAdaptorFilterTypeCode
ON [lookup].[tblSRMAdaptorFilterType]([SRMAdaptorFilterTypeCode]);


-- sp_help ''erv.tblTempVersionSpecificField''
DECLARE @Schema NVARCHAR(100)
	,	@Table NVARCHAR(500)
	,	@Column NVARCHAR(500)
	,	@Sql NVARCHAR(max)
	,	@Count INT
	,	@DataType NVARCHAR(500)
	,	@MaxLen INT
	,	@IsNullable nvarchar(5)
--
-- ADJUST NON-NULLABLE COLUMNS
--



UPDATE dbo.tblSites SET UseTankReconciliation = 0 where UseTankReconciliation IS NULL
UPDATE dbo.tblSites SET MeterReconciliationToleranceIsPercent = 0 where MeterReconciliationToleranceIsPercent IS NULL
UPDATE dbo.tblSites SET AllowUseOfSpecialChars = 1 where AllowUseOfSpecialChars IS NULL
UPDATE dbo.tblSites SET EnablePeriodicSyncFlag = 0 where EnablePeriodicSyncFlag IS NULL
UPDATE dbo.tblSites SET PeriodicSyncIntervalMinutes = 0 where PeriodicSyncIntervalMinutes IS NULL
UPDATE dbo.tblTransactionAliases SET IncludeInDispatch = 0 where IncludeInDispatch IS NULL
UPDATE dbo.tblTransactionAliasFields SET ClearOnNew = 0 where ClearOnNew IS NULL


UPDATE dbo.tblArchivedUsers
SET SiteGuid=''00000000-0000-0000-0000-000000000001''
WHERE SiteGuid IS NULL


UPDATE dbo.tblChangesQueue
SET SiteGuid=''00000000-0000-0000-0000-000000000001''
WHERE SiteGuid IS NULL


UPDATE [dbo].[tblArchivedUsers]
SET [UserGuid]=NEWID()
WHERE [UserGuid] IS NULL

UPDATE [dbo].[tblAuditLog]
SET [AuditedDate]=[CreatedDate]
WHERE [AuditedDate] IS NULL

--UPDATE [dbo].[tblBSMEReconciliationData]
--SET [CreatedBy] = ''System''
--WHERE [CreatedBy] IS NULL
--
--UPDATE [dbo].[tblBSMEReconciliationData]
--SET [CreatedDate] = sysdatetimeoffset()
--WHERE [CreatedDate] IS NULL
--
--
--UPDATE [dbo].[tblBSMEReconciliationHistory]
--SET [CreatedBy] = ''System''
--WHERE [CreatedBy] IS NULL
--
--UPDATE [dbo].[tblBSMEReconciliationHistory]
--SET [CreatedDate] = sysdatetimeoffset()
--WHERE [CreatedDate] IS NULL
--
--
--UPDATE [dbo].[tblBSMEReconciliationHistory]
--SET [UpdatedBy] = ''System''
--WHERE [UpdatedBy] IS NULL
--
--UPDATE [dbo].[tblBSMEReconciliationHistory]
--SET [UpdatedDate] = sysdatetimeoffset()
--WHERE [UpdatedDate] IS NULL

-- Vivian Added DROP AND RECREATE _CLUSTERIDX AS IDENTITY

	SELECT	distinct tmp.TABLE_SCHEMA as SchemaName
		,	tmp.TABLE_NAME as TableName
		,	tmp.column_NAME as ColumnName
		,	tmp.DATA_TYPE as DataType
		,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
		,	tmp.IS_NULLABLE AS IsNullable
	into #Clusteredidx
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
	where tmp.column_NAME like  ''%_ClusterIdx''


	--select * from #Clusteredidx


DECLARE ColumnCursor CURSOR FOR
	SELECT	tmp.TABLE_SCHEMA as SchemaName
		,	tmp.TABLE_NAME as TableName
		,	tmp.column_NAME as ColumnName
		,	tmp.DATA_TYPE as DataType
		,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
		,	tmp.IS_NULLABLE AS IsNullable
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
	INNER JOIN INFORMATION_SCHEMA.COLUMNS upg 
		ON(		upg.TABLE_SCHEMA=tmp.TABLE_SCHEMA
			AND	upg.TABLE_NAME=tmp.TABLE_NAME
			AND	upg.COLUMN_NAME=tmp.COLUMN_NAME)
	WHERE tmp.IS_NULLABLE<>upg.IS_NULLABLE
	/** Vivian added the filter out for _clusteridx because you cannnot alter the column with existing nullable data to an identity field***/
	/** Note - This does not resolve the ommitted fields == look at later ***/
AND tmp.column_NAME LIKE ''%_ClusterIdx''
	--,''ID'',''LookupPresetTypeIndex'',''AutomaticCloseout'',''SystemQuery'',''Enterprise'',''GlobalAccessToEquipment'',''GlobalAccessToPersonnel'',''OperateTabGroups'',''LookupStationInterfaceTypeIndex'',''LookupStationTypeIndex'',''PromptForGravityCaptured'',''PromptForTemperatureCaptured'')
	and ISNULL(tmp.CHARACTER_MAXIMUM_LENGTH,0)<>-1
	and NOT (tmp.TABLE_NAME = ''tblAlarmAndEventLog'' and tmp.COLUMN_NAME = ''AssociatedData'')
	AND NOT (tmp.table_schema = ''dbo'' and tmp.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY tmp.TABLE_SCHEMA,tmp.TABLE_NAME,tmp.column_NAME
	OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
WHILE @@FETCH_STATUS=0
BEGIN



SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  COLUMN_NAME 	LIKE ''%_ClusterIdx''

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  COLUMN_NAME =@Column
	--LIKE ''%_ClusterIdx''
    AND TABLE_NAME = @Table
    AND TABLE_SCHEMA = @Schema )
	BEGIN
	
		SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
			PRINT @Sql
		EXEC sp_executesql @Statement=@Sql
		--	SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
END	
		--PRINT @Sql
		--EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor


DECLARE ColumnCursor CURSOR FOR
SELECT	tmp.SchemaName
		,	tmp.TableName
		,	tmp.ColumnName
		,	tmp.DataType
		,	tmp.MaxLen
		,	tmp.IsNullable

FROM    #Clusteredidx tmp
	ORDER BY tmp.SchemaName,tmp.TABLENAME,tmp.columnNAME
	OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
WHILE @@FETCH_STATUS=0
BEGIN
IF @Column LIKE ''%_ClusterIdx''
		--SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
		--	PRINT @Sql
		--EXEC sp_executesql @Statement=@Sql
			SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
		
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql
	FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor





DECLARE ColumnCursor CURSOR FOR
	SELECT	tmp.TABLE_SCHEMA as SchemaName
		,	tmp.TABLE_NAME as TableName
		,	tmp.column_NAME as ColumnName
		,	tmp.DATA_TYPE as DataType
		,	tmp.CHARACTER_MAXIMUM_LENGTH AS MaxLen
		,	tmp.IS_NULLABLE AS IsNullable
	FROM FuelsManagerDB_Template.INFORMATION_SCHEMA.COLUMNS tmp
	INNER JOIN INFORMATION_SCHEMA.COLUMNS upg 
		ON(		upg.TABLE_SCHEMA=tmp.TABLE_SCHEMA
			AND	upg.TABLE_NAME=tmp.TABLE_NAME
			AND	upg.COLUMN_NAME=tmp.COLUMN_NAME)
	WHERE tmp.IS_NULLABLE<>upg.IS_NULLABLE
	/** Vivian added the filter out for _clusteridx because you cannnot alter the column with existing nullable data to an identity field***/
	/** Note - This does not resolve the ommitted fields == look at later ***/
	AND tmp.column_NAME NOT IN (''ListViewID'',''ID'',''LookupPresetTypeIndex'',''AutomaticCloseout'',''SystemQuery'',''Enterprise'',''GlobalAccessToEquipment'',''GlobalAccessToPersonnel'',''OperateTabGroups'',''LookupStationInterfaceTypeIndex'',''LookupStationTypeIndex'',''PromptForGravityCaptured'',''PromptForTemperatureCaptured'')
	and ISNULL(tmp.CHARACTER_MAXIMUM_LENGTH,0)<>-1
	and NOT (tmp.TABLE_NAME = ''tblAlarmAndEventLog'' and tmp.COLUMN_NAME = ''AssociatedData'')
	AND NOT (tmp.table_schema = ''dbo'' and tmp.TABLE_NAME in (''tblExportResults'', ''tblExportResultDetails'',
							''tblTransactionLineItems'',
							''tblTransactions'',
							''tblTransactionUserData'',
							''tblTransactionLineItemUserData'',
							''tblTransactionNotes'',
							''tblTransactionSignature'',
							''tblTransactionTransportLineItems'',
							''tblTransactionWeightReadings'',
							''tblTransactionLinks'',
							''tblTransactionPIDX'',
							''tblTransactionSubLineItems''))
	ORDER BY tmp.TABLE_SCHEMA,tmp.TABLE_NAME,tmp.column_NAME

OPEN ColumnCursor
FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
WHILE @@FETCH_STATUS=0
BEGIN
	IF (@Column=''CreatedBy'') OR (@Column=''UpdatedBy'')
	BEGIN
		SET @DataType=''UdtUserID'';
		SET @MaxLen=NULL;
	END

	IF @Column= ''SiteGuid''
	BEGIN
		SET @Sql=''UPDATE  [''+@Schema+''].[''+@Table+''] SET [''+@Column+''] = ''''00000000-0000-0000-0000-000000000001'''' WHERE [''+@Column+''] IS NULL''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

	END 

	--/***  VIVIAN ADDED THIS PORTION FOR CLUSTERIDX COLUMNS ****/
	--	IF @Column LIKE ''%_ClusterIdx''
	--BEGIN
		
	--	SET @SQL =  '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
	--		PRINT @Sql
	--	EXEC sp_executesql @Statement=@Sql
	--		SET @SQL = @SQL + ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''] INT IDENTITY ''
	
		
	--	PRINT @Sql
	--	EXEC sp_executesql @Statement=@Sql
			

	--END





	IF @Column = ''PinNumber'' and @Table = ''tblPersonnel''
	BEGIN
		
		SET @SQL = ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ADD [''+@Column+''_Temp] ''+@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''')
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql
		
		SET @SQL = '' UPDATE [''+@Schema+''].[''+@Table+''] SET [''+@Column+''_Temp] = CONVERT('' +@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''') + '', ['' + @Column + '']); ''
		SET @SQL = @SQL + '' ALTER TABLE [''+@Schema+''].[''+@Table+''] DROP COLUMN [''+@Column +'']; ''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

		SET @SQL = '' EXEC sp_rename '''''' + @Table + ''.'' + @Column+''_Temp'''', '''''' + @Column + '''''', ''''COLUMN''''; ''
		PRINT @Sql
		EXEC sp_executesql @Statement=@Sql

	END

	SET @Sql = ''ALTER TABLE [''+@Schema+''].[''+@Table+''] ALTER COLUMN [''+@Column+''] ''+@DataType+ISNULL(''(''+CAST(@MaxLen AS VARCHAR(100))+'')'','''') + iif(@IsNullable = ''YES'', '' NULL '', '' NOT NULL '')
	
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM ColumnCursor INTO @Schema,@Table,@Column,@DataType,@MaxLen, @IsNullable
END
CLOSE ColumnCursor
DEALLOCATE ColumnCursor

-- EXCEPTION: THE ABOVE SCRIPT FAILED TO CHANGE THE BELOW COLUMN BECAUSE IT DOES NOT HANDLE "MAX" ON NVARCHAR COLUMN
PRINT ''ALTER TABLE [dbo].[tblAlarmAndEventLog] ALTER COLUMN [AssociatedData] nvarchar(MAX) NOT NULL''

ALTER TABLE [dbo].[tblAlarmAndEventLog] ALTER COLUMN [AssociatedData] nvarchar(MAX) NOT NULL 
GO
PRINT ''Completed successfully''


GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00100 Create Foreign Keys]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00100 Create Foreign Keys', 
		@step_id=46, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET NOCOUNT ON;

/*
	TABLE CLEANUP
*/

UPDATE [dbo].[tblProducts]
SET FlowUnitIndex=NULL
WHERE FlowUnitIndex=0 -- There is no inumeration for 0


UPDATE [dbo].[tblProducts]
SET LevelUnitIndex=NULL
WHERE LevelUnitIndex=0 -- There is no inumeration for 0

UPDATE [dbo].[tblProducts]
SET PressureUnitIndex=NULL
WHERE PressureUnitIndex=0 -- There is no inumeration for 0

DELETE FROM [map].tblGroupToRight WHERE lookupRightIndex = 6


/*
	PROCESS TO ADD FK CONSTRAINTS
*/
DECLARE @FkName NVARCHAR(500)
	,	@OwnerSchema NVARCHAR(100)
	,	@OwnerTable NVARCHAR(300)
	,	@RefSchema NVARCHAR(100)
	,	@RefTable NVARCHAR(300)
	,	@FkId INT
	,	@OwnerFkCols NVARCHAR(max)
	,	@RefFkCols NVARCHAR(max)
	,	@Sql NVARCHAR(max)
	,	@ColCounter INT
	,	@TotalCols INT
	,	@OwnerColName NVARCHAR(500)
	,	@RefColName NVARCHAR(500)
	,	@OwnerColList NVARCHAR(max)
	,	@RefColList NVARCHAR(max)

CREATE TABLE #FKColums(RowNumber INT IDENTITY PRIMARY KEY, OwnerColName NVARCHAR(500), RefColName NVARCHAR(500))
	
DECLARE CtCursor CURSOR FOR
	SELECT	fkname.object_id as FkId
		,	fkname.name as FkName
		,	scowner.name as OwnerSchema
		,	tbowner.name as OwnerTable
		,	scref.name as RefSchema
		,	tbref.name as RefTable
		--,	fkcolowner.name as ColumnOwner
		--,	fkcolref.name as ColumnRef
	FROM FuelsManagerDB_Template.sys.foreign_keys fkname
	INNER JOIN FuelsManagerDB_Template.sys.tables tbowner on tbowner.object_id=fkname.parent_object_id
	INNER JOIN FuelsManagerDB_Template.sys.tables tbref on tbref.object_id=fkname.referenced_object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas scowner on scowner.schema_id=tbowner.schema_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas scref on scref.schema_id=tbref.schema_id
	WHERE NOT EXISTS( SELECT 1 FROM ConsolidatedDB.sys.foreign_keys fm WHERE fm.name=fkname.name) -- FOR NEW CONSTRAINTS ONLY
	/** Vivian added filter to omit the following constraints where there is no primary key for referenced table ***/
	
	--and fkname.name	NOT in (''FK_tblOpcUaMonitoredItem_SubscriptionId'',''FK_tblSitesAncillaryData_AdjustmentTransactionAliasGuid'',''FK_tblSRMAdaptorFilter_SRMAdaptorFilterTypeCode'',''FK_tblSitesAncillaryData_IATAIndexGuid'',''FK_tblSitesAncillaryData_InventoryTransactionAliasGuid'')
	ORDER BY scowner.name,tbowner.name,fkname.name

OPEN CtCursor
FETCH NEXT FROM CtCursor INTO @FkId,@FkName,@OwnerSchema,@OwnerTable,@RefSchema,@RefTable
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql=NULL
	SET @OwnerColList=NULL
	SET @RefColList=NULL
	SET @TotalCols=0
	SET @ColCounter=1
	TRUNCATE TABLE #FKColums
	INSERT INTO #FKColums(OwnerColName,RefColName)
	-- GET COLUMNS FOR FOREIGN KEY
	SELECT	OwnerCol.name as OwnerColumn
		,	RefCol.name as Refcolumn
	FROM FuelsManagerDB_Template.sys.foreign_key_columns FkCons
	INNER JOIN FuelsManagerDB_Template.sys.columns OwnerCol ON (FkCons.parent_object_id=OwnerCol.object_id and FkCons.parent_column_id=OwnerCol.Column_id)
	INNER JOIN FuelsManagerDB_Template.sys.columns RefCol ON (FkCons.referenced_object_id=RefCol.object_id and FkCons.referenced_column_id=RefCol.column_id)
	WHERE constraint_object_id=@FkId
	SET @TotalCols=@@ROWCOUNT
	
	WHILE @ColCounter<= @TotalCols
	BEGIN
		SELECT	@OwnerColName=OwnerColName
			,	@RefColName=RefColName
		FROM #FKColums
		WHERE RowNumber=@ColCounter
	
		IF @OwnerColList IS NULL -- FIRST COLUMN TO BE ADDRESSED
			SET @OwnerColList = ''[''+@OwnerColName+'']''
		ELSE
			SET @OwnerColList += '',[''+@OwnerColName+'']''
			

		IF @RefColList IS NULL -- FIRST COLUMN TO BE ADDRESSED
			SET @RefColList = ''[''+@RefColName+'']''
		ELSE
			SET @RefColList += '',[''+@RefColName+'']''

	
		SET @ColCounter+= 1;
	END
	
	
	SET @Sql=''ALTER TABLE [''+@OwnerSchema+''].[''+@OwnerTable+''] ADD CONSTRAINT [''+@FkName+''] FOREIGN KEY(''+@OwnerColList+'') REFERENCES [''+@RefSchema+''].[''+@RefTable+''](''+@RefColList+'');''
	
	PRINT @Sql
	EXEC sp_executesql @statement=@sql

	FETCH NEXT FROM CtCursor INTO @FkId,@FkName,@OwnerSchema,@OwnerTable,@RefSchema,@RefTable
END
CLOSE CtCursor
DEALLOCATE CtCursor

DROP TABLE #FKColums

PRINT ''Completed successfully''

GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00120 Reinstate Indexes]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00120 Reinstate Indexes', 
		@step_id=47, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'set noexec off
SET NOCOUNT ON;


DECLARE @IndexName AS NVARCHAR(500)
	,	@ObjectId AS INTEGER
	,	@Schema AS NVARCHAR(300)
	,	@Table AS NVARCHAR(500)
	,	@IndexId INTEGER
	,	@Sql NVARCHAR(MAX)
	,	@IndexType NVARCHAR(100)
	,	@ColCounter INT
	,	@TotalCols INT
	,	@ColList NVARCHAR(max)
	,	@ColName NVARCHAR(500)

--IF EXISTS(SELECT 1 FROM tempdb.sys.tables WHERE name LIKE ''%#Columns%'')
--	DROP TABLE #Columns

--CREATE TABLE #Columns(RowNumber INT IDENTITY,ColName NVARCHAR(200))

DECLARE IndexCursor  CURSOR STATIC FOR 
	SELECT	distinct idx.object_id as ObjectId
		,	idx.Index_Id as IndexId
		,	idx.Type_Desc as TypeDesc
		,	idx.name as IndexName
		,	sch.name as SchemaName
		,	obj.name as TableName
	FROM	FuelsManagerDB_Template.sys.indexes idx
	INNER JOIN FuelsManagerDB_Template.sys.objects obj ON idx.object_id=obj.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sch ON obj.schema_id=sch.schema_id
	WHERE obj.type=''U''
	AND idx.[type] <> 0
	AND idx.[type] IN (1,2)
	--AND OBJ.NAME =''tblSRMDuplicateMessageInformation''

AND idx.name NOT IN (''IXU_tblTankGroups_ID_SiteGuid'',''IXU_tblTanks_TankID_SiteGuid'',''IXU_tblTransactionAliases_SiteGuid_MasterRecordGuid'',''IXU_tblTransactionAliases_TransactionAliasGuid_AliasName'',''IXU_tblTransactionLineItems_CoveringAssociatedTransactionQueries'',''IXU_tblTransactions_CoveringAssociatedTransactionQueries'',''IXU_tblPointAccessGroupToPointTemplate_PointTemplateGuid_PointAccessGroupGuid'',''IXU_lookup_tblUserDataType_UserDataTypeGuid'',''IXU_tblTransactions_CoveringPreviousVersionInformation'',''IXU_tblTransactions_ReversedTransID'',''IXU_tblPointAccessGroupToTag_TagGuid_PointAccessGroupGuid'',''IXU_lookup_tblVesselType_VesselTypeGuid'',''IXU_tblTransactions_TransID'',''IXU_lookup_tblWatchdogMode_WatchdogModeGuid'' ,''UIX_tblCompanyCrossReferenceMap_MapKeyName_MapValueName'' ,''IXU_tblTrend_ID_SiteGuid'',''IX_tblTransactionSubLineItems_TransactionInventoryDate'' ,''IXU_tblTrend_ID_SiteGuid_PointTemplateGuid'',''IX_tblModule_ClusterIdx'' ,''IXU_tblPointAccessGroupToUserGroup_UserGroupGuid_PointAccessGroupGuid'' ,''UIX_lookup_tblSRMAdaptorFilterType_SRMAdaptorFilterTypeCode'',''UIX_tblExportRequest_RequestID'',''IXU_tblUsers_UserGuid'',''IX_tblUserDataFieldUser_CreatedDate'',''IX_tblTransactionSubLineItems_TransactionLineItemGuid_SequenceID'',''IX_map_tblEntityMaintenanceReasonToSite_AuditRowVersion_EventType_EventSequence'' ,''UIX_tblFuelCardLimit_ID_SiteGuid'',''IXU_tblUsers_UserID_SiteGuid'',''IXU_tblIATA_IATAID_SiteGuid'',''IX_tblTransactionTransportLineItems_ClusterIdx'',''IX_tblMobileDeviceProfilePrinter_ClusterIdx'',''NCIX_tblCompanyCompanyToUserGroup_GroupGuid_CreatedDate_CompanyGuid'',''IX_tblModuleToPointTemplate_ClusterIdx'',''IX_tblEntityPointTemplateTypeToSite_ClusterIdx'', ''IX_tblEntityPointTemplateToSite_ClusterIdx'',''IX_tblEntityPointCategoryToSite_ClusterIdx'',''UIX_tblFuelCardLimitPeriod_FuelCardLimitPeriodGuid'',''IX_tblModule_AuditRowVersion_EventType_EventSequence'',''UIX_tblSRMDuplicateMessageInformation_FlightKeyFields'',''IXU_tblWebLink_LinkName'',''IXU_tblListViews_ID_SiteGuid'',''IX_tblMobileDeviceProfilePrinter_CreatedDate'',''IX_tblTransactionTransportLineItems_CreatedDate'' ,''IX_tblModule_ClusterIdx'',''IX_tblTransactionTransportLineItems_TransactionGuid_TransportOrderNumber'') -- NEED TO VERIFY IF WE WANT TO DROP PRE-EXISTING INDEXES
AND NOT EXISTS
(	SELECT t.NAME, S.NAME,I.NAME
FROM SYS.OBJECTS O
INNER JOIN  sys.tables  T
ON O.object_id=T.object_id
INNER JOIN sys.schemas S
ON T.schema_id=S.schema_id
INNER JOIN  sys.indexes I    
ON T.Object_id = I.Object_id  
where i.name=idx.name
and s.name= sch.name
and t.name = obj.name

	)

AND IDX.NAME NOT LIKE ''IX_tblModule_ClusterIdx''  -- THIS TABLE ALREADY HAS A DIFFERENT CLUSTERED INDEX
	--AND idx.name <> ''IXU_tblListViews_ID_SiteGuid'' --todo: remove this line
	--AND sch.name <> ''SQLDependency''
	ORDER BY sch.name, idx.name


OPEN IndexCursor
FETCH NEXT FROM IndexCursor INTO @ObjectId,@IndexId,@IndexType,@IndexName,@Schema,@Table
WHILE @@FETCH_STATUS=0
BEGIN
--print @IndexName
IF NOT EXISTS 
(SELECT t.NAME, S.NAME,I.NAME 
FROM SYS.OBJECTS O
INNER JOIN  sys.tables  T
ON O.object_id=T.object_id
INNER JOIN sys.schemas S
ON T.schema_id=S.schema_id
INNER JOIN  sys.indexes I    
ON T.Object_id = I.Object_id  
WHERE I.name= @IndexName AND S.NAME =@Schema AND T.NAME=@Table and i.name not like ''IX_tblModule_ClusterIdx'')
BEGIN
	SELECT DISTINCT  @Sql = ''IF NOT EXISTS (SELECT 1 FROM sysindexes WHERE id=object_id('''''' + S.name + ''.'' + T.name +'''''') AND name='''''' + I.name + '''''') CREATE '' +  
	
    CASE WHEN I.is_unique = 1 THEN '' UNIQUE '' ELSE '''' END  +   
    I.type_desc COLLATE DATABASE_DEFAULT +'' INDEX ['' +    
    I.name  + ''] ON [''  + S.name + ''].[''+T.name + ''] ( '' +  
    KeyColumns + '' )  '' +  
    ISNULL('' INCLUDE (''+IncludedColumns+'' ) '','''') +  
    ISNULL('' WHERE  ''+I.Filter_definition,'''') + 
	/*'' WITH ( '' +  
    CASE WHEN I.is_padded = 1 THEN '' PAD_INDEX = ON '' ELSE '' PAD_INDEX = OFF '' END + '',''  +  
    ''FILLFACTOR = ''+CONVERT(CHAR(5),CASE WHEN I.Fill_factor = 0 THEN 100 ELSE I.Fill_factor END) + '',''  +  
    -- default value  
    ''SORT_IN_TEMPDB = OFF ''  + '',''  +  
    CASE WHEN I.ignore_dup_key = 1 THEN '' IGNORE_DUP_KEY = ON '' ELSE '' IGNORE_DUP_KEY = OFF '' END + '',''  +  
    CASE WHEN ST.no_recompute = 0 THEN '' STATISTICS_NORECOMPUTE = OFF '' ELSE '' STATISTICS_NORECOMPUTE = ON '' END + '',''  +  
    -- default value   
    '' DROP_EXISTING = ON ''  + '',''  +  
    -- default value   
    '' ONLINE = OFF ''  + '',''  +  
   CASE WHEN I.allow_row_locks = 1 THEN '' ALLOW_ROW_LOCKS = ON '' ELSE '' ALLOW_ROW_LOCKS = OFF '' END + '',''  +  
   CASE WHEN I.allow_page_locks = 1 THEN '' ALLOW_PAGE_LOCKS = ON '' ELSE '' ALLOW_PAGE_LOCKS = OFF '' END  + '' ) ON ['' +  
   DS.name + '' ] ''  [CreateIndexScript] 
   */
   '';'' 
FROM FuelsManagerDB_Template.sys.indexes I    
 JOIN FuelsManagerDB_Template.sys.tables T ON T.Object_id = I.Object_id  
 INNER JOIN FuelsManagerDB_Template.sys.schemas S ON T.schema_id=S.schema_id    
 JOIN FuelsManagerDB_Template.sys.sysindexes SI ON I.Object_id = SI.id AND I.index_id = SI.indid    
 JOIN (SELECT * FROM (   
    SELECT IC2.object_id , IC2.index_id ,   
        STUFF((SELECT '' , ['' + C.name + '']'' + CASE WHEN MAX(CONVERT(INT,IC1.is_descending_key)) = 1 THEN '' DESC '' ELSE '' ASC '' END 
    FROM FuelsManagerDB_Template.sys.index_columns IC1   
    JOIN FuelsManagerDB_Template.Sys.columns C    
       ON C.object_id = IC1.object_id    
       AND C.column_id = IC1.column_id    
       AND IC1.is_included_column = 0   
    WHERE IC1.object_id = IC2.object_id    
       AND IC1.index_id = IC2.index_id    
    GROUP BY IC1.object_id,C.name,index_id   
    ORDER BY MAX(IC1.key_ordinal)   
       FOR XML PATH('''')), 1, 2, '''') KeyColumns    
    FROM FuelsManagerDB_Template.sys.index_columns IC2    
    --WHERE IC2.Object_id = object_id(''Person.Address'') --Comment for all tables   
    GROUP BY IC2.object_id ,IC2.index_id) tmp3 )tmp4    
  ON I.object_id = tmp4.object_id AND I.Index_id = tmp4.index_id   
 JOIN FuelsManagerDB_Template.sys.stats ST ON ST.object_id = I.object_id AND ST.stats_id = I.index_id    
 JOIN FuelsManagerDB_Template.sys.data_spaces DS ON I.data_space_id=DS.data_space_id    
 JOIN FuelsManagerDB_Template.sys.filegroups FG ON I.data_space_id=FG.data_space_id    
 LEFT JOIN (SELECT * FROM (    
    SELECT IC2.object_id , IC2.index_id ,    
        STUFF((SELECT '' , ['' + C.name  +'']''
    FROM FuelsManagerDB_Template.sys.index_columns IC1    
    JOIN FuelsManagerDB_Template.sys.columns C     
       ON C.object_id = IC1.object_id     
       AND C.column_id = IC1.column_id     
       AND IC1.is_included_column = 1    
    WHERE IC1.object_id = IC2.object_id     
       AND IC1.index_id = IC2.index_id     
    GROUP BY IC1.object_id,C.name,index_id    
       FOR XML PATH('''')), 1, 2, '''') IncludedColumns     
   FROM FuelsManagerDB_Template.sys.index_columns IC2     
   --WHERE IC2.Object_id = object_id(''Person.Address'') --Comment for all tables    
   GROUP BY IC2.object_id ,IC2.index_id) tmp1    
   WHERE IncludedColumns IS NOT NULL ) tmp2     
ON tmp2.object_id = I.object_id AND tmp2.index_id = I.index_id    
WHERE I.is_primary_key = 0 AND I.is_unique_constraint = 0  
--AND I.Object_id = object_id(''Person.Address'') --Comment for all tables  
AND I.name = @IndexName --comment for all indexes  
AND S.NAME = @Schema
AND T.NAME =@Table
AND @IndexName NOT IN (''IXU_tblTankGroups_ID_SiteGuid'',''IXU_tblTanks_TankID_SiteGuid'',''IXU_tblTransactionAliases_SiteGuid_MasterRecordGuid'',''IXU_tblTransactionAliases_TransactionAliasGuid_AliasName'',''IXU_tblTransactionLineItems_CoveringAssociatedTransactionQueries'',''IXU_tblTransactions_CoveringAssociatedTransactionQueries'',''IXU_tblPointAccessGroupToPointTemplate_PointTemplateGuid_PointAccessGroupGuid'',''IXU_lookup_tblUserDataType_UserDataTypeGuid'',''IXU_tblTransactions_CoveringPreviousVersionInformation'',''IXU_tblTransactions_ReversedTransID'',''IXU_tblPointAccessGroupToTag_TagGuid_PointAccessGroupGuid'',''IXU_lookup_tblVesselType_VesselTypeGuid'',''IXU_lookup_tblWatchdogMode_WatchdogModeGuid'' ,''UIX_tblCompanyCrossReferenceMap_MapKeyName_MapValueName'' ,''IXU_tblTrend_ID_SiteGuid'',''IX_tblTransactionSubLineItems_TransactionInventoryDate'' ,''IXU_tblTrend_ID_SiteGuid_PointTemplateGuid'',''IX_tblModule_ClusterIdx'' ,''IX_tblUserDataFieldUser_CreatedDate'',''IX_tblTransactionSubLineItems_TransactionLineItemGuid_SequenceID'',''IX_map_tblEntityMaintenanceReasonToSite_AuditRowVersion_EventType_EventSequence'' ,''UIX_tblFuelCardLimit_ID_SiteGuid'',''IXU_tblUsers_UserID_SiteGuid'',''IXU_tblIATA_IATAID_SiteGuid'',''IX_tblTransactionTransportLineItems_ClusterIdx'',''IX_tblMobileDeviceProfilePrinter_ClusterIdx'',''NCIX_tblCompanyCompanyToUserGroup_GroupGuid_CreatedDate_CompanyGuid'',''IX_tblModuleToPointTemplate_ClusterIdx'',''IX_tblEntityPointTemplateTypeToSite_ClusterIdx'', ''IX_tblEntityPointTemplateToSite_ClusterIdx'',''IX_tblEntityPointCategoryToSite_ClusterIdx'',''UIX_tblFuelCardLimitPeriod_FuelCardLimitPeriodGuid'',''IX_tblModule_AuditRowVersion_EventType_EventSequence'',''UIX_tblSRMDuplicateMessageInformation_FlightKeyFields'',''IXU_tblWebLink_LinkName'',''IXU_tblListViews_ID_SiteGuid'',''IX_tblMobileDeviceProfilePrinter_CreatedDate'',''IX_tblTransactionTransportLineItems_CreatedDate'' ,''IX_tblModule_ClusterIdx'',''IX_tblTransactionTransportLineItems_TransactionGuid_TransportOrderNumber'') -- NEED TO VERIFY IF WE WANT TO DROP PRE-EXISTING INDEXES

	PRINT @Sql
	
EXEC sp_executesql @statement=@sql
END
	FETCH NEXT FROM IndexCursor INTO @ObjectId,@IndexId,@IndexType,@IndexName,@Schema,@Table
END
CLOSE IndexCursor
DEALLOCATE IndexCursor;
--DROP TABLE #
PRINT ''Completed successfully''


GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00140 Deploy User Defined Function]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00140 Deploy User Defined Function', 
		@step_id=48, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
---------------------------------
	ADD ADDITIONAL FUNCTIONS THAT WERE NOT DEPLOYED BUT EXISTS ON THE TEMPLATE DATABASE
---------------------------------
*/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DECLARE @Definition VARCHAR(max)
	,	@Name NVARCHAR(500)
	,	@Schema NVARCHAR(100)

DECLARE ObjCursor CURSOR  FOR
	SELECT	md.definition as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.objects pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.objects pr2 INNER JOIN sys.schemas sc2 on sc2.schema_id=pr2.schema_id where pr2.name=pr.name and sc.name=sc2.name)
	AND  pr.[type] IN (''FN'',''IF'',''TF'')
	-- DEPENDABLE FUNCTIONS: dbo.udf_AliasList DEPENDS ON erv.udf_GetTransactionAliasRecordVersions
	AND sc.name+''.''+pr.name NOT IN(''dbo.udf_AliasList'',''dbo.udf_CompanyList'',''dbo.udf_NoteListBySiteGuid'')  

	ORDER BY pr.[type],sc.name,pr.name

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)
	FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


DECLARE ObjCursor CURSOR  FOR
	SELECT	md.definition as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.objects pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.objects pr2 INNER JOIN sys.schemas sc2 on sc2.schema_id=pr2.schema_id where pr2.name=pr.name and sc.name=sc2.name)
	AND  pr.[type] IN (''FN'',''IF'',''TF'')
	-- DEPENDABLE FUNCTIONS: dbo.udf_AliasList DEPENDS ON erv.udf_GetTransactionAliasRecordVersions
	AND sc.name+''.''+pr.name IN(''dbo.udf_AliasList'',''dbo.udf_CompanyList'',''dbo.udf_NoteListBySiteGuid'')  

	ORDER BY pr.[type],sc.name,pr.name

OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)
	FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00145 Deploy Views]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00145 Deploy Views', 
		@step_id=49, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DECLARE @Definition VARCHAR(max)
	,	@Name NVARCHAR(500)
	,	@Schema NVARCHAR(200)

DECLARE ObjCursor CURSOR  FOR
	SELECT	md.definition as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.views pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.views pr2 where pr2.name=pr.name)
	AND pr.name <> ''vw_AutoDistributionRuleManagersProducts''
	--ORDER BY sc.name,CASE WHEN pr.name=''vw_AutoDistributionRuleProducts'' THEN ''Zvw_AutoDistributionRuleProducts'' ELSE pr.name END
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)
	FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
END
CLOSE ObjCursor
DEALLOCATE ObjCursor


/*

Msg 208, Level 16, State 1, Procedure vw_AutoDistributionRuleManagersProducts, Line 16
Invalid object name ''dbo.vw_AutoDistributionRuleProducts''.

*/

DECLARE ObjCursor CURSOR  FOR
	SELECT	md.definition as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.views pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.views pr2 where pr2.name=pr.name)
	AND pr.name = ''vw_AutoDistributionRuleManagersProducts''
	--ORDER BY sc.name,CASE WHEN pr.name=''vw_AutoDistributionRuleProducts'' THEN ''Zvw_AutoDistributionRuleProducts'' ELSE pr.name END
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)
	FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
END
CLOSE ObjCursor
DEALLOCATE ObjCursor
PRINT ''Completed successfully''


GO


', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00150 Deploy Stored Procedures]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00150 Deploy Stored Procedures', 
		@step_id=50, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[usp_CalculatePercentGainLoss]'') AND type in (N''P'', N''PC''))
DROP PROCEDURE [dbo].[usp_CalculatePercentGainLoss]
/****** Object:  StoredProcedure [dbo].[usp_CalculatePercentGainLoss]    Script Date: 8/8/2013 10:20:27 AM ******/
GO
CREATE PROCEDURE [dbo].[usp_CalculatePercentGainLoss]
@XMLstring NVARCHAR (4000)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Aliases xml
	SET @Aliases = @XMLstring
	
	DECLARE @RecCntrNumber01 FLOAT -- Shipment quantity
	DECLARE @RecTrfrNumber01 FLOAT -- Shipment quantity
	DECLARE @RecCntrNumber02 FLOAT -- Gain/loss quantity
	DECLARE @RecTrfrNumber02 FLOAT -- Gain/loss quantity
	DECLARE @TotalNumber01   FLOAT
	DECLARE @TotalNumber02   FLOAT
	DECLARE @PercentGainLoss FLOAT

	SELECT @RecCntrNumber01 = ParamValues.ID.value(''.'', ''FLOAT'')
	FROM @Aliases.nodes(''/Receive-Contract/n1'') AS ParamValues(ID)
	
	SELECT @RecCntrNumber02 = ParamValues.ID.value(''.'', ''FLOAT'')
	FROM @Aliases.nodes(''/Receive-Contract/n2'') AS ParamValues(ID)
	
	SELECT @RecTrfrNumber01 = ParamValues.ID.value(''.'', ''FLOAT'')
	FROM @Aliases.nodes(''/Receive-Transfer/n1'') AS ParamValues(ID)
	
	SELECT @RecTrfrNumber02 = ParamValues.ID.value(''.'', ''FLOAT'')
	FROM @Aliases.nodes(''/Receive-Transfer/n2'') AS ParamValues(ID)
	
	-- Total the Shipment quantity
	SELECT @TotalNumber01 = @RecCntrNumber01 + @RecTrfrNumber01
	
	-- Total the Gain/Loss quantity
	SELECT @TotalNumber02 = @RecCntrNumber02 + @RecTrfrNumber02
	
	SELECT @PercentGainLoss = 0.0
	
	-- Calculate the percentage gain/loss
	IF (@TotalNumber01 <> 0)
	BEGIN
		SELECT @PercentGainLoss = (@TotalNumber02 / @TotalNumber01) * 100.0
	END
	
	-- For custom functions the calculated value is always returned in field Number01.
	-- All the listed fields have to be returned, that is the convention.		
	SELECT CAST(0.0 AS FLOAT) AS Gross,
	       CAST(0.0 AS FLOAT) AS Net,
	       CAST(0.0 AS FLOAT) AS Mass,
	       CAST(0.0 AS FLOAT) AS GrossPrice,
	       CAST(0.0 AS FLOAT) AS NetPrice,
	       CAST(0.0 AS FLOAT) AS MassPrice,
	       CAST(round(@PercentGainLoss,2) AS FLOAT)   AS Number01,
	       CAST(0.0 AS FLOAT) AS Number02,
	       CAST(0.0 AS FLOAT) AS Number03,
	       CAST(0.0 AS FLOAT) AS Number04,
	       CAST(0.0 AS FLOAT) AS Number05,
	       CAST(0.0 AS FLOAT) AS Number06
END
GO

DECLARE @ErrorCount int=0
DECLARE @MSG NVARCHAR(MAX)

DECLARE @Definition VARCHAR(max)
	,	@Name NVARCHAR(500)
	,	@Schema NVARCHAR(100)

DECLARE ObjCursor CURSOR  FOR
	SELECT	md.definition as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.procedures pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.procedures pr2 where pr2.name=pr.name) AND pr.name <> ''rpt.usp_DsProductListx''
	

	ORDER BY sc.name,pr.name
OPEN ObjCursor
FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)

	FETCH NEXT FROM ObjCursor INTO @Definition,@Name,@Schema
END
CLOSE ObjCursor
DEALLOCATE ObjCursor

/****** Update old references to use new stored procedure names/naming convention ******/
UPDATE [dbo].[tblLedgerAggregateColumns] SET CustomFunctionName = ''usp_CalculatePercentGainLoss'' WHERE CustomFunctionName = ''fm_PercentGainLoss'';


PRINT ''Completed successfully''
GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00160 Deploy Check Constraints]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00160 Deploy Check Constraints', 
		@step_id=51, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'SET NOCOUNT ON;

DELETE FROM [erv].[tblEntityRecordVersioningFieldConfig] WHERE SiteGroupGuid NOT IN (SELECT SiteGuid FROM dbo.tblSites)

DECLARE @Constraint AS NVARCHAR(1000)
	,	@Schema AS NVARCHAR(300)
	,	@Table AS NVARCHAR(500)
	,	@Definition AS NVARCHAR(max)
	,	@Sql NVARCHAR(MAX)

DECLARE ConstCursor CURSOR FOR

	SELECT	ct.name as ConstraintName
		,	sc.name as SchemaName
		,	tb.name as TableName
		,	ct.[Definition]
		FROM FuelsManagerDB_Template.sys.check_constraints ct
		INNER JOIN FuelsManagerDB_Template.sys.objects tb on tb.object_id=ct.parent_object_id
		INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=tb.schema_id
		WHERE tb.type=''U''
		AND NOT EXISTS
		(	SELECT 1
			FROM ConsolidatedDB.sys.check_constraints ct2
			WHERE ct2.name=ct.name
		)
		AND ct.name NOT IN(''CK_tblEquipmentMaintenanceLog_InServiceFlag'',''CK_tblTankMaintenanceLog_InServiceFlag'',''CK_tblEntityExternalAttribute_RelationshipName'') -- CONFLICT ERRORS
		Order By sc.name,tb.name,ct.name



OPEN ConstCursor
FETCH NEXT FROM ConstCursor INTO @Constraint,@Schema,@Table,@Definition
WHILE @@FETCH_STATUS=0
BEGIN


	SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] WITH '' + CASE WHEN @Constraint = ''CK_tblTransactions_DocumentNumberUniqueness'' THEN ''NO'' ELSE '''' END + ''CHECK ADD CONSTRAINT [''+@Constraint+''] CHECK (''+@Definition+'');''
	
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql
		
	SET @Sql=''ALTER TABLE [''+@Schema+''].[''+@Table+''] CHECK CONSTRAINT [''+@Constraint+'']; ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	FETCH NEXT FROM ConstCursor INTO @Constraint,@Schema,@Table,@Definition
END
CLOSE ConstCursor
DEALLOCATE ConstCursor

-- DEPLOY CONSTRAINT CK_tblEntityExternalAttribute_RelationshipName
PRINT ''Creating constraint CK_tblEntityExternalAttribute_RelationshipName''


IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''[erv].[CK_tblEntityExternalAttribute_RelationshipName]'') AND parent_object_id = OBJECT_ID(N''[erv].[tblEntityExternalAttribute]''))
ALTER TABLE [erv].[tblEntityExternalAttribute] DROP CONSTRAINT [CK_tblEntityExternalAttribute_RelationshipName]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''[erv].[CK_tblEntityExternalAttribute_RelationshipName]'') AND parent_object_id = OBJECT_ID(N''[erv].[tblEntityExternalAttribute]''))
ALTER TABLE [erv].[tblEntityExternalAttribute]  WITH CHECK ADD  CONSTRAINT [CK_tblEntityExternalAttribute_RelationshipName] CHECK  (([erv].[udf_IsFieldNameUsed]([EntitySegmentTemplateGuid],[RelationshipName])=(0)))
GO

IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''[erv].[CK_tblEntityExternalAttribute_RelationshipName]'') AND parent_object_id = OBJECT_ID(N''[erv].[tblEntityExternalAttribute]''))
ALTER TABLE [erv].[tblEntityExternalAttribute] CHECK CONSTRAINT [CK_tblEntityExternalAttribute_RelationshipName]
GO

-- DEPLOY CONSTRAINTS CK_tblEquipmentMaintenanceLog_InServiceFlag & CK_tblTankMaintenanceLog_InServiceFlag
PRINT ''Creating constraints  CK_tblEquipmentMaintenanceLog_InServiceFlag and CK_tblTankMaintenanceLog_InServiceFlag''
-- Some of the original data fail constraint ([InServiceFlag]=1 AND [MaintenanceReasonGuid] IS NULL). This will correct that.
UPDATE [dbo].[tblEquipmentMaintenanceLog] SET [MaintenanceReasonGuid]=null WHERE [InServiceFlag]=1 AND [MaintenanceReasonGuid] IS NOT NULL

DECLARE @MaintReasonGuid NVARCHAR(50)
	,	@Sql NVARCHAR(max)

SELECT @MaintReasonGuid=MaintenanceReasonGuid
FROM dbo.tblMaintenanceReasons
WHERE [ID]=''Is Currently In Service''

IF @MaintReasonGuid IS NOT NULL
BEGIN
	-- CK_tblEquipmentMaintenanceLog_InServiceFlag
	SET @Sql=''IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''''[dbo].[CK_tblEquipmentMaintenanceLog_InServiceFlag]'''') AND parent_object_id = OBJECT_ID(N''''[dbo].[tblEquipmentMaintenanceLog]'''')) ALTER TABLE [dbo].[tblEquipmentMaintenanceLog] DROP CONSTRAINT [CK_tblEquipmentMaintenanceLog_InServiceFlag] ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql
	SET @Sql =''IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''''[dbo].[CK_tblEquipmentMaintenanceLog_InServiceFlag]'''') AND parent_object_id = OBJECT_ID(N''''[dbo].[tblEquipmentMaintenanceLog]'''')) ''
	SET @SQL+=''ALTER TABLE [dbo].[tblEquipmentMaintenanceLog]  WITH CHECK ADD  CONSTRAINT [CK_tblEquipmentMaintenanceLog_InServiceFlag] CHECK  (([InServiceFlag]=(1) AND [MaintenanceReasonGuid]=''''''+@MaintReasonGuid+'''''' OR [InServiceFlag]=(0) AND [MaintenanceReasonGuid]<>''''''+@MaintReasonGuid+''''''))''
	PRINT @SQL
	EXEC sp_executesql @Statement=@Sql

	-- CK_tblTankMaintenanceLog_InServiceFlag
	SET @Sql=''IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''''[dbo].[CK_tblTankMaintenanceLog_InServiceFlag]'''') AND parent_object_id = OBJECT_ID(N''''[dbo].[tblTankMaintenanceLog]'''')) ALTER TABLE [dbo].[tblTankMaintenanceLog] DROP CONSTRAINT [CK_tblTankMaintenanceLog_InServiceFlag] ''
	PRINT @Sql
	EXEC sp_executesql @Statement=@Sql

	SET @Sql =''IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N''''[dbo].[CK_tblTankMaintenanceLog_InServiceFlag]'''') AND parent_object_id = OBJECT_ID(N''''[dbo].[tblTankMaintenanceLog]'''')) ''
	SET @SQL+=''ALTER TABLE [dbo].[tblTankMaintenanceLog]  WITH CHECK ADD  CONSTRAINT [CK_tblTankMaintenanceLog_InServiceFlag] CHECK  (([InServiceFlag]=(1) AND [MaintenanceReasonGuid]=''''''+@MaintReasonGuid+'''''' OR [InServiceFlag]=(0) AND [MaintenanceReasonGuid]<>''''''+@MaintReasonGuid+''''''))''
	PRINT @SQL
	EXEC sp_executesql @Statement=@Sql

END


PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00170 Deploy Triggers]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00170 Deploy Triggers', 
		@step_id=52, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DECLARE @Sql NVARCHAR(max)
	,	@Object NVARCHAR(500)
	,	@Schema NVARCHAR(100)
	,	@Definition NVARCHAR(max)
	
DECLARE DropTriggerCursor CURSOR FOR
	SELECT trg.name
		,	sch.name	
	FROM sys.objects trg
	INNER JOIN sys.schemas sch ON sch.schema_id=trg.schema_id
	WHERE LEFT(trg.name,10)=''trg_Audit_''
OPEN DropTriggerCursor
FETCH NEXT FROM DropTriggerCursor INTO @Object,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	SET @Sql = ''DROP TRIGGER [''+@Schema+''].[''+@Object+'']''
	PRINT @Sql
	EXEC sp_executesql @statment=@Sql
	FETCH NEXT FROM DropTriggerCursor INTO @Object,@Schema
END
CLOSE DropTriggerCursor
DEALLOCATE DropTriggerCursor


PRINT ''Recreating triggers...''

DECLARE TriggerCursor CURSOR FOR
	SELECT	md.[definition] as SpDefinition
		,	pr.name as SpName
		,	sc.name as SchemaName
	FROM FuelsManagerDB_Template.sys.sql_modules md
	INNER JOIN FuelsManagerDB_Template.sys.objects pr on pr.object_id=md.object_id
	INNER JOIN FuelsManagerDB_Template.sys.schemas sc on sc.schema_id=pr.schema_id
	WHERE not exists(select 1 from sys.objects pr2 INNER JOIN sys.schemas sc2 on sc2.schema_id=pr2.schema_id where pr2.name=pr.name and sc.name=sc2.name)
	AND  pr.[type] IN (''tr'')
	ORDER BY sc.name,pr.name
OPEN TriggerCursor
FETCH NEXT FROM TriggerCursor INTO @Definition,@Object,@Schema
WHILE @@FETCH_STATUS=0
BEGIN
	PRINT @Definition
	EXEC(@Definition)

	FETCH NEXT FROM TriggerCursor INTO  @Definition,@Object,@Schema
END
CLOSE TriggerCursor
DEALLOCATE TriggerCursor


PRINT ''Completed successfully''
GO



PRINT ''''
PRINT ''*****************************************************************''
PRINT ''Recreating triggers complete.''
PRINT ''*****************************************************************''
PRINT ''''
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00180 Deploy Security]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00180 Deploy Security', 
		@step_id=53, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
		DECLARE @UserRoleName sysname
		SET @UserRoleName = N''FMDUserRole''

		IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @UserRoleName AND type = ''R'')
		IF @UserRoleName <> N''public'' AND (select is_fixed_role FROM sys.database_principals WHERE name = @UserRoleName) = 0
		BEGIN
			DECLARE @UserRoleMemberName sysname
			DECLARE Member_Cursor CURSOR FOR
			SELECT [name]
			FROM sys.database_principals 
			WHERE principal_id IN ( 
				SELECT member_principal_id
				FROM sys.database_role_members
				WHERE role_principal_id IN (
					SELECT principal_id
					FROM sys.database_principals WHERE [name] = @UserRoleName AND type = ''R''))

			OPEN Member_Cursor;

			FETCH NEXT FROM Member_Cursor
			INTO @UserRoleMemberName
    
			DECLARE @UserAlterSQL NVARCHAR(4000)

			WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @UserAlterSQL = ''ALTER ROLE ''+ QUOTENAME(@UserRoleName,''['') +'' DROP MEMBER ''+ QUOTENAME(@UserRoleMemberName,''['')
				EXEC(@UserAlterSQL)
        
				FETCH NEXT FROM Member_Cursor
				INTO @UserRoleMemberName
			END;

			CLOSE Member_Cursor;
			DEALLOCATE Member_Cursor;
		END
		/****** Object:  DatabaseRole [FMDUserRole]    Script Date: 2/20/2019 5:27:32 PM ******/
		IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N''FMDUserRole'' AND type = ''R'')
		DROP ROLE [FMDUserRole]
		GO

		DECLARE @AdminRoleName sysname
		SET @AdminRoleName = N''FMDAdminRole''
		IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @AdminRoleName AND type = ''R'')
		IF @AdminRoleName <> N''public'' AND (select is_fixed_role FROM sys.database_principals WHERE name = @AdminRoleName) = 0
		BEGIN
			DECLARE @AdminRoleMemberName sysname
			DECLARE Member_Cursor CURSOR FOR
			SELECT [name]
			FROM sys.database_principals 
			WHERE principal_id IN ( 
				SELECT member_principal_id
				FROM sys.database_role_members
				WHERE role_principal_id in (
					SELECT principal_id
					FROM sys.database_principals WHERE [name] = @AdminRoleName AND type = ''R''))

			OPEN Member_Cursor;

			FETCH NEXT FROM Member_Cursor
			INTO @AdminRoleMemberName
    
			DECLARE @AdminAlterSQL NVARCHAR(4000)

			WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @AdminAlterSQL = ''ALTER ROLE ''+ QUOTENAME(@AdminRoleName,''['') +'' DROP MEMBER ''+ QUOTENAME(@AdminRoleMemberName,''['')
				EXEC(@AdminAlterSQL)
        
				FETCH NEXT FROM Member_Cursor
				INTO @AdminRoleMemberName
			END;

			CLOSE Member_Cursor;
			DEALLOCATE Member_Cursor;
		END
		/****** Object:  DatabaseRole [FMDAdminRole]    Script Date: 2/20/2019 5:27:27 PM ******/
		IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N''FMDAdminRole'' AND type = ''R'')
		DROP ROLE [FMDAdminRole]
		GO


--DECLARE ObjCursor CURSOR FOR
--	SELECT	sc.name 
--		,	ob.name
--		,	ob.Type_Desc
--	FROM	sys.objects ob
--	INNER JOIN sys.schemas sc on sc.schema_id=ob.schema_id
--	WHERE ob.Type_Desc IN(''USER_TABLE'',''SQL_TABLE_VALUED_FUNCTION'',''SQL_STORED_PROCEDURE'',''SQL_INLINE_TABLE_VALUED_FUNCTION'',''SQL_SCALAR_FUNCTION'')
--	ORDER BY ob.Type_Desc,sc.name,ob.name
--OPEN ObjCursor
--FETCH NEXT FROM ObjCursor INTO @Schema,@ObjName,@Type
--WHILE @@FETCH_STATUS=0
--BEGIN

--	IF @Type IN(''USER_TABLE'')
--		SET @Sql=''GRANT DELETE,INSERT,SELECT,REFERENCES ON OBJECT::[''+@Schema+''].[''+@ObjName+''] TO FMDUserRole;''
--	IF @Type IN(''SQL_TABLE_VALUED_FUNCTION'',''SQL_INLINE_TABLE_VALUED_FUNCTION'')
--		SET @Sql=''GRANT SELECT,REFERENCES ON OBJECT::[''+@Schema+''].[''+@ObjName+''] TO FMDUserRole;''
--	IF @Type IN (''SQL_STORED_PROCEDURE'',''SQL_SCALAR_FUNCTION'')
--		SET @Sql=''GRANT EXECUTE,REFERENCES ON OBJECT::[''+@Schema+''].[''+@ObjName+''] TO FMDUserRole;''
		
--	PRINT @Sql
--	EXEC sp_executesql @Statement=@Sql
	
--	FETCH NEXT FROM ObjCursor INTO @Schema,@ObjName,@Type
--END
--CLOSE ObjCursor
--DEALLOCATE ObjCursor
--GO



PRINT ''Completed successfully''
GO

', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00900 Update Database Version]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00900 Update Database Version', 
		@step_id=54, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	UPDATES tblVersion from latest version available on template database
*/

INSERT INTO dbo.tblVersion(
	[Version]
,	[PackageName]
,	[DateApplied]
,	[Comments]
,	[Check1]
,	[Check2]
,	[CreatedDate]
,	[CreatedBy]
,	[UpdatedBy]
)
SELECT
	[Version]
,	[PackageName]
,	SYSDATETIMEOFFSET()
,	[Comments] + '' - Migration''
,	[Check1]
,	[Check2]
,	SYSDATETIMEOFFSET()
,	[CreatedBy]
,	[UpdatedBy]
FROM FuelsManagerDB_Template.dbo.tblVersion
WHERE VersionIndex=(SELECT TOP 1 VersionIndex FROM FuelsManagerDB_Template.dbo.tblVersion ORDER BY VersionIndex Desc)

PRINT ''Completed successfully''


GO
', 
		@database_name=N'ConsolidatedDB', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 00920 Rename Databases]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 00920 Rename Databases', 
		@step_id=55, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/*
	SCRIPT TO RENAME DATABASE FROM ConsolidatedDB TO FuelsManagerDB
	CANNOT BE EXECUTED ON AZURE DATABASE
*/
IF EXISTS(SELECT 1 FROM sys.databases WHERE name=''ConsolidatedDB'') AND NOT EXISTS(SELECT 1 FROM sys.databases WHERE name=''FuelsManagerDB'')
BEGIN
	ALTER DATABASE ConsolidatedDB
	SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	EXEC sp_renamedb ''ConsolidatedDB'',''FuelsManagerDB''
	ALTER DATABASE FuelsManagerDB
	SET MULTI_USER WITH ROLLBACK IMMEDIATE
	PRINT ''Renamed ConsolidatedDB to FuelsManagerDB.''
END
ELSE
	PRINT ''Renaming ConsolidatedDB failed. Database FuelsManagerDB exists.''
GO


IF EXISTS(SELECT 1 FROM sys.databases WHERE name=''ConsolidatedDBArchive'') AND NOT EXISTS(SELECT 1 FROM sys.databases WHERE name=''FuelsManagerDBArchive'')
	EXEC sp_renamedb ''ConsolidatedDBArchive'',''FuelsManagerDBArchive''
ELSE
	PRINT ''Renaming ConsolidatedDBArchive failed. Database FuelsManagerDBArchive exists.''
GO
', 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 01000-1 New Configurations and Data]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 01000-1 New Configurations and Data', 
		@step_id=56, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FuelsManagerDB
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- 56618 - Migration must create a Service Account user group
--
--BEGIN TRANSACTION
--DECLARE @CreatedBy nvarchar(100) = ''V9 Upgrade. AAC''

--IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblGroups] g JOIN dbo.tblSites s ON g.SiteGuid=s.SiteGuid WHERE g.GroupID = ''Service Account'' AND s.ID=''SiteAdmin''))
--	INSERT INTO [dbo].[tblGroups]
--	(
--		  [GroupID]
--		  ,[GroupDescription]
--		  ,[CreatedDate]
--		  ,[CreatedBy]
--		  ,[UpdatedDate]
--		  ,[UpdatedBy]
--		  ,[GroupGuid]
--		  ,[SiteGuid]
--	)
--	SELECT
--		  ''Service Account'' AS [GroupID]
--		  ,''Service Account'' AS [GroupDescription]
--		  ,SysDateTimeOffset() AS [CreatedDate]
--		  ,@CreatedBy AS [CreatedBy]
--		  ,SysDateTimeOffset() AS [UpdatedDate]
--		  ,@CreatedBy AS [UpdatedBy]
--		  ,''18F69989-FAD2-4074-8DE5-A73D75078349'' AS [GroupGuid]
--		  ,SiteGuid AS [SiteGuid]
--	FROM dbo.tblSites WHERE ID=''SiteAdmin''

--DECLARE @GroupGuid UniqueIdentifier = (SELECT g.GroupGuid FROM [dbo].[tblGroups] g JOIN dbo.tblSites s ON g.SiteGuid=s.SiteGuid WHERE g.GroupID = ''Service Account'' AND s.ID=''SiteAdmin'')
--INSERT INTO [map].[tblEntityUserGroupToSite] ([UserGroupToSiteGuid], [GroupGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) 
--	SELECT newid() as GroupToSiteGuid, @GroupGuid, ss.ChildSiteGuid, SYSDATETIMEOFFSET(), @CreatedBy, SYSDATETIMEOFFSET(), @CreatedBy, ss.ParentSiteGuid
--	FROM dbo.tblSites s JOIN map.tblSiteToSite ss ON s.SiteGuid=ss.ParentSiteGuid AND s.SiteGuid = CONVERT(uniqueidentifier, N''00000000-0000-0000-0000-000000000001'')
--	WHERE NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblEntityUserGroupToSite] m WHERE m.GroupGuid=@GroupGuid AND m.SiteGuid=ss.ChildSiteGuid AND m.[AssignedFromSiteGuid]=ss.ParentSiteGuid)

--INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
--	SELECT newid() as GroupToRightGuid, @GroupGuid, RightIndex, SYSDATETIMEOFFSET(), @CreatedBy, SYSDATETIMEOFFSET(), @CreatedBy
--	FROM lookup.tblRight r WHERE RightCode IN (
--			''PERFORM_SYNCHRONIZATION'',
--			''VIEW_SYNC_CONFIG_CLIENT_SETTINGS'',
--			''VIEW_SYNC_CONFLICT_STATUS'', 
--			''VIEW_SYNC_CONFIG_PERIODIC_SETTINGS'',
--			''VIEW_SYNC_CONFIG_SERVER_SETTINGS'',
--			''MODIFY_SYNC_CONFIG_SERVER_SETTINGS'',
--			''MODIFY_SYNC_CONFIG_CLIENT_SETTINGS'',
--			''MODIFY_CONFIGURATION_SETTINGS'',
--			''MODIFY_SYNC_CONFLICT_STATUS'',
--			''MODIFY_SYNC_CONFIG_PERIODIC_SETTINGS'') 
--	AND NOT EXISTS (SELECT TOP 1 1 FROM [map].[tblGroupToRight] m WHERE m.GroupGuid=@GroupGuid AND r.RightIndex=m.[LookupRightIndex])
--PRINT ''Successfully Completed''
--COMMIT TRANSACTION
GO

---- 
---- The TransactionStatus column is now LookupTransactionStatusIndex so we need to correct any TransactionAliasFields to use the new DbName.
----

IF EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] WHERE DbName = ''TransactionStatus'')
BEGIN
	DECLARE @date datetimeoffset(7) = GetDate();
	UPDATE [dbo].[tblTransactionAliasFields] SET DbName = ''LookupTransactionStatusIndex'' WHERE DbName = ''TransactionStatus'' 
END
GO


DELETE FROM tblUserDataFieldEquipment WHERE [Number]=23
INSERT INTO tblUserDataFieldEquipment
	(
	    UserDataFieldEquipmentGuid
	,   SiteGuid
	,	[Number]
	,	DisplayOrder
	,	DisplayName
	,	[LookupUserDataTypeIndex]
	,	CreatedDate
	,	CreatedBy
	,	UpdatedDate
	,	UpdatedBy
	)
	VALUES 
	(
		N''3957D8E4-DCA2-4CE3-96A2-1605442FB19C'', 
		N''00000000-0000-0000-0000-000000000001'',
		23,
		0,
		''Pulse Ratio'',
		0,
		GETUTCDATE(),
		''V9 Upgrade'',
		GETUTCDATE(),
		''V9 Upgrade''
	)
GO

UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''FM7Accounting.VDateTime'', 
									''System.DateTimeOffset''))
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''FM7Accounting.VBool'', 
									''System.Boolean''))
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''tblExportResultDetails'', 
									''tblExportResultDetails''))

UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''ConsolidatedBLL.EquipmentsClass'', 
									''FMBusinessObjects.DataObjects.EquipmentClass''))
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''FMBusinessObjects.DataObjects.EquipmentClass/EquipmentID'', 
									''FMBusinessObjects.DataObjects.EquipmentClass/ID''))

UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''<Value>%dometer%</Value>'', 
									''<Value/>''))
									WHERE  convert(nvarchar(max),queryxml) like ''%[%]dometer[%]%'' 

UPDATE fuelsmanagerdb.[dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),	
									''<Value>GUR</Value>'', 
									''<Value/>''))
			WHERE convert(xml,queryxml).exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Criterion/QueryCriteriaPhrase[contains(Operator[1],"NullOrEmpty")]/Value[text()="GUR"]'') = 1


UPDATE dbo.tblquerystorage SET queryxml=CONVERT(text,REPLACE(CONVERT(nvarchar(MAX), queryxml),''ConsolidatedDataObjects'',''FMBusinessObjects.DataObjects'')) 
	WHERE CONVERT(nvarchar(MAX), queryxml) LIKE ''%ConsolidatedDataObjects%''



 ;WITH a AS (SELECT querystorageguid, s.siteguid, s.id AS siteid,queryname,CONVERT(nvarchar(MAX), queryxml) AS querystr 
				FROM dbo.tblquerystorage q JOIN dbo.tblsites s ON q.siteguid=s.siteguid)
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''11/31/'', ''11/30/''))
			FROM a 
			WHERE [dbo].[tblQueryStorage].QueryStorageGuid=a.QueryStorageGuid AND  querystr like  ''%11/31/%'' 


;WITH a AS (SELECT querystorageguid, s.siteguid, s.id AS siteid,queryname,CONVERT(nvarchar(MAX), queryxml) AS querystr 
				FROM dbo.tblquerystorage q JOIN dbo.tblsites s ON q.siteguid=s.siteguid)
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''9/31/'', ''9/30/''))
			FROM a 
			WHERE [dbo].[tblQueryStorage].QueryStorageGuid=a.QueryStorageGuid AND querystr like  ''%9/31/%'' 


;WITH a AS (SELECT querystorageguid, s.siteguid, s.id AS siteid,queryname,CONVERT(nvarchar(MAX), queryxml) AS querystr 
				FROM dbo.tblquerystorage q JOIN dbo.tblsites s ON q.siteguid=s.siteguid)
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''4/31/'', ''4/30/''))
			FROM a 
			WHERE [dbo].[tblQueryStorage].QueryStorageGuid=a.QueryStorageGuid AND  querystr like  ''%4/31/%'' 


;WITH a AS (SELECT querystorageguid, s.siteguid, s.id AS siteid,queryname,CONVERT(nvarchar(MAX), queryxml) AS querystr 
				FROM dbo.tblquerystorage q JOIN dbo.tblsites s ON q.siteguid=s.siteguid)
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''6/31/'', ''6/30/''))
			FROM a 
			WHERE [dbo].[tblQueryStorage].QueryStorageGuid=a.QueryStorageGuid AND querystr like  ''%6/31/%'' 

;WITH a AS (SELECT querystorageguid, s.siteguid, s.id AS siteid,queryname,CONVERT(nvarchar(MAX), queryxml) AS querystr 
				FROM dbo.tblquerystorage q JOIN dbo.tblsites s ON q.siteguid=s.siteguid)
UPDATE [dbo].[tblQueryStorage] 
	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''10/1/014'', ''10/1/2014''))
			FROM a 
			WHERE [dbo].[tblQueryStorage].QueryStorageGuid=a.QueryStorageGuid AND querystr like  ''%10/1/014%'' 



UPDATE q	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''6/12015'', ''6/1/2015''))
			FROM [dbo].[tblQueryStorage] q JOIN dbo.tblsites s ON q.siteguid=s.siteguid
			WHERE id=''DLA ENERGY PACIFIC'' AND QueryName=''Receipt Contract'' 

UPDATE q 	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''28/03/2015'', ''03/28/2015''))
			FROM  [dbo].[tblQueryStorage] q JOIN dbo.tblsites s ON q.siteguid=s.siteguid
			WHERE id=''SE5F3U'' AND QueryName=''Daily A-10''

UPDATE q 	SET [QueryXML]=CONVERT(TEXT, REPLACE(CONVERT(NVARCHAR(MAX), [QueryXML]),''113114'', ''11/30/2014''))
			FROM  [dbo].[tblQueryStorage] q JOIN dbo.tblsites s ON q.siteguid=s.siteguid
			WHERE QueryName=''CX0756''

BEGIN TRANSACTION

select querystorageguid, s.ID as SiteID, queryname, convert(xml,[QueryXML]) as [QueryXML] into #tmp  from [dbo].[tblQueryStorage] q JOIN dbo.tblSites s ON s.SiteGuid=q.SiteGuid

update #tmp set  queryxml.modify(''replace value of (/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/Value[text() = "Y"]/text())[1] with "true"'')
where siteid=''se5f30'' and queryname=''test'' and queryxml.exist(''/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/Value[text() = "Y"]'')=1

update #tmp set  queryxml.modify(''replace value of (/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/FieldName[text() = "Flag02"]/text())[1] with "Alias"'')
where siteid=''SE5F4G'' and queryname=''Capitalized Receive Contract'' and queryxml.exist(''/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/FieldName[text() = "Flag02"]'')=1

update #tmp set  queryxml.modify(''delete (/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/Value[text() = "se5n1k"]/parent::QueryCriteriaPhrase)[1]'')
where  queryname = ''sales'' and siteid=''SE5N1K'' 

UPDATE #tmp set queryxml.modify(''delete (//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Criterion/QueryCriteriaPhrase[contains(Operator[1],"NullOrEmpty")]/Value[text()="GUR"])[1]'')
WHERE queryname = ''Unsent Transactions'' and siteid=''SE5F47''


--update #tmp set  queryxml.modify(''insert <Value>''''x0'''',''''x1'''',''''x2''''</Value>  into (/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase/FieldName[text() = "InterfaceData01"]/parent::QueryCriteriaPhrase)[1]'')
--where  queryname = ''MILSTRIP'' and siteid=''SE8N0H'' 

update a
set queryxml.modify(''
insert <TransactionAliasGuids/> into (//FuelsManager.Queries/FuelsManager.Query)[1]'')
from #tmp a
where queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/child::TransactionAliasGuids'') <>1
and queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]'') = 1

--Remove unobtainable from DisplayName for transaction aliases that are not physical inventory
update a
set queryxml.modify(''
replace value of (/FuelsManager.Queries/FuelsManager.Query/Fields/QueryWriterField/DisplayName[text() = "Quantity (lbs)/Unobtainable/Shipped Quantity"]/text())[1]  
       with "Quantity (lbs)/Shipped Quantity"'')
from #tmp a
where queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Fields/QueryWriterField[contains(DisplayName[1],"Unobtainable")]'') =1
and 
queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Criterion/QueryCriteriaPhrase[contains(Value[1],"Physical Inventory")]'') <> 1

--replace tbltransactionlineitems.number01 with tbltransactions.number01 for queries referencing Physical Inventory
update a
set queryxml.modify(''
replace value of (/FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Fields/QueryWriterField/DBFieldName[text() = "tblTransactionLineItems.Number01"]/text())[1]  
       with "tblTransactions.Number01" '')
from #tmp a
where 
queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Fields/QueryWriterField[contains(DBFieldName[1],"tblTransactionLineItems.Number01")]/parent::Fields/parent::FuelsManager.Query/Criterion/QueryCriteriaPhrase[contains(Value[1],"Physical Inventory")]'') =1

--Use only unobtainable for displayname if query referencing Physical Inventory
update a
set queryxml.modify(''
replace value of (/FuelsManager.Queries/FuelsManager.Query/Fields/QueryWriterField/DisplayName[text() = "Quantity (lbs)/Unobtainable/Shipped Quantity"]/text())[1]  
       with "Unobtainable"'')
from #tmp a
where 
queryXml.exist(''//FuelsManager.Queries/FuelsManager.Query[contains(TopicIDType[1],"FMBusinessObjects.DataObjects.TransactionDO")]/Fields/QueryWriterField[contains(DisplayName[1],"Unobtainable")]/parent::Fields/parent::FuelsManager.Query/Criterion/QueryCriteriaPhrase[contains(Value[1],"Physical Inventory")]'') =1

-- update the results
update qs set qs.QueryXML = convert(nvarchar(max), t.queryxml)
FROM tblQueryStorage qs inner join #tmp t on qs.QueryStorageGuid = t.QueryStorageGuid

COMMIT TRANSACTION

GO

select siteguid, QueryStorageGuid, convert(xml, queryxml) ''queryxml'' into #tmpT from tblQueryStorage --where queryname like ''peter%'' --where querystorageguid = ''7745BD4F-2A93-49F5-A8FE-37896BD33C75''
--drop table #tmpT


set nocount on
declare @queryXml xml, @nodexml xml, @nodeCount int, @loopval int, @siteGuid uniqueidentifier, @querystorageguid uniqueidentifier, 
@nodeindex int, @shortDateformat nvarchar(100), @longDateformat nvarchar(100), @needChange int, @datetype int



DECLARE queryCursor CURSOR FOR
	SELECT	
		Siteguid
	,	QueryStorageGuid
	,	queryxml
	FROM #tmpT
	where queryxml.value(''(/FuelsManager.Queries/FuelsManager.Query/TopicIDType)[1]'', ''nvarchar(100)'') IN (''FMBusinessObjects.DataObjects.TransactionDO'', ''FMBusinessObjects.DataObjects.EquipmentClass'')

OPEN queryCursor
FETCH NEXT FROM queryCursor INTO @siteGuid,@querystorageguid,@queryXml
WHILE @@FETCH_STATUS=0
BEGIN

	DECLARE @newCriteriaRootNode xml

	select @shortDateformat = shortdatepattern, @longDateformat = ShortDatePattern + '' '' + timepattern, @needChange = 0 from dbo.tblSites where siteguid	= @siteGuid

	DECLARE nodeCursor CURSOR FOR
	SELECT ROW_NUMBER() over (order by (select 1)), c.query(''.'')
	from @queryXml.nodes(''/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase'') T(c)

	OPEN nodeCursor
	FETCH NEXT FROM nodeCursor INTO @nodeIndex,@nodexml
	WHILE @@FETCH_STATUS=0
	BEGIN
		Declare @newDateVal nvarchar(2000)
		
		declare @baddate nvarchar(100), @queryObjectType nvarchar(250), @queryFieldName nvarchar(250)

		select @datetype = 0, @newDateVal = null, @queryObjectType = @nodexml.value(''(/QueryCriteriaPhrase/TopicObjectType)[1]'', ''nvarchar(100)''), @queryFieldName =  @nodexml.value(''(/QueryCriteriaPhrase/FieldName)[1]'', ''nvarchar(100)'')

		IF (@queryObjectType = ''FMBusinessObjects.DataObjects.TransactionDO'' and @queryFieldName IN (''InventoryDateAsDateOnly'',''EffectiveDateAsDateOnly'',''ExpirationDateAsDateOnly''))
		OR (@queryObjectType = ''FMBusinessObjects.DataObjects.EquipmentClass'' and @queryFieldName IN (''LockedOutDateTime''))
		BEGIN
			select @dateType = 1 -- date only, no time
			set @needChange = 1
		END

		IF (@queryObjectType = ''FMBusinessObjects.DataObjects.TransactionDO'' 
			and @queryFieldName IN (''TransactionDateTime'',''CreatedDate'',''UpdatedDate'',''RequestedDateTime'',''DispatchedDateTime'',''FST'',''TimeIn'',''TimeOut'',''TimeEnd'',''RequestedDeliveryDate'',''ScheduledDate'',''Date01'',''Date02'',''Date03'',''Date04''))
		BEGIN
			select @dateType = 2 -- date with time
			set @needChange = 1
		END

		IF @datetype = 1 -- date no time
		BEGIN
			declare @tmpDate datetime 

			IF ( @nodexml.value(''(/QueryCriteriaPhrase/Operator)[1]'', ''nvarchar(5)'') = ''IN'')
			BEGIN
				set @needChange = 2

				--converts a list of comma seperated dates to a comma seperated list of dates in site format
				SET @newDateVal = stuff((select '','' + format(dates, @shortDateformat) FROM 
				(SELECT 
				m.n.value(''.[1]'', ''date'') as dates from (
				Select cast(''<r><d>'' + replace(sub.dates2, '','',''</d><d>'') + ''</d></r>'' as xml) as x 
				from (select @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''nvarchar(1000)'') as dates2) as sub )t cross apply x.nodes(''/r/d'')m(n)
				where isdate(LTRIM(RTRIM(m.n.value(''.[1]'', ''nvarchar(50)'')))) = 1) sub
				FOR XML PATH ('''')),1,1,'''')

			END
			ELSE
			BEGIN
				begin try
					set @tmpDate = @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''date'')

					SET @newDateVal = format(@tmpDate, @shortDateformat)
				end try
				begin catch
					--nothing happening
					SET @newDateVal = ''''
					set @baddate =  @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''nvarchar(100)'')
					print ''invalid date: '' + @baddate
				end catch
			END
		END

		ELSE IF @datetype = 2 -- date with time
		BEGIN
			declare @tmpDateTime datetime 
			
			IF ( @nodexml.value(''(/QueryCriteriaPhrase/Operator)[1]'', ''nvarchar(5)'') = ''IN'')
			BEGIN
			--declare @var nvarchar(1000)
				set @needChange = 2

				--converts a list of comma seperated dates to a comma seperated list of dates in site format
				SET @newDateVal = stuff((select '','' + format(dates, @longDateformat) FROM 
				(SELECT 
				m.n.value(''.[1]'', ''datetime'') as dates from (
				Select cast(''<r><d>'' + replace(sub.dates2, '','',''</d><d>'') + ''</d></r>'' as xml) as x 
				from (select @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''nvarchar(1000)'') as dates2) as sub )t cross apply x.nodes(''/r/d'')m(n)
				where isdate(LTRIM(RTRIM(m.n.value(''.[1]'', ''nvarchar(50)'')))) = 1) sub
				FOR XML PATH ('''')),1,1,'''')

			END
			ELSE
			BEGIN
				begin try

					set @tmpDateTime = @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''datetime'')

					SET @newDateVal = format(@tmpDateTime, @longDateformat)
				end try
				begin catch
					--nothing happening
					SET @newDateVal = ''''

					set @baddate =  @nodexml.value(''(/QueryCriteriaPhrase/Value)[1]'', ''nvarchar(100)'')
					print ''invalid date: '' + @baddate
				end catch
			END
		END

		IF @needChange > 0 and @newDateVal is not null
		BEGIN
			--SET @nodexml.modify(''replace value of (/QueryCriteriaPhrase/Value/text())[1] with sql:variable("@newDateVal")'')
			SET @queryXml.modify(''replace value of (/FuelsManager.Queries/FuelsManager.Query/Criterion/QueryCriteriaPhrase[sql:variable("@nodeIndex")]/Value/text())[1] with sql:variable("@newDateVal")'')
		END

		FETCH NEXT FROM nodeCursor INTO @nodeIndex,@nodexml
	END
	CLOSE nodeCursor
	DEALLOCATE nodeCursor

	IF @needChange > 0
	BEGIN
		UPDATE dbo.tblquerystorage SET QueryXML = cast(@queryXml as varchar(max)) where QueryStorageGuid = @querystorageguid
			--select @queryXml
	END

	FETCH NEXT FROM queryCursor INTO @siteGuid,@querystorageguid,@queryXml
END
CLOSE queryCursor
DEALLOCATE queryCursor

DROP TABLE #tmpT
GO

-- ***************************************************************
-- THIS SHOULD NOW GET POPULATED WHEN WE PULL DOWN THE INITIAL
-- ENTERPRISE SETUP FOR THIS SITE.
-- ***************************************************************

-- Add missing user to site entity assignments for administrator.
/* NEED TO COMMENT THIS OUT DURING v2 Merge Migration to Enterprise */
--INSERT INTO [map].[tblEntityUserToSite]
--(
--	userguid, siteguid, createddate, createdby, updateddate, updatedby, AssignedFromSiteGuid
--)
--SELECT userguid, s.siteguid, getdate(), ''V9 Upgrade s1000'', getdate(), ''V9 Upgrade s1000'', u.SiteGuid
--  FROM dbo.tblsites s, dbo.tblusers u  
--  where userid=''administrator'' and s.[siteguid] not in (select m.siteguid from 
--						  [map].[tblEntityUserToSite] m join dbo.tblusers uu on uu.userguid=m.[userguid] 
--						  join [dbo].[tblSites] ss on ss.siteguid=m.siteguid where uu.userguid=u.userguid)

--GO
INSERT INTO [map].[tblUserToGroup] 
(
	[UserToGroupGuid]
      ,[UserGuid]
      ,[GroupGuid]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[SiteGuid]
)
SELECT newid()--[UserToGroupGuid]
      ,s.[UserGuid]
      ,''00000000-0000-0000-0000-000000000003'' as [GroupGuid] --administrator user group
      ,getdate() as [CreatedDate]
      ,''V9 Upgrade s1000'' as [CreatedBy]
      ,getdate() as [UpdatedDate]
      ,''V9 Upgrade s1000'' as [UpdatedBy]
      ,s.[SiteGuid]
  FROM [map].[tblEntityUserToSite] s 
  where userguid=''00000000-0000-0000-0000-000000000002'' --administrator
  and not exists(select top 1 1 from [map].[tblUserToGroup] u where s.userguid=u.userguid and u.[GroupGuid]=''00000000-0000-0000-0000-000000000003''
  and s.siteguid=u.siteguid)

GO


DECLARE @Command VARCHAR(MAX) = ''ALTER AUTHORIZATION ON DATABASE::FuelsManagerDB TO [sa]'' 

SELECT @Command = REPLACE(REPLACE(@Command 
            , ''<FuelsManagerDB>'', SD.Name)
            , ''<sa>'', SL.Name)
FROM master..sysdatabases SD 
JOIN master..syslogins SL ON  SD.SID = SL.SID
WHERE  SD.Name = DB_NAME()

PRINT @Command
EXEC(@Command)

GO

Update tblSites Set MinTimeAllowedToChangePwd = ''1''
Update tblSites Set InactivityDisablePeriod = ''35''
GO
', 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 01000-2 New Configurations and data]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 01000-2 New Configurations and data', 
		@step_id=57, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FuelsManagerDB
GO

DECLARE @seq BIGINT
SELECT @seq = ISNULL(MAX([SequenceNumber]),0) FROM tblAlarmAndEventLog
UPDATE tblsequences SET sequencevalue = @seq WHERE sequencekey =''AlarmAndEventEmailSequence'' 

IF ( @@ROWCOUNT = 0)
BEGIN
       INSERT INTO tblsequences (SequenceKey, SequenceValue) VALUES (''AlarmAndEventEmailSequence'', @seq)
END

PRINT ''updated AlarmAndEventEmailSequence to '' + cast(@seq as nvarchar(20))

GO

ALTER DATABASE FuelsManagerDB
SET TRUSTWORTHY OFF
GO

', 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [TAS UPG 01000-3 Initialize sync.tblSyncAnchors for LARGE tables]    Script Date: 3/13/2019 4:58:52 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'TAS UPG 01000-3 Initialize sync.tblSyncAnchors for LARGE tables', 
		@step_id=58, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/* Initialize the SYNC Anchors so historical data on LARGE */
USE FuelsManagerDB
GO		
		
/* Initialize the SYNC Anchors so historical data on LARGE tables are skipped.  These should be imported into the Enterprise during a separate phase */
BEGIN TRANSACTION
	DECLARE @SiteID nvarchar(50)
	DECLARE @MIN_ROWVERSION binary(8)
	
	SELECT @SiteID = ID FROM dbo.tblSites WHERE SiteGroupFlag = 0
	SELECT @MIN_ROWVERSION = MIN_ACTIVE_ROWVERSION();

   ;WITH existingData AS (
        SELECT [sync].[tblSyncAnchor].[SyncAnchorGuid],[sync].[tblSyncAnchor].[SiteID],[sync].[tblSyncAnchor].[TableName],[sync].[tblSyncAnchor].[LastReceivedAnchor],[sync].[tblSyncAnchor].[LastSentAnchor1],[sync].[tblSyncAnchor].[LastSentAnchor2],[sync].[tblSyncAnchor].[LastReceivedAnchor2]
            FROM [sync].[tblSyncAnchor]
    ) MERGE existingData
    USING (VALUES
		(NULL, @SiteID, ''dbo.tblExportResults'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblExportResultDetails'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblAlarmAndEventLog'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblAuditLog'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactions'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionLineItems'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionNotes'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionPIDX'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionSignature'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionTransportLineItems'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionUserData'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionWeightReadings'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionLineItemUserData'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionSubLineItems'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblTransactionLinks'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblOwnerCloseout'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblCloseoutInventory'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
		,(NULL, @SiteID, ''dbo.tblMessageLog'', 0x000000000, @MIN_ROWVERSION, @MIN_ROWVERSION, 0x000000000)
	) AS changedData ([SyncAnchorGuid],[SiteID],[TableName],[LastReceivedAnchor],[LastSentAnchor1],[LastSentAnchor2],[LastReceivedAnchor2])
    ON (existingData.[SiteID] = changedData.[SiteID] AND existingData.[TableName] = changedData.[TableName])
    WHEN MATCHED
        THEN
        UPDATE SET [LastSentAnchor1] = changedData.[LastSentAnchor1]
					,[LastSentAnchor2] = changedData.[LastSentAnchor2]
	WHEN NOT MATCHED THEN
		INSERT ([SyncAnchorGuid],[SiteID],[TableName],[LastReceivedAnchor],[LastSentAnchor1],[LastSentAnchor2],[LastReceivedAnchor2]) 
		VALUES (newid(),changedData.[SiteID],changedData.[TableName],changedData.[LastReceivedAnchor],changedData.[LastSentAnchor1],changedData.[LastSentAnchor2],changedData.[LastReceivedAnchor2])
		;
        
    /* Fix for CITGO SAP Interface
       Allows for tblTransaction _RowVersion values to be greater than
       Transversion so they get processed by SAP service */
    DECLARE @TopRowVersion BIGINT
    SET @TopRowVersion = (
        SELECT TOP 1 CONVERT(BIGINT, _RowVersion)
        FROM tblTransactions
        WHERE AliasName = ''BOL''
        ORDER BY CONVERT(BIGINT, _RowVersion) DESC
    )
    UPDATE tblExportResults
    SET TransVersion = @TopRowVersion
    WHERE InterfaceName = ''CITGO BOL Export''
        AND LookupExportResultTypeIndex = 1
        AND CreatedBy = ''SAPInterface''


COMMIT TRANSACTION

PRINT ''Completed successfully''
GO

', 
		@database_name=N'master', 
		@output_file_name=@OutputFile, 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO


