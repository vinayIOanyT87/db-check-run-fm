-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldFuelCard
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblUserDataFieldFuelCard]
@sync_initialized bit,
@sync_last_received_anchor bigint,
@sync_new_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_server_id_binary binary(16),
@sync_context_site_guid uniqueidentifier,
@sync_context_site_id nvarchar(30),
@sync_context_site_guid_list nvarchar(1024),
@sync_context_site_id_list nvarchar(1024),
@sync_table_name nvarchar(512),
@sync_batch_size_tblUserDataFieldFuelCard int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- During an initial synchronization, we don't want to bring back any updates since we 
    -- should be picking them up with the select incremental inserts 
    --
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid],[dbo].[tblUserDataFieldFuelCard].[Number],[dbo].[tblUserDataFieldFuelCard].[DisplayOrder],[dbo].[tblUserDataFieldFuelCard].[DisplayName],[dbo].[tblUserDataFieldFuelCard].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldFuelCard].[Required],[dbo].[tblUserDataFieldFuelCard].[UserGroupGuid],[dbo].[tblUserDataFieldFuelCard].[CreatedDate],[dbo].[tblUserDataFieldFuelCard].[CreatedBy],[dbo].[tblUserDataFieldFuelCard].[UpdatedDate],[dbo].[tblUserDataFieldFuelCard].[UpdatedBy],[dbo].[tblUserDataFieldFuelCard].[DispatchField],[dbo].[tblUserDataFieldFuelCard].[ClearOnNew],[dbo].[tblUserDataFieldFuelCard].[ReadOnly],[dbo].[tblUserDataFieldFuelCard].[Visibility],[dbo].[tblUserDataFieldFuelCard].[DefaultValue], [dbo].[tblUserDataFieldFuelCard].[_RowVersion]
            FROM [dbo].[tblUserDataFieldFuelCard]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblUserDataFieldFuelCard IS NULL OR 
        (@sync_batch_size_tblUserDataFieldFuelCard IS NOT NULL AND @sync_batch_size_tblUserDataFieldFuelCard = 0))
    BEGIN
        SET @sync_batch_size_tblUserDataFieldFuelCard = 2147483647;
    END

        SELECT TOP(@sync_batch_size_tblUserDataFieldFuelCard) WITH TIES [UserDataFieldFuelCardGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
        FROM (
            SELECT [UserDataFieldFuelCardGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblUserDataFieldFuelCard) WITH TIES [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid],[dbo].[tblUserDataFieldFuelCard].[Number],[dbo].[tblUserDataFieldFuelCard].[DisplayOrder],[dbo].[tblUserDataFieldFuelCard].[DisplayName],[dbo].[tblUserDataFieldFuelCard].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldFuelCard].[Required],[dbo].[tblUserDataFieldFuelCard].[UserGroupGuid],[dbo].[tblUserDataFieldFuelCard].[CreatedDate],[dbo].[tblUserDataFieldFuelCard].[CreatedBy],[dbo].[tblUserDataFieldFuelCard].[UpdatedDate],[dbo].[tblUserDataFieldFuelCard].[UpdatedBy],[dbo].[tblUserDataFieldFuelCard].[DispatchField],[dbo].[tblUserDataFieldFuelCard].[ClearOnNew],[dbo].[tblUserDataFieldFuelCard].[ReadOnly],[dbo].[tblUserDataFieldFuelCard].[Visibility],[dbo].[tblUserDataFieldFuelCard].[DefaultValue],CT.UpdatedRowVersion AS '_RowVersion'
                    FROM [dbo].[tblUserDataFieldFuelCard]
                        INNER JOIN [track].[tblUserDataFieldFuelCard] CT
                            ON CT.PK_UserDataFieldFuelCardGuid = [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid] 
                    WHERE ( [dbo].[tblUserDataFieldFuelCard].[SiteGuid] = @sync_context_site_guid )
                            AND ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    ORDER BY [_RowVersion] ASC
                ) rs
            UNION
            SELECT [UserDataFieldFuelCardGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblUserDataFieldFuelCard) WITH TIES [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid],[dbo].[tblUserDataFieldFuelCard].[Number],[dbo].[tblUserDataFieldFuelCard].[DisplayOrder],[dbo].[tblUserDataFieldFuelCard].[DisplayName],[dbo].[tblUserDataFieldFuelCard].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldFuelCard].[Required],[dbo].[tblUserDataFieldFuelCard].[UserGroupGuid],[dbo].[tblUserDataFieldFuelCard].[CreatedDate],[dbo].[tblUserDataFieldFuelCard].[CreatedBy],[dbo].[tblUserDataFieldFuelCard].[UpdatedDate],[dbo].[tblUserDataFieldFuelCard].[UpdatedBy],[dbo].[tblUserDataFieldFuelCard].[DispatchField],[dbo].[tblUserDataFieldFuelCard].[ClearOnNew],[dbo].[tblUserDataFieldFuelCard].[ReadOnly],[dbo].[tblUserDataFieldFuelCard].[Visibility],[dbo].[tblUserDataFieldFuelCard].[DefaultValue],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblUserDataFieldFuelCard]
                        INNER JOIN (SELECT [UserDataToSiteGuid],[UserDataFieldFuelCardGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedUserDataFieldFuelCardListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid] = data.[UserDataFieldFuelCardGuid]
                        INNER JOIN [track].[tblUserDataFieldFuelCard] CT
                            ON CT.PK_UserDataFieldFuelCardGuid = [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid] 
                        INNER JOIN [track].[tblEntityUserDataToSite] MAPCT
                            ON MAPCT.PK_UserDataToSiteGuid = data.[UserDataToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs1
        ) mainRs
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
