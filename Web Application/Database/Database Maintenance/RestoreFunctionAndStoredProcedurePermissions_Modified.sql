
/***************************************************************************** 
                                                                      */  print 
'NAME:    RestoreFunctionAndStoredProcedurePermissions.sql'                       /* 
                                                                               
 PURPOSE: Script to grant FMDUserRole access to the 
          principal objects of FuelsManager.                                   
                                                                               
 Copyright (C)  2009 Varec, Inc.     Norcross, GA, USA     All Rights Reserved 
                                                                               
 This file shall not be copied or reproduced in any form without the express   
 written consent of Varec, Inc.                                                
                                                                               
 DATE            CHANGED BY      VERSION      REASON                           
 ==========      ===========     ========     ================================ 
 2009-11-14      Chris Knight    Build 1      Initial creation.                
                                                                               
**************************************************************************** */ 

USE master
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'FMDUserRole' and type_desc = 'DATABASE_ROLE')
BEGIN
	CREATE ROLE FMDUserRole
END

GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'FMDAdminRole' and type_desc = 'DATABASE_ROLE')
BEGIN
	CREATE ROLE FMDAdminRole
END

GO



USE ConsolidatedDB
GO

EXEC sp_ChangeDBOwner 'sa','ConsolidatedDB'
GO

alter Database ConsolidatedDB set Trustworthy on
go

-- MUST be set as shown to support indexes on computed column or indexed view. 
SET ANSI_NULLS ON                                 -- Deprecated: leave set ON. 
SET ANSI_PADDING ON                               -- Deprecated: leave set ON. 
SET ANSI_WARNINGS ON                              -- No trailing blanks saved. 
SET ARITHABORT ON                                 -- Math failure not ignored. 
SET CONCAT_NULL_YIELDS_NULL ON                    -- NULL plus string is NULL. 
SET NUMERIC_ROUNDABORT OFF                        -- Allows loss of precision. 
SET QUOTED_IDENTIFIER ON                          -- Allows reserved keywords. 

-- Standard settings. 
SET NOCOUNT ON                                    -- Minimize network traffic. 
SET ROWCOUNT 0                                    -- Reset in case it got set. 
SET XACT_ABORT ON                                 -- Make transactions behave. 

IF DB_NAME() IN ('master', 'tempdb', 'model', 'msdb') RAISERROR('   $$$ YOU ARE ATTACHED TO A SYSTEM DB $$$', 20, 1) WITH NOWAIT, LOG


---------------------------------------------------------------------------- 
-- Create 'database role' FMDUserRole if it does not exist. 
---------------------------------------------------------------------------- 

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'FMDUserRole' and type_desc = 'DATABASE_ROLE')
BEGIN
	CREATE ROLE FMDUserRole
END

GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'FMDAdminRole' and type_desc = 'DATABASE_ROLE')
BEGIN
	CREATE ROLE FMDAdminRole
END

GO


---------------------------------------------------------------------------- 
-- Grant access to tables and views. 
---------------------------------------------------------------------------- 

DECLARE @sSql NVARCHAR(4000)
DECLARE @sName sysname

DECLARE cur CURSOR FOR
	SELECT name
	  FROM sys.objects
	 WHERE (type_desc = 'USER_TABLE' AND name LIKE 'tbl%')
		 OR (type_desc = 'VIEW'       AND name LIKE 'vw%')
	   AND is_ms_shipped = 0

--PRINT ' '
OPEN cur
FETCH NEXT FROM cur INTO @sName

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sSql = 'GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE ON ' + @sName + ' TO FMDUserRole '
	--PRINT @sSql
	EXEC sp_executesql @sSql
	FETCH NEXT FROM cur INTO @sName
END

CLOSE cur
DEALLOCATE cur

GO


---------------------------------------------------------------------------- 
-- Grant access to scalar functions. 
---------------------------------------------------------------------------- 

DECLARE @sSql NVARCHAR(4000)
DECLARE @sName sysname

DECLARE cur CURSOR FOR
	SELECT name
	  FROM sys.objects
	 WHERE type_desc IN ('SQL_SCALAR_FUNCTION', 'CLR_SCALAR_FUNCTION')
	   AND is_ms_shipped = 0

