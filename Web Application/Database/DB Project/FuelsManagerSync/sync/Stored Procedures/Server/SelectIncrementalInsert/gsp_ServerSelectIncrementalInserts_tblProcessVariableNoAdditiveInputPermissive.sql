-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableNoAdditiveInputPermissive
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProcessVariableNoAdditiveInputPermissive]
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
@sync_batch_size_tblProcessVariableNoAdditiveInputPermissive int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableNoAdditiveInputPermissive int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProcessVariableNoAdditiveInputPermissive IS NOT NULL AND @sync_first_time_sync_option_tblProcessVariableNoAdditiveInputPermissive = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProcessVariableNoAdditiveInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCItemID],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataType],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Quality],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[SIValue],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Maximum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Minimum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Input],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InputEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedBy],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedBy], [dbo].[tblProcessVariableNoAdditiveInputPermissive].[_RowVersion]
            FROM [dbo].[tblProcessVariableNoAdditiveInputPermissive]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableNoAdditiveInputPermissive IS NULL OR 
        (@sync_batch_size_tblProcessVariableNoAdditiveInputPermissive IS NOT NULL AND @sync_batch_size_tblProcessVariableNoAdditiveInputPermissive = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableNoAdditiveInputPermissive = 2147483647;
    END

    -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblStation and we must go 
    -- through tblLoadArms.  If you need to change this, it's better to make the changes to the templates (client and server) 
    -- and regenerate this script.  This will keep the templates up-to-date for other developers.

    -- Now that we know which LoadArms would have been inserted, we can focus on whether these records have been inserted or the loadarm, similar to an entity assignment
    SELECT TOP(@sync_batch_size_tblProcessVariableNoAdditiveInputPermissive) WITH TIES [dbo].[tblProcessVariableNoAdditiveInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCItemID],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataType],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Quality],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[SIValue],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Maximum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Minimum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Input],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InputEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedBy],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedBy], CT.InsertedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableNoAdditiveInputPermissive]
            INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data
                ON [dbo].[tblProcessVariableNoAdditiveInputPermissive].[LoadArmGuid] = data.[LoadArmGuid]
            INNER JOIN [track].[tblProcessVariableNoAdditiveInputPermissive] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableNoAdditiveInputPermissive].[ProcessVariableLoadArmGuid]
        WHERE ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC;


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
