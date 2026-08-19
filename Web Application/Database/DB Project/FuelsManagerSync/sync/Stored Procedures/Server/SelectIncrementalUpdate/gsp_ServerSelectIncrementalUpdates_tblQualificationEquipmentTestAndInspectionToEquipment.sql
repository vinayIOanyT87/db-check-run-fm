-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationEquipmentTestAndInspectionToEquipment
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalUpdates_tblQualificationEquipmentTestAndInspectionToEquipment]
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
@sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment int,
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
        SELECT [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationEquipmentTestAndInspectionToEquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[EquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Sequence],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Instructor],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateCompleted],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateDue],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ExpirationDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ID],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Rating],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[HistoricalRecord],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedBy],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedBy], [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[_RowVersion]
            FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment IS NULL OR 
        (@sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment IS NOT NULL AND @sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment = 0))
    BEGIN
        SET @sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment = 2147483647;
    END

            SELECT TOP(@sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment) WITH TIES [QualificationEquipmentTestAndInspectionToEquipmentGuid],[QualificationGuid],[EquipmentGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualificationEquipmentTestAndInspectionToEquipment) WITH TIES [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationEquipmentTestAndInspectionToEquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[EquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Sequence],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Instructor],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateCompleted],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateDue],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ExpirationDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ID],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Rating],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[HistoricalRecord],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedBy],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,MAPCT2.UpdatedRowVersion) AS '_RowVersion'
                    FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
                        INNER JOIN (SELECT [EquipmentTestAndInspectionToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationEquipmentTestAndInspectionListForSite](@sync_context_site_guid)) data
                            ON [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationGuid] = data.[QualificationGuid]
                        INNER JOIN (SELECT [EquipmentToSiteGuid],[EquipmentGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedEquipmentListForSite](@sync_context_site_guid)) data1
                            ON [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[EquipmentGuid] = data1.[EquipmentGuid]
                        INNER JOIN [track].[tblQualificationEquipmentTestAndInspectionToEquipment] CT
                            ON CT.PK_QualificationEquipmentTestAndInspectionToEquipmentGuid = [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationEquipmentTestAndInspectionToEquipmentGuid] 
                        INNER JOIN [track].[tblEntityEquipmentTestAndInspectionToSite] MAPCT
                            ON MAPCT.PK_EquipmentTestAndInspectionToSiteGuid = data.[EquipmentTestAndInspectionToSiteGuid]
                        INNER JOIN [track].[tblEntityEquipmentToSite] MAPCT2
                            ON MAPCT2.PK_EquipmentToSiteGuid = data1.[EquipmentToSiteGuid]
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT2.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT2.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT2.UpdatedRowVersion > MAPCT2.InsertedRowVersion)
                            AND (MAPCT2.UpdatedContext IS NULL OR MAPCT2.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    ORDER BY _RowVersion ASC
                ) rs1
            ORDER BY _RowVersion ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
