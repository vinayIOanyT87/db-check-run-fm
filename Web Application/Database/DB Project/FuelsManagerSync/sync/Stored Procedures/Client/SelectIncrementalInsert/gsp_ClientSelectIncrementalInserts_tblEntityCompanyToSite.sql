-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityCompanyToSite
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblEntityCompanyToSite]
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
@sync_batch_size_tblEntityCompanyToSite int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblEntityCompanyToSite int
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
        SELECT [map].[tblEntityCompanyToSite].[CompanyToSiteGuid],[map].[tblEntityCompanyToSite].[CompanyGuid],[map].[tblEntityCompanyToSite].[SiteGuid],[map].[tblEntityCompanyToSite].[CreatedDate],[map].[tblEntityCompanyToSite].[CreatedBy],[map].[tblEntityCompanyToSite].[UpdatedDate],[map].[tblEntityCompanyToSite].[UpdatedBy],[map].[tblEntityCompanyToSite].[AssignedFromSiteGuid], [map].[tblEntityCompanyToSite].[_RowVersion]
            FROM [map].[tblEntityCompanyToSite]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblEntityCompanyToSite IS NULL OR 
        (@sync_batch_size_tblEntityCompanyToSite IS NOT NULL AND @sync_batch_size_tblEntityCompanyToSite = 0))
    BEGIN
        SET @sync_batch_size_tblEntityCompanyToSite = 2147483647;
    END

        -- This is a list of entity assignments that meet the "change tracking" filter criteria and should be synchronized.
        -- We need to use this list to identify the other "entity assignments" that do not belong to the site but that are
        -- part of the entity assignment tree starting from the owning site.  We need these intermediate entity assignments to sync
        -- in order for the record versioning queries to work correctly and display the assigned entities in the GUI.
        ; WITH EntityAssignmentTree_CTE AS 
        (
            SELECT [CompanyToSiteGuid],[TrackedCompanyToSiteGuid],[IncludeChangeTrackingFlag]
                FROM (SELECT [CompanyToSiteGuid],[TrackedCompanyToSiteGuid],[IncludeChangeTrackingFlag] FROM [dbo].[udf_GetAssignmentTreeForCompanyListForSite](@sync_context_site_guid)) data
        ), ChangedEntityToSiteRecordList_CTE AS 
        (
            SELECT data.[CompanyToSiteGuid],data.[TrackedCompanyToSiteGuid],data.[IncludeChangeTrackingFlag],CT.InsertedRowVersion AS 'InsertedRowVersion' 
                FROM [map].[tblEntityCompanyToSite]
                    INNER JOIN EntityAssignmentTree_CTE data
                        ON [map].[tblEntityCompanyToSite].[CompanyToSiteGuid] = data.[CompanyToSiteGuid]
                    INNER JOIN [track].[tblEntityCompanyToSite] CT
                        ON CT.PK_CompanyToSiteGuid = [map].[tblEntityCompanyToSite].[CompanyToSiteGuid] 
                WHERE data.[IncludeChangeTrackingFlag] = 1
                        AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ) 
        SELECT TOP(@sync_batch_size_tblEntityCompanyToSite) WITH TIES [map].[tblEntityCompanyToSite].[CompanyToSiteGuid],[map].[tblEntityCompanyToSite].[CompanyGuid],[map].[tblEntityCompanyToSite].[SiteGuid],[map].[tblEntityCompanyToSite].[CreatedDate],[map].[tblEntityCompanyToSite].[CreatedBy],[map].[tblEntityCompanyToSite].[UpdatedDate],[map].[tblEntityCompanyToSite].[UpdatedBy],[map].[tblEntityCompanyToSite].[AssignedFromSiteGuid], data.InsertedRowVersion AS '_RowVersion'
            FROM [map].[tblEntityCompanyToSite]
                INNER JOIN (SELECT data.[CompanyToSiteGuid], data1.InsertedRowVersion
                                FROM EntityAssignmentTree_CTE data
                                    LEFT OUTER JOIN ChangedEntityToSiteRecordList_CTE data1
                                        ON data.[TrackedCompanyToSiteGuid] = data1.[CompanyToSiteGuid]
                                WHERE data1.[TrackedCompanyToSiteGuid] IS NOT NULL) data
                        ON data.CompanyToSiteGuid = [map].[tblEntityCompanyToSite].[CompanyToSiteGuid]
            ORDER BY _RowVersion ASC;


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
