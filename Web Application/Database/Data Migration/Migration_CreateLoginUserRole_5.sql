USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_CreateLoginUserRole_5]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_CreateLoginUserRole_5') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_CreateLoginUserRole_5
GO

CREATE PROCEDURE dbo.Migration_CreateLoginUserRole_5
 /*=============================================
 Author:			URVI PATEL
 Create date:		1/27/2010
 Description:		Create User Roles and Logins
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_CreateLoginUserRole_5 2, null

*/
(

@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID varchar(max) = NULL

)

AS 

DECLARE @Users VARCHAR(250) 
DECLARE @LoginExists varchar(2000)
DECLARE @CreateLogin  varchar(2000)
DECLARE @sqlCommand1  varchar(2000)
DECLARE @sqlCommand2  varchar(2000)

DECLARE db_cursor CURSOR FOR  
SELECT DISTINCT Userid 
FROM [ConsolidatedDB].dbo.tblUsers where UserID not in ('Administrator','BSMEAdmin')
OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @Users   

WHILE @@FETCH_STATUS = 0   
BEGIN   

SET @LoginExists = 'IF EXISTS (SELECT * FROM sys.server_principals WHERE name = '''+ @Users + ''''
+ ' and type_desc = ''SQL_LOGIN'')'+ char(13)+ 'BEGIN'+ char(13)+ 'drop login ['+ @Users +']'+ char(13) + 'END'

SET @CreateLogin = 'CREATE LOGIN ['+ @Users + '] WITH PASSWORD = ''1c9BDB9cD3a$cceDaaD*0c7*c9F1113393eB0*91'', DEFAULT_DATABASE = ConsolidatedDB'

SET @sqlCommand1  = 'USE MASTER' +  char(13) + 'IF EXISTS (select * from sys.database_principals  WHERE name = '''+ @Users + ''''
+ ' and type_desc = ''SQL_USER'')'+ char(13)+ 'DROP USER ['+ @Users +']'+ char(13)+ 'CREATE USER ['+ @Users +']'+ char(13)+
'EXEC sp_addrolemember ''FMDUserRole''' +','''+ @Users + ''''

SET @sqlCommand2  = 'USE ConsolidatedDB' +  char(13) + 'IF EXISTS (select * from sys.database_principals  WHERE name = '''+ @Users + ''''
+ ' and type_desc = ''SQL_USER'')'+ char(13)+ 'DROP USER ['+ @Users +']'+ char(13)+ 'CREATE USER ['+ @Users +']'+ char(13)+
'EXEC sp_addrolemember ''FMDUserRole''' +','''+ @Users + ''''


FETCH NEXT FROM db_cursor INTO @Users   
        EXEC (@LoginExists)
        EXEC (@CreateLogin)
        EXEC (@sqlCommand1)
        EXEC (@sqlCommand2)
        --SELECT @SQLCommand
      
END   

CLOSE db_cursor   
DEALLOCATE db_cursor 

/* Cursor for Admin Role */ 

DECLARE @AllUsers VARCHAR(2000) 
DECLARE @AddSvrRole VARCHAR(2000) 
DECLARE @FMDAdminRoleMaster VARCHAR(2000) 
DECLARE @FMDAdminRoleConsolidatedDB VARCHAR(2000) 

DECLARE AddRole_cursor CURSOR FOR  
SELECT distinct u.UserID
FROM [ConsolidatedDB].dbo.tblUserGroupMap  gm
INNER JOIN [ConsolidatedDB].dbo.tblGroups g
ON gm.GroupIndex = g.GroupIndex
INNER JOIN [ConsolidatedDB].dbo.tblUsers u
ON u.userindex = gm.userindex
WHERE GroupID = 'Administrator'

OPEN AddRole_cursor   
FETCH NEXT FROM AddRole_cursor INTO @AllUsers   

WHILE @@FETCH_STATUS = 0   
BEGIN   
SET @AddSvrRole = 'EXEC sp_addsrvrolemember '''+  @AllUsers +''','''+ 'securityadmin'''
SET @FMDAdminRoleMaster =  'USE MASTER' +  char(13) + 'EXEC sp_addrolemember ''FMDAdminRole''' +','''+ @AllUsers + ''''
SET @FMDAdminRoleConsolidatedDB =  'USE ConsolidatedDB' +  char(13) + 'EXEC sp_addrolemember ''FMDAdminRole''' +','''+ @AllUsers + '''' 
        FETCH NEXT FROM AddRole_cursor INTO @AllUsers   
		--SELECT @AddSvrRole
		--SELECT @FMDAdminRoleMaster
		--SELECT @FMDAdminRoleConsolidatedDB
        
        EXEC (@AddSvrRole)
        EXEC (@FMDAdminRoleMaster)
        EXEC (@FMDAdminRoleConsolidatedDB)
        
END   

CLOSE AddRole_cursor   
DEALLOCATE AddRole_cursor 