--PRINT ' '
OPEN cur
FETCH NEXT FROM cur INTO @sName

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sSql = 'GRANT EXECUTE, REFERENCES ON ' + @sName + ' TO FMDUserRole '
	--PRINT @sSql
	EXEC sp_executesql @sSql
	FETCH NEXT FROM cur INTO @sName
END

CLOSE cur
DEALLOCATE cur

GO


---------------------------------------------------------------------------- 
-- Grant access to table-valued functions. 
---------------------------------------------------------------------------- 

DECLARE @sSql NVARCHAR(4000)
DECLARE @sName sysname

DECLARE cur CURSOR FOR
	SELECT name
	  FROM sys.objects
	 WHERE type_desc IN ('SQL_TABLE_VALUED_FUNCTION', 'SQL_INLINE_TABLE_VALUED_FUNCTION', 'CLR_TABLE_VALUED_FUNCTION')
	   AND is_ms_shipped = 0

--PRINT ' '
OPEN cur
FETCH NEXT FROM cur INTO @sName

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sSql = 'GRANT REFERENCES, SELECT ON ' + @sName + ' TO FMDUserRole '
	--PRINT @sSql
	EXEC sp_executesql @sSql
	FETCH NEXT FROM cur INTO @sName
END

CLOSE cur
DEALLOCATE cur

GO


---------------------------------------------------------------------------- 
-- Grant access to stored procedures. 
---------------------------------------------------------------------------- 

DECLARE @sSql NVARCHAR(4000)
DECLARE @sName sysname

DECLARE cur CURSOR FOR
	SELECT name
	  FROM sys.objects
	 WHERE type_desc IN ('SQL_STORED_PROCEDURE', 'CLR_STORED_PROCEDURE', 'EXTENDED_STORED_PROCEDURE')
		AND (name LIKE 'fm%' OR name LIKE 'rpt%')
	   AND is_ms_shipped = 0

--PRINT ' '
OPEN cur
FETCH NEXT FROM cur INTO @sName

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sSql = 'GRANT EXECUTE ON ' + @sName + ' TO FMDUserRole '
	--PRINT @sSql
	EXEC sp_executesql @sSql
	FETCH NEXT FROM cur INTO @sName
END

CLOSE cur
DEALLOCATE cur

GO

------------------------------------------------------------------------------ 
-- Done. 
------------------------------------------------------------------------------ 

/*
8.0.0.5-027 WI 11010 Grant FMDUserRole SqlDependency Rights.sql
Chris Knight 3-Feb-2010

This script grants to users in FMDUserRole the ability to start and stop the SqlDependency, which is required by Dispatch
*/
Use ConsolidatedDB
GO

-- HJH - I added the folliowing two sttements:

IF EXISTS (select * from information_schema.schemata where schema_name = 'SQLDependency')
	DROP SCHEMA SQLDependency;
GO

-- HJH - Added 03/12/2010 (should have been part of the earlier SQL added as part of work item 11010.
IF NOT EXISTS (SELECT * FROM sysusers WHERE name = 'NT AUTHORITY\NETWORK SERVICE')
	CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE]
GO

-- Steps for those creating SqlDependency
CREATE SCHEMA [SQLDependency] AUTHORIZATION [NT AUTHORITY\NETWORK SERVICE]
GO

GRANT CREATE PROCEDURE to FMDUserRole 
GRANT CREATE QUEUE to FMDUserRole 
GRANT CREATE SERVICE to FMDUserRole 
GRANT REFERENCES on CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] to FMDUserRole  
GRANT VIEW DEFINITION TO FMDUserRole 
GRANT ALTER ON SCHEMA::SQLDependency to FMDUserRole 
GRANT CONTROL ON SCHEMA::SQLDependency to FMDUserRole
GRANT SUBSCRIBE QUERY NOTIFICATIONS to FMDUserRole
GO

-- Steps for those consuming SqlDependency
GRANT SELECT to FMDUserRole 
GRANT SUBSCRIBE QUERY NOTIFICATIONS TO FMDUserRole 
GRANT RECEIVE ON QueryNotificationErrorsQueue TO FMDUserRole 
GRANT REFERENCES on CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] to FMDUserRole
GO

-- 8.0.0.5-032 WI 11094 Add more permissions to FMDAdminRole and FMDUserRole.sql

