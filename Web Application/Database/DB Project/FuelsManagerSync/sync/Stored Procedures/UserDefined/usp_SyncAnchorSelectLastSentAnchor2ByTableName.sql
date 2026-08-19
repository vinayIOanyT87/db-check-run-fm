
/* {CheckPoint: CREATING STORED PROCEDURE: [sync].[usp_SyncAnchorSelectLastSentAnchor2ByTableName]] } */
CREATE PROCEDURE [sync].[usp_SyncAnchorSelectLastSentAnchor2ByTableName] (
		@sync_context_site_id nvarchar(30)
		,@sync_current_table nvarchar(256)
		,@sync_table_sent_anchor bigint output)
AS
BEGIN
	IF (@sync_context_site_id IS NULL)
	BEGIN
		SET @sync_context_site_id = '';
	END

	-- Note, Don't try to combine this stored procedure with usp_SyncAnchorSelectLastSentAnchorByTableName.  These are used by the SyncFramework and utilize a known parameter list
	-- so it's difficult to add a third parameter to tell this stored procedure if it should return anchor1 or anchor2.
	IF @sync_current_table IS NULL
	BEGIN
		RAISERROR('Could not locate anchor record: table name "%s" not found.', 10, 1, @sync_current_table) WITH NOWAIT
		RETURN
	END

	SELECT @sync_table_sent_anchor = sa.LastSentAnchor2
		FROM [sync].[tblSyncAnchor] sa WITH (NOLOCK)
			WHERE sa.TableName = @sync_current_table
					AND (@sync_context_site_id IS NOT NULL AND sa.SiteID = @sync_context_site_id)

	IF (@sync_table_sent_anchor IS NULL)
		SET @sync_table_sent_anchor = 1
END
