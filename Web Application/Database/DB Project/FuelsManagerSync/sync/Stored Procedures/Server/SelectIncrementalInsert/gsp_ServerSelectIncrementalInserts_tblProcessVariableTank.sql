-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableTank
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProcessVariableTank]
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
@sync_batch_size_tblProcessVariableTank int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableTank int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProcessVariableTank IS NOT NULL AND @sync_first_time_sync_option_tblProcessVariableTank = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid],[dbo].[tblProcessVariableTank].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableTank].[InstanceNumber],[dbo].[tblProcessVariableTank].[TankGuid],[dbo].[tblProcessVariableTank].[OPCConnectionGuid],[dbo].[tblProcessVariableTank].[OPCItemID],[dbo].[tblProcessVariableTank].[DataType],[dbo].[tblProcessVariableTank].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableTank].[Quality],[dbo].[tblProcessVariableTank].[SIValue],[dbo].[tblProcessVariableTank].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableTank].[DateTimeStamp],[dbo].[tblProcessVariableTank].[Maximum],[dbo].[tblProcessVariableTank].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableTank].[Minimum],[dbo].[tblProcessVariableTank].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableTank].[DataTypeEnabled],[dbo].[tblProcessVariableTank].[Input],[dbo].[tblProcessVariableTank].[InputEnabled],[dbo].[tblProcessVariableTank].[MessageApplicationStringGuid],[dbo].[tblProcessVariableTank].[CreatedDate],[dbo].[tblProcessVariableTank].[CreatedBy],[dbo].[tblProcessVariableTank].[UpdatedDate],[dbo].[tblProcessVariableTank].[UpdatedBy], [dbo].[tblProcessVariableTank].[_RowVersion]
            FROM [dbo].[tblProcessVariableTank]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableTank IS NULL OR 
        (@sync_batch_size_tblProcessVariableTank IS NOT NULL AND @sync_batch_size_tblProcessVariableTank = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableTank = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblProcessVariableTank) WITH TIES [ProcessVariableTankGuid],[LookupProcessVariableTypeIndex],[InstanceNumber],[TankGuid],[OPCConnectionGuid],[OPCItemID],[DataType],[ServerEngineeringUnitsIndex],[Quality],[SIValue],[LookupSIValueVariantTypeIndex],[DateTimeStamp],[Maximum],[LookupMaximumVariantTypeIndex],[Minimum],[LookupMinimumVariantTypeIndex],[DataTypeEnabled],[Input],[InputEnabled],[MessageApplicationStringGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblProcessVariableTank) WITH TIES [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid],[dbo].[tblProcessVariableTank].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableTank].[InstanceNumber],[dbo].[tblProcessVariableTank].[TankGuid],[dbo].[tblProcessVariableTank].[OPCConnectionGuid],[dbo].[tblProcessVariableTank].[OPCItemID],[dbo].[tblProcessVariableTank].[DataType],[dbo].[tblProcessVariableTank].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableTank].[Quality],[dbo].[tblProcessVariableTank].[SIValue],[dbo].[tblProcessVariableTank].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableTank].[DateTimeStamp],[dbo].[tblProcessVariableTank].[Maximum],[dbo].[tblProcessVariableTank].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableTank].[Minimum],[dbo].[tblProcessVariableTank].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableTank].[DataTypeEnabled],[dbo].[tblProcessVariableTank].[Input],[dbo].[tblProcessVariableTank].[InputEnabled],[dbo].[tblProcessVariableTank].[MessageApplicationStringGuid],[dbo].[tblProcessVariableTank].[CreatedDate],[dbo].[tblProcessVariableTank].[CreatedBy],[dbo].[tblProcessVariableTank].[UpdatedDate],[dbo].[tblProcessVariableTank].[UpdatedBy],CT.InsertedRowVersion AS '_RowVersion'
                FROM [dbo].[tblProcessVariableTank]
                    INNER JOIN (SELECT [ProcessVariableTankGuid],[TankGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableTankListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid] = data.[ProcessVariableTankGuid]
                    INNER JOIN [track].[tblProcessVariableTank] CT
                        ON CT.PK_ProcessVariableTankGuid = [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid] 
                WHERE ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
