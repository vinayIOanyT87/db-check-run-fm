-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblListViewFields
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblListViewFields]
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
@sync_batch_size_tblListViewFields int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblListViewFields int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblListViewFields].[ColumnOrder],[dbo].[tblListViewFields].[CreatedDate],[dbo].[tblListViewFields].[CreatedBy],[dbo].[tblListViewFields].[UpdatedDate],[dbo].[tblListViewFields].[UpdatedBy],[dbo].[tblListViewFields].[ListViewID],[dbo].[tblListViewFields].[ListViewFieldGuid],[dbo].[tblListViewFields].[LookupListViewFieldTypeIndex],[dbo].[tblListViewFields].[LookupStandardFieldTypeIndex],[dbo].[tblListViewFields].[ListViewGuid],[dbo].[tblListViewFields].[TransactionAliasGuid],[dbo].[tblListViewFields].[TransactionAliasFieldGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblListViewFields].[LedgerAggregateColumnGuid], [dbo].[tblListViewFields].[_RowVersion]
            FROM [dbo].[tblListViewFields]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblListViewFields IS NULL OR 
        (@sync_batch_size_tblListViewFields IS NOT NULL AND @sync_batch_size_tblListViewFields = 0))
    BEGIN
        SET @sync_batch_size_tblListViewFields = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblListViewFields) WITH TIES [ColumnOrder],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ListViewID],[ListViewFieldGuid],[LookupListViewFieldTypeIndex],[LookupStandardFieldTypeIndex],[ListViewGuid],[TransactionAliasGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[LedgerAggregateColumnGuid],_RowVersion
        FROM (
            SELECT [ColumnOrder],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ListViewID],[ListViewFieldGuid],[LookupListViewFieldTypeIndex],[LookupStandardFieldTypeIndex],[ListViewGuid],[TransactionAliasGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[LedgerAggregateColumnGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblListViewFields) WITH TIES [dbo].[tblListViewFields].[ColumnOrder],[dbo].[tblListViewFields].[CreatedDate],[dbo].[tblListViewFields].[CreatedBy],[dbo].[tblListViewFields].[UpdatedDate],[dbo].[tblListViewFields].[UpdatedBy],[dbo].[tblListViewFields].[ListViewID],[dbo].[tblListViewFields].[ListViewFieldGuid],[dbo].[tblListViewFields].[LookupListViewFieldTypeIndex],[dbo].[tblListViewFields].[LookupStandardFieldTypeIndex],[dbo].[tblListViewFields].[ListViewGuid],[dbo].[tblListViewFields].[TransactionAliasGuid],[dbo].[tblListViewFields].[TransactionAliasFieldGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblListViewFields].[LedgerAggregateColumnGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblListViewFields]
                        INNER JOIN (SELECT [ListViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedListViewListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblListViewFields].[ListViewGuid] = data.[ListViewGuid]
                        INNER JOIN [track].[tblListViewFields] CT
                            ON CT.PK_ListViewFieldGuid = [dbo].[tblListViewFields].[ListViewFieldGuid] 
                        INNER JOIN [track].[tblEntityListViewToSite] MAPCT
                            ON MAPCT.PK_ListViewToSiteGuid = data.[ListViewToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [ColumnOrder],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ListViewID],[ListViewFieldGuid],[LookupListViewFieldTypeIndex],[LookupStandardFieldTypeIndex],[ListViewGuid],[TransactionAliasGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[LedgerAggregateColumnGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblListViewFields) WITH TIES [dbo].[tblListViewFields].[ColumnOrder],[dbo].[tblListViewFields].[CreatedDate],[dbo].[tblListViewFields].[CreatedBy],[dbo].[tblListViewFields].[UpdatedDate],[dbo].[tblListViewFields].[UpdatedBy],[dbo].[tblListViewFields].[ListViewID],[dbo].[tblListViewFields].[ListViewFieldGuid],[dbo].[tblListViewFields].[LookupListViewFieldTypeIndex],[dbo].[tblListViewFields].[LookupStandardFieldTypeIndex],[dbo].[tblListViewFields].[ListViewGuid],[dbo].[tblListViewFields].[TransactionAliasGuid],[dbo].[tblListViewFields].[TransactionAliasFieldGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblListViewFields].[LedgerAggregateColumnGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblListViewFields]
                        INNER JOIN (SELECT [LedgerViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedLedgerViewListForSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblListViewFields].[ListViewGuid] = data1.[ListViewGuid]
                        INNER JOIN [track].[tblListViewFields] CT
                            ON CT.PK_ListViewFieldGuid = [dbo].[tblListViewFields].[ListViewFieldGuid] 
                        INNER JOIN [track].[tblEntityLedgerViewToSite] MAPCT
                            ON MAPCT.PK_LedgerViewToSiteGuid = data1.[LedgerViewToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs2  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
