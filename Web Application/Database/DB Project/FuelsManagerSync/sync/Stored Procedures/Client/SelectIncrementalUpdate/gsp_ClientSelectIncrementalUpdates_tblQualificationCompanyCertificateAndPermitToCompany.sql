-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationCompanyCertificateAndPermitToCompany
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblQualificationCompanyCertificateAndPermitToCompany]
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
@sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany int,
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
        SELECT [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationCompanyCertificateAndPermitToCompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Sequence],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Instructor],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateCompleted],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateDue],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ExpirationDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ID],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Rating],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[HistoricalRecord],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedBy],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedBy], [map].[tblQualificationCompanyCertificateAndPermitToCompany].[_RowVersion]
            FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany IS NULL OR 
        (@sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany IS NOT NULL AND @sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany = 0))
    BEGIN
        SET @sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany = 2147483647;
    END

            SELECT TOP(@sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany) WITH TIES [QualificationCompanyCertificateAndPermitToCompanyGuid],[QualificationGuid],[CompanyGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualificationCompanyCertificateAndPermitToCompany) WITH TIES [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationCompanyCertificateAndPermitToCompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Sequence],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Instructor],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateCompleted],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateDue],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ExpirationDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ID],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Rating],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[HistoricalRecord],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedBy],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,MAPCT2.UpdatedRowVersion) AS '_RowVersion'
                    FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany]
                        INNER JOIN (SELECT [CompanyCertificateAndPermitToSiteGuid],[QualificationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualificationCompanyCertificateAndPermitListForSite](@sync_context_site_guid)) data
                            ON [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationGuid] = data.[QualificationGuid]
                        INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
                            ON [map].[tblQualificationCompanyCertificateAndPermitToCompany].[CompanyGuid] = data1.[CompanyGuid]
                        INNER JOIN [track].[tblQualificationCompanyCertificateAndPermitToCompany] CT
                            ON CT.PK_QualificationCompanyCertificateAndPermitToCompanyGuid = [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationCompanyCertificateAndPermitToCompanyGuid] 
                        INNER JOIN [track].[tblEntityCompanyCertificateAndPermitToSite] MAPCT
                            ON MAPCT.PK_CompanyCertificateAndPermitToSiteGuid = data.[CompanyCertificateAndPermitToSiteGuid]
                        INNER JOIN [track].[tblEntityCompanyToSite] MAPCT2
                            ON MAPCT2.PK_CompanyToSiteGuid = data1.[CompanyToSiteGuid]
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT2.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT2.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT2.UpdatedRowVersion > MAPCT2.InsertedRowVersion)
                            AND (MAPCT2.UpdatedContext IS NULL OR MAPCT2.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
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
