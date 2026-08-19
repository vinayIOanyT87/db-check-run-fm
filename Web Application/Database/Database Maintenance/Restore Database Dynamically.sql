USE master
GO

DECLARE @DynSql NVARCHAR(4000)
	,	@DataFileLocation NVARCHAR(200)
	,	@BackupFileLocationAndName NVARCHAR(200)
	,	@DatabaseName NVARCHAR(200)
	
/*
==============================================================
		PARAMETER SECTION
==============================================================
*/
SET	@BackupFileLocationAndName = 'C:\Replication\ConsolidatedDB Envision Replication 4 Subscriber 201012215.bak'
SET @DataFileLocation = 'C:\Replication\ConsolidatedDB_Sub\'
--'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\' 
SET @DatabaseName = 'ConsolidatedDB_Sub'

/*
==============================================================
		EXECUTION SECTION
==============================================================
*/
SET @DynSql = 
'RESTORE DATABASE [' + @DatabaseName +  ']  
FROM  DISK = N''' + @BackupFileLocationAndName + '''  
WITH  FILE = 1,  
MOVE N''ConsolidatedDB'' TO N''' + @DataFileLocation + 'ConsolidatedDB.ConsolidatedDB'',  
MOVE N''ConsolidatedDB_log'' TO N''' + @DataFileLocation + 'ConsolidatedDB.ConsolidatedDB_LOG'',  
NOUNLOAD,  
REPLACE,  
STATS = 10 '


EXEC sp_executesql @stmt = @DynSql


