-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblApplicationString
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalUpdates_tblApplicationString]
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
@sync_batch_size_tblApplicationString int,
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
        SELECT [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex], [dbo].[tblApplicationString].[_RowVersion]
            FROM [dbo].[tblApplicationString]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblApplicationString IS NULL OR 
        (@sync_batch_size_tblApplicationString IS NOT NULL AND @sync_batch_size_tblApplicationString = 0))
    BEGIN
        SET @sync_batch_size_tblApplicationString = 2147483647;
    END

        SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
        FROM (
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [AlarmAndEventCategoryToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringAlarmAndEventCategoryListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityAlarmAndEventCategoryToSite] MAPCT
                            ON MAPCT.PK_AlarmAndEventCategoryToSiteGuid = data.[AlarmAndEventCategoryToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [AllocationGroupToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringAllocationGroupListForSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data1.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityAllocationGroupToSite] MAPCT
                            ON MAPCT.PK_AllocationGroupToSiteGuid = data1.[AllocationGroupToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs2
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [CompanyGroupToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringCompanyGroupListForSite](@sync_context_site_guid)) data2
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data2.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityCompanyGroupToSite] MAPCT
                            ON MAPCT.PK_CompanyGroupToSiteGuid = data2.[CompanyGroupToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs3
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [CompanyTypeToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringCompanyTypeListForSite](@sync_context_site_guid)) data3
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data3.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityCompanyTypeToSite] MAPCT
                            ON MAPCT.PK_CompanyTypeToSiteGuid = data3.[CompanyTypeToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs4
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [DotHazardousMessagesToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringDotHazardousMessageListForSite](@sync_context_site_guid)) data4
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data4.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityDotHazardousMessagesToSite] MAPCT
                            ON MAPCT.PK_DotHazardousMessagesToSiteGuid = data4.[DotHazardousMessagesToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs5
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [EmailAddressToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringEmailAddressListForSite](@sync_context_site_guid)) data5
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data5.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityEmailAddressToSite] MAPCT
                            ON MAPCT.PK_EmailAddressToSiteGuid = data5.[EmailAddressToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs6
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [EntryMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringEntryMessageListForSite](@sync_context_site_guid)) data6
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data6.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityEntryMessageToSite] MAPCT
                            ON MAPCT.PK_EntryMessageToSiteGuid = data6.[EntryMessageToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs7
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [ExitMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringExitMessageListForSite](@sync_context_site_guid)) data7
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data7.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityExitMessageToSite] MAPCT
                            ON MAPCT.PK_ExitMessageToSiteGuid = data7.[ExitMessageToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs8
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [FootNoteToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringFootNoteListForSite](@sync_context_site_guid)) data8
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data8.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityFootNoteToSite] MAPCT
                            ON MAPCT.PK_FootNoteToSiteGuid = data8.[FootNoteToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs9
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data9
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data9.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityProcessVariableMessageToSite] MAPCT
                            ON MAPCT.PK_ProcessVariableMessageToSiteGuid = data9.[ProcessVariableMessageToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs10
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [ProductGroupToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProductGroupListForSite](@sync_context_site_guid)) data10
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data10.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityProductGroupToSite] MAPCT
                            ON MAPCT.PK_ProductGroupToSiteGuid = data10.[ProductGroupToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs11
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [ProductMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProductMessageListForSite](@sync_context_site_guid)) data11
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data11.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityProductMessageToSite] MAPCT
                            ON MAPCT.PK_ProductMessageToSiteGuid = data11.[ProductMessageToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs12
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [FuelCardTypeToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringFuelCardTypeListForSite](@sync_context_site_guid)) data12
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data12.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityFuelCardTypeToSite] MAPCT
                            ON MAPCT.PK_FuelCardTypeToSiteGuid = data12.[FuelCardTypeToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs13
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [PointCategoryToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringPointCategoryListForSite](@sync_context_site_guid)) data13
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data13.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityPointCategoryToSite] MAPCT
                            ON MAPCT.PK_PointCategoryToSiteGuid = data13.[PointCategoryToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs14
            UNION
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                        INNER JOIN (SELECT [PointTemplateTypeToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringPointTemplateTypeListForSite](@sync_context_site_guid)) data14
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data14.[ApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityPointTemplateTypeToSite] MAPCT
                            ON MAPCT.PK_PointTemplateTypeToSiteGuid = data14.[PointTemplateTypeToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs15
            UNION   -- Special Application String Handler for Ship To State Strings which are NOT Entity Assignable and must filter according to the Foot Note Assignments
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion, MAPCT.UpdatedRowVersion, MAPCT2.UpdatedRowVersion) AS '_RowVersion'
                    FROM (SELECT [FootNoteToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringFootNoteListForSite](@sync_context_site_guid)) spec1
                        INNER JOIN (SELECT [ApplicationStringToFootNoteShipToStateGuid], [ApplicationStringGuid], [AssignedToApplicationStringGuid] FROM [map].[tblApplicationStringToFootNoteShipToState]) spec2
                            ON spec1.[ApplicationStringGuid] = spec2.[ApplicationStringGuid]
                        INNER JOIN [dbo].[tblApplicationString]
                            ON [dbo].[tblApplicationString].[ApplicationStringGuid] = spec2.[AssignedToApplicationStringGuid]
                        INNER JOIN [track].[tblApplicationString] CT
                            ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                        INNER JOIN [track].[tblEntityFootNoteToSite] MAPCT
                            ON MAPCT.PK_FootNoteToSiteGuid = spec1.[FootNoteToSiteGuid] 
                        INNER JOIN [track].[tblApplicationStringToFootNoteShipToState] MAPCT2
                            ON MAPCT2.PK_ApplicationStringToFootNoteShipToStateGuid = spec2.[ApplicationStringToFootNoteShipToStateGuid]
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
            ) rs16
            UNION   -- Special Application String Handler for Certificate List Strings which are NOT Entity Assignable and must filter according site ownership
            SELECT [ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StartDate],[EndDate],[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblApplicationString) WITH TIES [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion, NULL, NULL) AS '_RowVersion'
                    FROM [dbo].[tblApplicationString]
                    INNER JOIN (SELECT [ApplicationStringGuid] FROM [dbo].[udf_GetAssignedApplicationStringCertificateListForSite](@sync_context_site_guid)) data15
                        ON [dbo].[tblApplicationString].[ApplicationStringGuid] = data15.[ApplicationStringGuid]
                    INNER JOIN [track].[tblApplicationString] CT
                        ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid] 
                    WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    ORDER BY _RowVersion ASC
            ) rs17            

        ) mainRs
        ORDER BY _RowVersion ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
