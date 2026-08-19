
/* {CheckPoint: CREATING STORED PROCEDURE: [sync].[usp_SyncAnchorGenerateSentAnchor2ByTableName]] } */
CREATE PROCEDURE [sync].[usp_SyncAnchorGenerateSentAnchor2ByTableName] (
		@__server_id_binary binary
		,@sync_current_table nvarchar(256)
		,@sync_anchor bigint output)
AS
BEGIN
	IF @__server_id_binary IS NULL
	BEGIN
		RAISERROR('Invalid parameter value provided: server id cannot be null.', 10, 1) WITH NOWAIT
		RETURN
	END
	
	IF @sync_current_table IS NULL
	BEGIN
		RAISERROR('Could not locate anchor record: table name "%s" not found.', 10, 1, @sync_current_table) WITH NOWAIT
		RETURN
	END
	
	DECLARE @SQL nvarchar(max)
	DECLARE @SQLParams nvarchar(max)
	
	SET @SQL = N'SELECT @anchor = max(ct.InsertedRowVersion) FROM [track].[' + PARSENAME(@sync_current_table, 1) + '] ct WITH (NOLOCK) WHERE ct.InsertedContext IS NOT NULL AND ct.InsertedContext = @serverId'

	SET @SQLParams = N'@serverId binary, @anchor bigint output';
	
	EXEC sp_executesql @SQL,
						@SQLParams,
						@serverId = @__server_id_binary,
						@anchor = @sync_anchor OUTPUT 

END