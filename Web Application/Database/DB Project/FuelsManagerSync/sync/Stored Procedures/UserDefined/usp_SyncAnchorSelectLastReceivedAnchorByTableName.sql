

/* {CheckPoint: CREATING STORED PROCEDURE: [sync].[usp_SyncAnchorSelectLastReceivedAnchorByTableName] } */
CREATE PROCEDURE [sync].[usp_SyncAnchorSelectLastReceivedAnchorByTableName] (
		@sync_context_site_id nvarchar(30)
		,@sync_current_table nvarchar(256)
		,@sync_table_received_anchor bigint output)
AS
BEGIN
	IF (@sync_context_site_id IS NULL)
	BEGIN
		SET @sync_context_site_id = '';
	END

	IF @sync_current_table IS NULL
	BEGIN
		RAISERROR('Could not locate anchor record: table name "%s" not found.', 10, 1, @sync_current_table) WITH NOWAIT
		RETURN
	END
	
	IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncAnchor] sa WITH (NOLOCK)
								WHERE sa.TableName = @sync_current_table
										AND (@sync_context_site_id IS NOT NULL AND sa.SiteID = @sync_context_site_id)
)

	BEGIN
		INSERT INTO [sync].[tblSyncAnchor] (SiteID, 
											TableName,
											LastReceivedAnchor,
											LastSentAnchor1,
											LastSentAnchor2)
											VALUES (@sync_context_site_id,
													@sync_current_table,
													0,
													0,
													0);
	END
	
	SELECT @sync_table_received_anchor = sa.LastReceivedAnchor
		FROM [sync].[tblSyncAnchor] sa WITH (NOLOCK)
			WHERE sa.TableName = @sync_current_table
					AND (@sync_context_site_id IS NOT NULL AND sa.SiteID = @sync_context_site_id)
END
