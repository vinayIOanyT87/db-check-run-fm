USE master
GO
/****** Object:  StoredProcedure [dbo].[Migration_ClearDBUsers]    Script Date: 03/09/2010 10:50:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ClearDBUsers') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ClearDBUsers
GO

/*=============================================
 Author:			Sijuan Jiang
 Create date:		3/9/2010
 Description:		Drop all 6.0 Databases
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_ClearDBUsers

*/

CREATE PROCEDURE [dbo].Migration_ClearDBUsers	
(
    @dbName varchar(50),
    @bDropProcedure int = 0
 ) 
AS 

BEGIN 
    DECLARE @spid INT, 
        @cnt INT, 
        @sql VARCHAR(255) 
 
    SELECT @spid = MIN(spid), @cnt = COUNT(*) 
        FROM master..sysprocesses 
        WHERE dbid = DB_ID(@dbname) 
        AND spid != @@SPID 
     
    /*WHILE @spid IS NOT NULL 
    BEGIN */
        PRINT 'About to KILL '+RTRIM(@spid) 
 
        SET @sql = 'KILL '+RTRIM(@spid) 
        EXEC(@sql) 
 
        SELECT @spid = MIN(spid), @cnt = COUNT(*) 
            FROM master..sysprocesses 
            WHERE dbid = DB_ID(@dbname) 
            AND spid != @@SPID
    /*END  */   
   
	IF ((@DBName = 'ConsolidatedDB') and (@bDropProcedure = 1))
	BEGIN
		If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ClearDBUsers') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		Drop Procedure dbo.Migration_ClearDBUsers
	END 

END 
GO
