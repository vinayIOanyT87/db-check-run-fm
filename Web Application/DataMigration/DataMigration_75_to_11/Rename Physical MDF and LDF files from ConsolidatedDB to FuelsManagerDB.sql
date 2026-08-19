/*
EXECUTE THE FOLLOWING STEPS to EXECUTE SCRIPTS / ACTIONS THAT WILL RENAME THE PHYSICAL DATABASE FILES FROM ConsolidatedDB to FuelsManagerDB

1. Execute the T-SQL in STEP 1 to get the RootPath for the Physical Database Files.  Save off the RootPath and PhysicalFileNames for each row.
2. Execute the T-SQL in STEP 2 to take the new FuelsManagerDB database offline
3. Using Windows Explorer, rename the physical MDF and LDF files; replacing ConsolidatedDB with FuelsManagerDB.
4. Update the system catalog with the new filenames by altering the T-SQL in STEP 4 with the full path of the MDF and LDF files plus the new file names.
5. Execute the T-SQL in STEP 5 to bring the new FuelsManagerDB database back online 
6. Update the Logical Names to match FuelsManagerDB by executing the T-SQL in STEP 6
7. Reset the database back to MULTI_USER
*/

/* STEP 1 - Locate the physical database file for the migrated FuelsManagerDB database */
SELECT
    db.name AS DBName,
	mf.name AS LogicalName,
    type_desc AS FileType,
    Physical_Name AS Location,
	LEFT(Physical_Name,LEN(Physical_Name) - charindex('\',reverse(Physical_Name),1) + 1) [RootPath], 
    RIGHT(Physical_Name, CHARINDEX('\', REVERSE(Physical_Name)) -1)  [PhysicalFileName]
FROM
    sys.master_files mf
INNER JOIN 
    sys.databases db ON db.database_id = mf.database_id
WHERE db.name = 'FuelsManagerDB'



/* STEP 2 - Take the FuelsManagerDB database offline */
USE [master];
GO
--Disconnect all existing session.
ALTER DATABASE FuelsManagerDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
--Change database in to OFFLINE mode.
ALTER DATABASE FuelsManagerDB SET OFFLINE


/* STEP 3 - Using Windows Explorer; rename the physical database files, replacing ConsolidatedDB with FuelsManagerDB */


/* STEP 4 - Update the System Catalog with the newly renamed physical files */
ALTER DATABASE FuelsManagerDB MODIFY FILE (Name='ConsolidatedDB', FILENAME='D:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERVER2014\MSSQL\DATA\FuelsManagerDB.mdf')
GO
ALTER DATABASE FuelsManagerDB MODIFY FILE (Name='ConsolidatedDB_log', FILENAME='D:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERVER2014\MSSQL\DATA\FuelsManagerDB_0.ldf')
GO

/* STEP 5 - Bring the FuelsManagerDB database back online */
ALTER DATABASE FuelsManagerDB SET ONLINE
GO

/* STEP 6 - Update the Logical Names to match */
ALTER DATABASE FuelsManagerDB MODIFY FILE (Name=ConsolidatedDB, NEWNAME=FuelsManagerDB)
GO
ALTER DATABASE FuelsManagerDB MODIFY FILE (Name=ConsolidatedDB_log, NEWNAME=FuelsManagerDB_log)
GO

/* STEP 7 - Update FuelsManagerDB and set it back to MULTI_USER */
ALTER DATABASE FuelsManagerDB SET MULTI_USER
GO
ALTER DATABASE FuelsManagerDB
SET MULTI_USER WITH ROLLBACK IMMEDIATE
GO