use ConsolidatedDB
grant select on sys.database_principals to FMDAdminRole
grant select on sys.database_principals to FMDUserRole
grant select on sys.syscolumns to FMDUserRole
grant select on sys.services to FMDUserRole
grant select on sys.schemas to FMDUserRole
grant select on sys.objects to FMDUserRole
GO

use master
grant select on sys.database_principals to FMDAdminRole
grant select on sys.server_principals to FMDAdminRole
grant select on sys.databases to FMDUserRole
grant select on sys.services to FMDUserRole
GO

-- 8.0.0.5-035 WI 11132 Add permissions for database audit log.sql

GRANT SELECT ON sys.tables TO FMDAdminRole
GO

-- 8.0.0.5-043 WI 11801 Add more permissions for database audit log.sql
-- 2/23/2010 CHK
--
-- grants select access to fn_trace_gettable.
-- I missed this one earlier - one more thing missed by not
-- running the scripts to strip out [public]
--
Use master
GO
GRANT SELECT ON sys.fn_trace_gettable TO FMDAdminRole
GRANT SELECT ON tblFMDAuditConfiguration TO FMDAdminRole
GRANT INSERT ON tblFMDAuditConfiguration TO FMDAdminRole 
GRANT UPDATE ON tblFMDAuditConfiguration TO FMDAdminRole
GRANT DELETE ON tblFMDAuditConfiguration TO FMDAdminRole
GO

-- 8.0.0.5-056 WI 12217 grant role management rights to FMDAdminRole.sql

USE master
GO

GRANT EXECUTE ON sp_droprolemember TO FMDAdminRole
GRANT EXECUTE ON sp_dropsrvrolemember TO FMDAdminRole
GRANT EXECUTE ON sp_addrolemember TO FMDAdminRole
GRANT EXECUTE ON sp_addsrvrolemember TO FMDAdminRole
GO


------------------------------------------------------------------------------ 
-- Create BSME Site Admin login if it does not exist. 
------------------------------------------------------------------------------ 

USE master		-- Required for startup proc. 
GO
-- Taking special care not to modify XACT_ABORT state of master database. 
DECLARE @bWasOff BIT = 0

IF NOT @@OPTIONS & 16384 > 0
BEGIN
	RAISERROR('Turning XACT_ABORT to ''ON'' for ''master'' database', 10, 1) WITH NOWAIT, LOG
	SET @bWasOff = 1
	SET XACT_ABORT ON
END

-- Both, or neither. 
BEGIN TRAN

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'FMDAdminRole' AND type_desc = 'DATABASE_ROLE')
BEGIN
	RAISERROR('DATABASE_ROLE ''FMDAdminRole'' in database ''master'' should already exist!', 20, 1) WITH NOWAIT, LOG
END

RAISERROR('Granting EXECUTE right to FMDAdminRole', 10, 1) WITH NOWAIT, LOG
GRANT EXECUTE ON sys.sp_addsrvrolemember TO FMDAdminRole
COMMIT

-- Reset if changed. 
IF @bWasOff = 1
BEGIN
	RAISERROR('Turning XACT_ABORT back to ''OFF'' for ''master'' database', 10, 1) WITH NOWAIT, LOG
	SET XACT_ABORT OFF
END


/***************************************************************************** 
                                                                      */  print 
'NAME:    7.5.1.0-103 WI 5722 Turn SQL Server Broker On.sql'                /* 
                                                                               
 PURPOSE: Necessary for Dispatch.                                              
                                                                               
 Copyright (C)  2009 Varec, Inc.     Norcross, GA, USA     All Rights Reserved 
                                                                               
 This file shall not be copied or reproduced in any form without the express   
 written consent of Varec, Inc.                                                
                                                                               
 DATE            CHANGED BY      VERSION      REASON                           
 ==========      ===========     ========     ================================ 
 2009-09-25      L. Leonard      7.5 SP1      Upgrade script.                  
 2009-10-09      L. Leonard      7.5 SP1      Fixed call to usp_ErrorHandler.  
                                                                               
**************************************************************************** */ 


USE ConsolidatedDB
GO

