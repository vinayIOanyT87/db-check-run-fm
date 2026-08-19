

/* {CheckPoint: CREATING STORED PROCEDURE: [sync].[usp_SyncAnchorUpdateLastSentAnchorByTableName]] } */
CREATE PROCEDURE [sync].[usp_SyncAnchorUpdateLastSentAnchorByTableName] (
		@sync_context_site_id nvarchar(30)
		,@sync_current_table nvarchar(256)
		,@sync_anchor bigint output)
AS
BEGIN
	IF (@sync_context_site_id IS NULL)
	BEGIN
		SET @sync_context_site_id = '';
	END

	-- Note, Don't try to combine this stored procedure with usp_SyncAnchorUpdateLastSentAnchor2ByTableName.  
	-- These are used by the SyncFramework and utilize a known parameter list so it's difficult to add a third parameter 
	-- to tell this stored procedure if it should return anchor1 or anchor2.
	IF @sync_current_table IS NULL
	BEGIN
		RAISERROR('Could not locate anchor record: table name "%s" not found.', 10, 1, @sync_current_table) WITH NOWAIT
		RETURN;
	END;

	MERGE [sync].[tblSyncAnchor] AS target
	USING (SELECT sa.SiteID, sa.TableName, sa.LastSentAnchor1
			FROM [sync].[tblSyncAnchor] sa WITH (NOLOCK)
			WHERE sa.TableName = @sync_current_table
					AND (@sync_context_site_id IS NOT NULL AND sa.SiteID = @sync_context_site_id)
			) AS source (SiteID, TableName, LastSentAnchor1)
	ON (target.SiteID = source.SiteID 
		AND target.TableName = source.TableName)
	WHEN MATCHED 
		THEN UPDATE SET LastSentAnchor1 = @sync_anchor;
END
