USE master
GO
/****** Object:  StoredProcedure [dbo].[usp_Migration_ClearExistingConnections]    Script Date: 03/09/2010 10:50:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'dbo.usp_Migration_ClearExistingConnections') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[usp_Migration_ClearExistingConnections]
GO

/*=============================================
 Author:		George Peters
 Create date:	5/29/2013
 Description:	Drop any existing database 
				connections on the database.	
 =============================================*/
/*

EXEC Migration_ClearExistingConnections

*/

CREATE PROCEDURE [dbo].[usp_Migration_ClearExistingConnections]
(
    @DatabaseName nvarchar(100)
    ,@RemoveOnExitFlag int = 0
 ) 
AS 

BEGIN 
    DECLARE @spid int 
			,@cnt int
			,@sql nvarchar(512)
			,@msg nvarchar(512)
 
    SELECT @spid = MIN(spid), @cnt = COUNT(*) 
        FROM master..sysprocesses 
        WHERE dbid = DB_ID(@DatabaseName) 
        AND spid != @@SPID 
     
    WHILE @spid IS NOT NULL 
    BEGIN
		SET @msg = 'About to KILL ' + RTRIM(@spid);
        RAISERROR (@msg, 0, 1) WITH NOWAIT
 
        SET @sql = 'KILL '+RTRIM(@spid) 
        EXEC(@sql) 

		-- Give the system at least 1 second for the process to clear out.
		WAITFOR DELAY '00:00:01'

        WHILE EXISTS (SELECT 1 FROM master..sysprocesses WHERE dbid = DB_ID(@DatabaseName) AND spid = @spid AND spid != @@SPID)
		BEGIN
			SET @msg = 'Waiting on ' + RTRIM(@spid);
			RAISERROR (@msg, 0, 1) WITH NOWAIT
			WAITFOR DELAY '00:00:02'
		END

		SET @msg = 'Terminated ' + RTRIM(@spid);
        RAISERROR (@msg, 0, 1) WITH NOWAIT

        SELECT @spid = MIN(spid), @cnt = COUNT(*) 
            FROM master..sysprocesses 
            WHERE dbid = DB_ID(@DatabaseName)
            AND spid != @@SPID
    END
   
	IF ((@DatabaseName = 'ConsolidatedDB') AND (@RemoveOnExitFlag = 1))
	BEGIN
		IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'dbo.usp_Migration_ClearExistingConnections') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
		DROP PROCEDURE [dbo].[usp_Migration_ClearExistingConnections]
	END 

END 
GO
