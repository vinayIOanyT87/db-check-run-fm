-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableSite
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblProcessVariableSite]
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
@sync_batch_size_tblProcessVariableSite int,
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
        SELECT [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid],[dbo].[tblProcessVariableSite].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableSite].[InstanceNumber],[dbo].[tblProcessVariableSite].[SiteGuid],[dbo].[tblProcessVariableSite].[OPCConnectionGuid],[dbo].[tblProcessVariableSite].[OPCItemID],[dbo].[tblProcessVariableSite].[DataType],[dbo].[tblProcessVariableSite].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableSite].[Quality],[dbo].[tblProcessVariableSite].[SIValue],[dbo].[tblProcessVariableSite].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableSite].[DateTimeStamp],[dbo].[tblProcessVariableSite].[Maximum],[dbo].[tblProcessVariableSite].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableSite].[Minimum],[dbo].[tblProcessVariableSite].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableSite].[DataTypeEnabled],[dbo].[tblProcessVariableSite].[Input],[dbo].[tblProcessVariableSite].[InputEnabled],[dbo].[tblProcessVariableSite].[MessageApplicationStringGuid],[dbo].[tblProcessVariableSite].[CreatedDate],[dbo].[tblProcessVariableSite].[CreatedBy],[dbo].[tblProcessVariableSite].[UpdatedDate],[dbo].[tblProcessVariableSite].[UpdatedBy], [dbo].[tblProcessVariableSite].[_RowVersion]
            FROM [dbo].[tblProcessVariableSite]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableSite IS NULL OR 
        (@sync_batch_size_tblProcessVariableSite IS NOT NULL AND @sync_batch_size_tblProcessVariableSite = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableSite = 2147483647;
    END

            SELECT TOP(@sync_batch_size_tblProcessVariableSite) WITH TIES [ProcessVariableSiteGuid],[LookupProcessVariableTypeIndex],[InstanceNumber],[SiteGuid],[OPCConnectionGuid],[OPCItemID],[DataType],[ServerEngineeringUnitsIndex],[Quality],[SIValue],[LookupSIValueVariantTypeIndex],[DateTimeStamp],[Maximum],[LookupMaximumVariantTypeIndex],[Minimum],[LookupMinimumVariantTypeIndex],[DataTypeEnabled],[Input],[InputEnabled],[MessageApplicationStringGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblProcessVariableSite) WITH TIES [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid],[dbo].[tblProcessVariableSite].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableSite].[InstanceNumber],[dbo].[tblProcessVariableSite].[SiteGuid],[dbo].[tblProcessVariableSite].[OPCConnectionGuid],[dbo].[tblProcessVariableSite].[OPCItemID],[dbo].[tblProcessVariableSite].[DataType],[dbo].[tblProcessVariableSite].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableSite].[Quality],[dbo].[tblProcessVariableSite].[SIValue],[dbo].[tblProcessVariableSite].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableSite].[DateTimeStamp],[dbo].[tblProcessVariableSite].[Maximum],[dbo].[tblProcessVariableSite].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableSite].[Minimum],[dbo].[tblProcessVariableSite].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableSite].[DataTypeEnabled],[dbo].[tblProcessVariableSite].[Input],[dbo].[tblProcessVariableSite].[InputEnabled],[dbo].[tblProcessVariableSite].[MessageApplicationStringGuid],[dbo].[tblProcessVariableSite].[CreatedDate],[dbo].[tblProcessVariableSite].[CreatedBy],[dbo].[tblProcessVariableSite].[UpdatedDate],[dbo].[tblProcessVariableSite].[UpdatedBy],CT.UpdatedRowVersion AS '_RowVersion'
                    FROM [dbo].[tblProcessVariableSite]
                        INNER JOIN (SELECT [ProcessVariableSiteGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableSiteListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid] = data.[ProcessVariableSiteGuid]
                        INNER JOIN [track].[tblProcessVariableSite] CT
                            ON CT.PK_ProcessVariableSiteGuid = [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid] 
                    WHERE ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs1
            ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
