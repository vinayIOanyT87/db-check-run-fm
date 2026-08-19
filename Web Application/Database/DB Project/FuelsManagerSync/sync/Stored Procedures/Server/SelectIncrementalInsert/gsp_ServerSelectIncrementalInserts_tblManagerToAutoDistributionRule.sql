-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblManagerToAutoDistributionRule
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblManagerToAutoDistributionRule]
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
@sync_batch_size_tblManagerToAutoDistributionRule int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblManagerToAutoDistributionRule int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblManagerToAutoDistributionRule IS NOT NULL AND @sync_first_time_sync_option_tblManagerToAutoDistributionRule = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [map].[tblManagerToAutoDistributionRule].[ManagerToAutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[ManagerGuid],[map].[tblManagerToAutoDistributionRule].[CreatedDate],[map].[tblManagerToAutoDistributionRule].[CreatedBy],[map].[tblManagerToAutoDistributionRule].[UpdatedDate],[map].[tblManagerToAutoDistributionRule].[UpdatedBy], [map].[tblManagerToAutoDistributionRule].[_RowVersion]
            FROM [map].[tblManagerToAutoDistributionRule]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblManagerToAutoDistributionRule IS NULL OR 
        (@sync_batch_size_tblManagerToAutoDistributionRule IS NOT NULL AND @sync_batch_size_tblManagerToAutoDistributionRule = 0))
    BEGIN
        SET @sync_batch_size_tblManagerToAutoDistributionRule = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblManagerToAutoDistributionRule) WITH TIES [ManagerToAutoDistributionRuleGuid],[AutoDistributionRuleGuid],[ManagerGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblManagerToAutoDistributionRule) WITH TIES [map].[tblManagerToAutoDistributionRule].[ManagerToAutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[ManagerGuid],[map].[tblManagerToAutoDistributionRule].[CreatedDate],[map].[tblManagerToAutoDistributionRule].[CreatedBy],[map].[tblManagerToAutoDistributionRule].[UpdatedDate],[map].[tblManagerToAutoDistributionRule].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,MAPCT2.InsertedRowVersion) AS '_RowVersion'
                FROM [map].[tblManagerToAutoDistributionRule]
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data
                        ON [map].[tblManagerToAutoDistributionRule].[ManagerGuid] = data.[CompanyGuid]
                    INNER JOIN (SELECT [AutoDistributionRuleToSiteGuid],[AutoDistributionRuleGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedAutoDistributionRuleListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblManagerToAutoDistributionRule].[AutoDistributionRuleGuid] = data1.[AutoDistributionRuleGuid]
                    INNER JOIN [track].[tblManagerToAutoDistributionRule] CT
                        ON CT.PK_ManagerToAutoDistributionRuleGuid = [map].[tblManagerToAutoDistributionRule].[ManagerToAutoDistributionRuleGuid] 
                    INNER JOIN [track].[tblEntityCompanyToSite] MAPCT
                        ON MAPCT.PK_CompanyToSiteGuid = data.[CompanyToSiteGuid]
                    INNER JOIN [track].[tblEntityAutoDistributionRuleToSite] MAPCT2
                        ON MAPCT2.PK_AutoDistributionRuleToSiteGuid = data1.[AutoDistributionRuleToSiteGuid]
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT2.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT2.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT2.InsertedContext IS NULL OR MAPCT2.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1   -- DetectedSubFunctions: True / IncludeEntityAssignments: True
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
