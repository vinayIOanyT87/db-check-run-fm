CREATE PROCEDURE [sync].[usp_SyncReindexAfterInitialSync]
(@IdentityGuid uniqueidentifier = NULL, @ID nvarchar(80) =  NULL)
AS
BEGIN
	DECLARE @SchemaName varchar(255)
	DECLARE @TableName varchar(255)
	DECLARE @Sql nvarchar(MAX)

	DECLARE TableCursor CURSOR FOR
	select  PARSENAME(st.TableName, 2) 'SchemaName', PARSENAME(st.TableName, 1) 'TableName'
	FROM sync.tblSyncProfile as sp  WITH (NOLOCK)
	INNER JOIN sync.tblSyncScope as sc WITH (NOLOCK)
		on sp.SyncProfileGuid = sc.SyncProfileGuid
	INNER JOIN sync.tblSyncTableToScopeMap as stsm WITH (NOLOCK)
		on sc.SyncScopeGuid = stsm.SyncScopeGuid
	INNER JOIN sync.tblSyncTable st WITH (NOLOCK)
		ON stsm.SyncTableGuid = st.SyncTableGuid
	WHERE (@IdentityGuid IS NULL OR sp.SyncProfileGuid = @IdentityGuid)
	AND (@ID IS NULL OR sp.ID = @ID)
	ORDER BY sc.SyncOrder, stsm.SyncOrder

	OPEN TableCursor
	FETCH NEXT FROM TableCursor INTO @SchemaName, @TableName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @FullTable varchar(255)
		SET @FullTable = @SchemaName + '.' + @TableName

		--DBCC DBREINDEX(@FullTable,' ',90)
		SET @Sql = 'ALTER INDEX ALL ON ' + @FullTable + ' REBUILD'
		EXEC sp_executesql @Statement=@Sql

		FETCH NEXT FROM TableCursor INTO @SchemaName, @TableName
	END

	CLOSE TableCursor
	DEALLOCATE TableCursor
	
END