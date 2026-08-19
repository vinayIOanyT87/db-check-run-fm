-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldTransactionAlias
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblUserDataFieldTransactionAlias]
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
@sync_batch_size_tblUserDataFieldTransactionAlias int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblUserDataFieldTransactionAlias int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblUserDataFieldTransactionAlias IS NOT NULL AND @sync_first_time_sync_option_tblUserDataFieldTransactionAlias = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[Number],[dbo].[tblUserDataFieldTransactionAlias].[DisplayOrder],[dbo].[tblUserDataFieldTransactionAlias].[DisplayName],[dbo].[tblUserDataFieldTransactionAlias].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldTransactionAlias].[Required],[dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid],[dbo].[tblUserDataFieldTransactionAlias].[CreatedDate],[dbo].[tblUserDataFieldTransactionAlias].[CreatedBy],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedDate],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedBy],[dbo].[tblUserDataFieldTransactionAlias].[DispatchField],[dbo].[tblUserDataFieldTransactionAlias].[ClearOnNew],[dbo].[tblUserDataFieldTransactionAlias].[ReadOnly],[dbo].[tblUserDataFieldTransactionAlias].[Visibility],[dbo].[tblUserDataFieldTransactionAlias].[DefaultValue], [dbo].[tblUserDataFieldTransactionAlias].[_RowVersion]
            FROM [dbo].[tblUserDataFieldTransactionAlias]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblUserDataFieldTransactionAlias IS NULL OR 
        (@sync_batch_size_tblUserDataFieldTransactionAlias IS NOT NULL AND @sync_batch_size_tblUserDataFieldTransactionAlias = 0))
    BEGIN
        SET @sync_batch_size_tblUserDataFieldTransactionAlias = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblUserDataFieldTransactionAlias) WITH TIES [UserDataFieldTransactionAliasGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
    FROM (
        SELECT [UserDataFieldTransactionAliasGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblUserDataFieldTransactionAlias) WITH TIES [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[Number],[dbo].[tblUserDataFieldTransactionAlias].[DisplayOrder],[dbo].[tblUserDataFieldTransactionAlias].[DisplayName],[dbo].[tblUserDataFieldTransactionAlias].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldTransactionAlias].[Required],[dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid],[dbo].[tblUserDataFieldTransactionAlias].[CreatedDate],[dbo].[tblUserDataFieldTransactionAlias].[CreatedBy],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedDate],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedBy],[dbo].[tblUserDataFieldTransactionAlias].[DispatchField],[dbo].[tblUserDataFieldTransactionAlias].[ClearOnNew],[dbo].[tblUserDataFieldTransactionAlias].[ReadOnly],[dbo].[tblUserDataFieldTransactionAlias].[Visibility],[dbo].[tblUserDataFieldTransactionAlias].[DefaultValue],CT.InsertedRowVersion AS '_RowVersion'
                FROM [dbo].[tblUserDataFieldTransactionAlias]
                    INNER JOIN [track].[tblUserDataFieldTransactionAlias] CT
                        ON CT.PK_UserDataFieldTransactionAliasGuid = [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid] 
                WHERE ( [dbo].[tblUserDataFieldTransactionAlias].[SiteGuid] = @sync_context_site_guid )
                        AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs
        UNION
        SELECT [UserDataFieldTransactionAliasGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblUserDataFieldTransactionAlias) WITH TIES [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[Number],[dbo].[tblUserDataFieldTransactionAlias].[DisplayOrder],[dbo].[tblUserDataFieldTransactionAlias].[DisplayName],[dbo].[tblUserDataFieldTransactionAlias].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldTransactionAlias].[Required],[dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid],[dbo].[tblUserDataFieldTransactionAlias].[CreatedDate],[dbo].[tblUserDataFieldTransactionAlias].[CreatedBy],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedDate],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedBy],[dbo].[tblUserDataFieldTransactionAlias].[DispatchField],[dbo].[tblUserDataFieldTransactionAlias].[ClearOnNew],[dbo].[tblUserDataFieldTransactionAlias].[ReadOnly],[dbo].[tblUserDataFieldTransactionAlias].[Visibility],[dbo].[tblUserDataFieldTransactionAlias].[DefaultValue],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblUserDataFieldTransactionAlias]
                    INNER JOIN (SELECT [UserDataFieldTransactionAliasGuid],[TransactionAliasToSiteGuid],[UserGroupToSiteGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssignedUserDataFieldTransactionAliasListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid] = data.[UserDataFieldTransactionAliasGuid]
                    INNER JOIN [track].[tblUserDataFieldTransactionAlias] CT
                        ON CT.PK_UserDataFieldTransactionAliasGuid = [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid] 
                    INNER JOIN [track].[tblEntityTransactionAliasToSite] MAPCT
                        ON MAPCT.PK_TransactionAliasToSiteGuid = data.[TransactionAliasToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
