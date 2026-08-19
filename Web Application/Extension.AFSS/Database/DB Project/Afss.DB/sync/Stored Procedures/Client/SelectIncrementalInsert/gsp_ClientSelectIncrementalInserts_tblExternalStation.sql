-- ********************** CREATE SYNCHRONIZATION METHODS FOR tblExternalStation **********************
-- 
-- 
-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStation
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblExternalStation]
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
@sync_batch_size_tblExternalStation int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblExternalStation].[ExternalStationGuid],[dbo].[tblExternalStation].[SiteGuid],[dbo].[tblExternalStation].[ID],[dbo].[tblExternalStation].[LookupExternalStationTypeIndex],[dbo].[tblExternalStation].[BillingID],[dbo].[tblExternalStation].[DownloadTransactionsAutomatically],[dbo].[tblExternalStation].[LookupExternalStationStatusIndex],[dbo].[tblExternalStation].[LastSuccessfulConnection],[dbo].[tblExternalStation].[LastConnectionAttempt],[dbo].[tblExternalStation].[LastTransactionID],[dbo].[tblExternalStation].[LastDeviceCount],[dbo].[tblExternalStation].[CreatedBy],[dbo].[tblExternalStation].[CreatedDate],[dbo].[tblExternalStation].[UpdatedBy],[dbo].[tblExternalStation].[UpdatedDate], [dbo].[tblExternalStation].[_RowVersion]
            FROM [dbo].[tblExternalStation]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblExternalStation IS NULL OR 
        (@sync_batch_size_tblExternalStation IS NOT NULL AND @sync_batch_size_tblExternalStation = 0))
    BEGIN
        SET @sync_batch_size_tblExternalStation = 2147483647;
    END

        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
            SELECT [ExternalStationGuid],[SiteGuid],[ID],[LookupExternalStationTypeIndex],[BillingID],[DownloadTransactionsAutomatically],[LookupExternalStationStatusIndex],[LastSuccessfulConnection],[LastConnectionAttempt],[LastTransactionID],[LastDeviceCount],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblExternalStation) [dbo].[tblExternalStation].[ExternalStationGuid],[dbo].[tblExternalStation].[SiteGuid],[dbo].[tblExternalStation].[ID],[dbo].[tblExternalStation].[LookupExternalStationTypeIndex],[dbo].[tblExternalStation].[BillingID],[dbo].[tblExternalStation].[DownloadTransactionsAutomatically],[dbo].[tblExternalStation].[LookupExternalStationStatusIndex],[dbo].[tblExternalStation].[LastSuccessfulConnection],[dbo].[tblExternalStation].[LastConnectionAttempt],[dbo].[tblExternalStation].[LastTransactionID],[dbo].[tblExternalStation].[LastDeviceCount],[dbo].[tblExternalStation].[CreatedBy],[dbo].[tblExternalStation].[CreatedDate],[dbo].[tblExternalStation].[UpdatedBy],[dbo].[tblExternalStation].[UpdatedDate],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblExternalStation]
                        INNER JOIN (SELECT [ExternalStationToSiteGuid],[ExternalStationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedExternalStationListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblExternalStation].[ExternalStationGuid] = data.[ExternalStationGuid]
                        INNER JOIN [track].[tblExternalStation] CT
                            ON CT.PK_ExternalStationGuid = [dbo].[tblExternalStation].[ExternalStationGuid] 
                        INNER JOIN [track].[tblEntityExternalStationToSite] MAPCT
                            ON MAPCT.PK_ExternalStationToSiteGuid = data.[ExternalStationToSiteGuid] 
                    WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1   -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            ORDER BY _RowVersion ASC

    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END