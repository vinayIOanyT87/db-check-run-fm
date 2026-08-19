-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteShipTo
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblApplicationStringToFootNoteShipTo]
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
@sync_batch_size_tblApplicationStringToFootNoteShipTo int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblApplicationStringToFootNoteShipTo int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblApplicationStringToFootNoteShipTo IS NOT NULL AND @sync_first_time_sync_option_tblApplicationStringToFootNoteShipTo = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid],[map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipTo].[CompanyGuid],[map].[tblApplicationStringToFootNoteShipTo].[Sequence],[map].[tblApplicationStringToFootNoteShipTo].[CreatedDate],[map].[tblApplicationStringToFootNoteShipTo].[CreatedBy],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedBy], [map].[tblApplicationStringToFootNoteShipTo].[_RowVersion]
            FROM [map].[tblApplicationStringToFootNoteShipTo]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblApplicationStringToFootNoteShipTo IS NULL OR 
        (@sync_batch_size_tblApplicationStringToFootNoteShipTo IS NOT NULL AND @sync_batch_size_tblApplicationStringToFootNoteShipTo = 0))
    BEGIN
        SET @sync_batch_size_tblApplicationStringToFootNoteShipTo = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblApplicationStringToFootNoteShipTo) WITH TIES [ApplicationStringToFootNoteShipToGuid],[ApplicationStringGuid],[CompanyGuid],[Sequence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
    FROM (
        SELECT [ApplicationStringToFootNoteShipToGuid],[ApplicationStringGuid],[CompanyGuid],[Sequence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblApplicationStringToFootNoteShipTo) WITH TIES [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid],[map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipTo].[CompanyGuid],[map].[tblApplicationStringToFootNoteShipTo].[Sequence],[map].[tblApplicationStringToFootNoteShipTo].[CreatedDate],[map].[tblApplicationStringToFootNoteShipTo].[CreatedBy],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,MAPCT2.InsertedRowVersion) AS '_RowVersion'
                FROM [map].[tblApplicationStringToFootNoteShipTo]
                    INNER JOIN (SELECT [FootNoteToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM dbo.udf_GetAssignedApplicationStringFootNoteListForSite(@sync_context_site_guid)) data
                        ON [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid] = data.[ApplicationStringGuid]
                    INNER JOIN (SELECT [ApplicationStringToFootNoteShipToGuid],[CompanyToSiteGuid], [CompanyGuid],[OwnerSiteGuid],[FootNoteToSiteGuid] FROM [map].[udf_GetAssociatedShipToCompaniesAssignedToFootnotesForSite](@sync_context_site_guid,0)) data1
                        ON [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid] = data1.[ApplicationStringToFootNoteShipToGuid]
                    INNER JOIN [track].[tblApplicationStringToFootNoteShipTo] CT
                        ON CT.PK_ApplicationStringToFootNoteShipToGuid = [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid] 
                    INNER JOIN [track].[tblEntityFootNoteToSite] MAPCT
                        ON MAPCT.PK_FootNoteToSiteGuid = data.[FootNoteToSiteGuid]
                    INNER JOIN [track].[tblEntityCompanyToSite] MAPCT2
                        ON MAPCT2.PK_CompanyToSiteGuid = data1.[CompanyToSiteGuid]
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
        UNION
        SELECT [ApplicationStringToFootNoteShipToGuid],[ApplicationStringGuid],[CompanyGuid],[Sequence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblApplicationStringToFootNoteShipTo) WITH TIES [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid],[map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipTo].[CompanyGuid],[map].[tblApplicationStringToFootNoteShipTo].[Sequence],[map].[tblApplicationStringToFootNoteShipTo].[CreatedDate],[map].[tblApplicationStringToFootNoteShipTo].[CreatedBy],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,MAPCT2.InsertedRowVersion) AS '_RowVersion'
                FROM [map].[tblApplicationStringToFootNoteShipTo]
                    INNER JOIN (SELECT [FootNoteToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM dbo.udf_GetAssignedApplicationStringFootNoteListForSite(@sync_context_site_guid)) data2
                        ON [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid] = data2.[ApplicationStringGuid]
                    INNER JOIN (SELECT [ApplicationStringToFootNoteShipToGuid],[CompanyToSiteGuid], [CompanyGuid],[OwnerSiteGuid],[FootNoteToSiteGuid] FROM [map].[udf_GetAssociatedShipToCompaniesAssignedToFootnotesForSite](@sync_context_site_guid,1)) data3
                        ON [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid] = data3.[ApplicationStringToFootNoteShipToGuid]
                    INNER JOIN [track].[tblApplicationStringToFootNoteShipTo] CT
                        ON CT.PK_ApplicationStringToFootNoteShipToGuid = [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid] 
                    INNER JOIN [track].[tblEntityFootNoteToSite] MAPCT
                        ON MAPCT.PK_FootNoteToSiteGuid = data2.[FootNoteToSiteGuid]
                    INNER JOIN [track].[tblEntityFootNoteToSite] MAPCT2
                        ON MAPCT2.PK_FootNoteToSiteGuid = data3.[FootNoteToSiteGuid]
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
            ) rs2   -- DetectedSubFunctions: True / IncludeEntityAssignments: True
        ) mainRs
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
