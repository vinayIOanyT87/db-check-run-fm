-- ********************** CREATE SYNCHRONIZATION METHODS FOR tblGasboyStationGeneralConfiguration **********************
-- 
-- 
-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyStationGeneralConfiguration
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblGasboyStationGeneralConfiguration]
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
@sync_batch_size_tblGasboyStationGeneralConfiguration int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyFleetGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyDepartmentGuid],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedDate],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedDate], [dbo].[tblGasboyStationGeneralConfiguration].[_RowVersion]
            FROM [dbo].[tblGasboyStationGeneralConfiguration]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblGasboyStationGeneralConfiguration IS NULL OR 
        (@sync_batch_size_tblGasboyStationGeneralConfiguration IS NOT NULL AND @sync_batch_size_tblGasboyStationGeneralConfiguration = 0))
    BEGIN
        SET @sync_batch_size_tblGasboyStationGeneralConfiguration = 2147483647;
    END

        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblGasboyStationGeneralConfiguration) [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyFleetGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyDepartmentGuid],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedDate],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedDate],CT.InsertedRowVersion AS '_RowVersion'
            FROM [dbo].[tblGasboyStationGeneralConfiguration]
                 INNER JOIN [dbo].[tblExternalStationGeneralConfiguration] ON [dbo].[tblExternalStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid] = [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid] 
                INNER JOIN [track].[tblGasboyStationGeneralConfiguration] CT
                    ON CT.PK_ExternalStationGeneralConfigurationGuid = [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid] 
            WHERE ( [dbo].[tblExternalStationGeneralConfiguration].[SiteGuid] = @sync_context_site_guid)
                    AND ((CT.InsertedRowVersion > @sync_last_received_anchor)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY _RowVersion ASC

    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END