-- MUST be set as shown to support indexes on computed column or indexed view. 
SET ANSI_NULLS ON                                 -- Deprecated: leave set ON. 
SET ANSI_PADDING ON                               -- Deprecated: leave set ON. 
SET ANSI_WARNINGS ON                              -- No trailing blanks saved. 
SET ARITHABORT ON                                 -- Math failure not ignored. 
SET CONCAT_NULL_YIELDS_NULL ON                    -- NULL plus string is NULL. 
SET NUMERIC_ROUNDABORT OFF                        -- Allows loss of precision. 
SET QUOTED_IDENTIFIER ON                          -- Allows reserved keywords. 

-- Standard settings. 
SET NOCOUNT ON                                    -- Minimize network traffic. 
SET ROWCOUNT 0                                    -- Reset in case it got set. 
SET XACT_ABORT ON                                 -- Make transactions behave. 

IF DB_NAME() IN ('master', 'tempdb', 'model', 'msdb') RAISERROR('   $$$ YOU ARE ATTACHED TO A SYSTEM DB $$$', 20, 1) WITH NOWAIT, LOG


----------------------------------------------------------------------------- 
-- The Service Broker requires the ability to apply a database lock when it 
-- starts up. Must be using the master database and refer to the target db 
-- in three-part names. 
-- 
-- See http://www.sql-server-performance.com/articles/dba/migrating_databases_checklist_part3_p2.aspx 
-- for why we have to check that there are no other databases have the same 
-- service broker GUID with the service broker enabled. 
----------------------------------------------------------------------------- 

USE master

BEGIN TRY

	-- Trying to enable service broker for a database in an instance where another 
	-- database already exists with the same service broker GUID and is broker-enabled 
	-- will fail. Can be more than one enabled. 
	
	-- Get 'our' guid. 
	DECLARE @guidConsolidatedDB UNIQUEIDENTIFIER
	
	SELECT @guidConsolidatedDB = d.service_broker_guid
	  FROM sys.databases d
	 WHERE name = 'ConsolidatedDB'
	
	-- See if any *other* dbs with same guid are enabled, give us a new guid. 
	IF @guidConsolidatedDB IN (SELECT d.service_broker_guid
										  FROM sys.databases d
										 WHERE d.is_broker_enabled = 1
										   AND name != 'ConsolidatedDB')
	BEGIN
		RAISERROR('Another database with the same service_broker_guid as ''ConsolidatedDB'' is enabled', 10, 1) WITH LOG, NOWAIT

		RAISERROR('Entering SINGLE_USER mode', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE

		RAISERROR('Getting a new service_broker_guid for ''ConsolidatedDB''', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET NEW_BROKER

		RAISERROR('Exiting SINGLE_USER mode', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET MULTI_USER

		RETURN
	END
	
	-- If service broker already enabled for us, no point in re-starting. 
	IF EXISTS (SELECT *
					 FROM sys.databases
					WHERE name = 'ConsolidatedDB'
					  AND is_broker_enabled = 1)
	BEGIN
		RAISERROR('Service Broker is already enabled on ''ConsolidatedDB''', 10, 1) WITH LOG, NOWAIT
		RETURN
	END

	-- Only other possibility is that service broker is not already enabled for us. 
	IF EXISTS (SELECT *
					 FROM sys.databases
					WHERE name = 'ConsolidatedDB'
					  AND is_broker_enabled = 0)
	BEGIN
		RAISERROR('Enabling Service Broker on ''ConsolidatedDB''', 10, 1) WITH LOG, NOWAIT

		RAISERROR('Entering SINGLE_USER mode', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE

		RAISERROR('Enabling Service Broker', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET ENABLE_BROKER

		RAISERROR('Exiting SINGLE_USER mode', 10, 1) WITH LOG, NOWAIT
		ALTER DATABASE ConsolidatedDB SET MULTI_USER
		
		RETURN
	END
END TRY
BEGIN CATCH
	ALTER DATABASE ConsolidatedDB SET MULTI_USER
	IF XACT_STATE() = -1  BEGIN  RAISERROR('Rolling back uncommittable transaction', 10, 1) WITH LOG  ROLLBACK TRANSACTION  END
	IF @@TRANCOUNT  =  1  BEGIN  RAISERROR('Rolling back non-nested transaction',    10, 1) WITH LOG  ROLLBACK TRANSACTION  END
	EXEC ConsolidatedDB.dbo.usp_ErrorHandler
	RETURN
END CATCH


                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         