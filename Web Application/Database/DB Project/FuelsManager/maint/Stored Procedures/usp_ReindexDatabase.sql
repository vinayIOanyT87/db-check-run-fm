/***********************************************************************************

-- Use this reindex only in the inhouse /production mainteinance window
Running the procedure may cause timeouts during high activity.

Windows scheduler cmd example:
sqlcmd -b -E -S MyServer -d MyDatabase -Q "exec [dbo].[usp_ReindexDB]"
sqlcmd -b -E -S D-CPL59J3\MSSQL2022 -d FuelsmanagerDB_Ent -Q "exec [dbo].[usp_ReindexDB]"

************************************************************************************/
CREATE PROCEDURE maint.usp_ReindexDatabases
AS

DECLARE @MyTable VARCHAR(255)
DECLARE myCursor CURSOR
FOR
SELECT i.TABLE_SCHEMA + '.' + i.table_name
FROM information_schema.tables i
WHERE i.table_type = 'base table'

OPEN myCursor

FETCH NEXT
FROM myCursor
INTO @MyTable

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Reindexing Table:  ' + @MyTable

    DBCC DBREINDEX (
            @MyTable
            , ''
           , 100
            )

    FETCH NEXT
    FROM myCursor
    INTO @MyTable
END

CLOSE myCursor

DEALLOCATE myCursor

EXEC sp_updatestats
GO


