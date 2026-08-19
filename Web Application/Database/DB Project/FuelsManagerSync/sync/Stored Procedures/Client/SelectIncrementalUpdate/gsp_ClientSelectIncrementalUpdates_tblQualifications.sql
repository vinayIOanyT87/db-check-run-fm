-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblQualifications
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblQualifications]
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
@sync_batch_size_tblQualifications int,
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
        SELECT [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex], [dbo].[tblQualifications].[_RowVersion]
            FROM [dbo].[tblQualifications]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblQualifications IS NULL OR 
        (@sync_batch_size_tblQualifications IS NOT NULL AND @sync_batch_size_tblQualifications = 0))
    BEGIN
        SET @sync_batch_size_tblQualifications = 2147483647;
    END

        SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
        FROM (
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [CompanyCertificateAndPermitToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationCompanyCertificateAndPermitListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblQualifications].[QualificationGuid] = data.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityCompanyCertificateAndPermitToSite] MAPCT
                            ON MAPCT.PK_CompanyCertificateAndPermitToSiteGuid = data.[CompanyCertificateAndPermitToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs1
            UNION
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [EquipmentTagAndLicenseToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationEquipmentTagAndLicenseListForSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblQualifications].[QualificationGuid] = data1.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityEquipmentTagAndLicenseToSite] MAPCT
                            ON MAPCT.PK_EquipmentTagAndLicenseToSiteGuid = data1.[EquipmentTagAndLicenseToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs2
            UNION
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [EquipmentTestAndInspectionToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationEquipmentTestAndInspectionListForSite](@sync_context_site_guid)) data2
                            ON [dbo].[tblQualifications].[QualificationGuid] = data2.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityEquipmentTestAndInspectionToSite] MAPCT
                            ON MAPCT.PK_EquipmentTestAndInspectionToSiteGuid = data2.[EquipmentTestAndInspectionToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs3
            UNION
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [PersonnelLicenseToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationPersonnelLicenseListForSite](@sync_context_site_guid)) data3
                            ON [dbo].[tblQualifications].[QualificationGuid] = data3.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityPersonnelLicenseToSite] MAPCT
                            ON MAPCT.PK_PersonnelLicenseToSiteGuid = data3.[PersonnelLicenseToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs4
            UNION
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [PersonnelQualificationToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationPersonnelQualificationListForSite](@sync_context_site_guid)) data4
                            ON [dbo].[tblQualifications].[QualificationGuid] = data4.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityPersonnelQualificationToSite] MAPCT
                            ON MAPCT.PK_PersonnelQualificationToSiteGuid = data4.[PersonnelQualificationToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs5
            UNION
            SELECT [ID],[Description],[Duration],[Reoccurrence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualificationGuid],[SiteGuid],[LookupQualificationTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualifications) WITH TIES [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualifications]
                        INNER JOIN (SELECT [PersonnelTrainingToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationPersonnelTrainingListForSite](@sync_context_site_guid)) data5
                            ON [dbo].[tblQualifications].[QualificationGuid] = data5.[QualificationGuid]
                        INNER JOIN [track].[tblQualifications] CT
                            ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid] 
                        INNER JOIN [track].[tblEntityPersonnelTrainingToSite] MAPCT
                            ON MAPCT.PK_PersonnelTrainingToSiteGuid = data5.[PersonnelTrainingToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs6
        ) mainRs
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
