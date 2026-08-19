-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPersonnel
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblPersonnel]
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
@sync_batch_size_tblPersonnel int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblPersonnel int
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblPersonnel].[PersonID],[dbo].[tblPersonnel].[CardNumber],[dbo].[tblPersonnel].[FirstName],[dbo].[tblPersonnel].[MiddleName],[dbo].[tblPersonnel].[LastName],[dbo].[tblPersonnel].[Title],[dbo].[tblPersonnel].[Department],[dbo].[tblPersonnel].[Address1],[dbo].[tblPersonnel].[Address2],[dbo].[tblPersonnel].[City],[dbo].[tblPersonnel].[State],[dbo].[tblPersonnel].[Zip],[dbo].[tblPersonnel].[Country],[dbo].[tblPersonnel].[Phone1],[dbo].[tblPersonnel].[Phone2],[dbo].[tblPersonnel].[AssignmentDate],[dbo].[tblPersonnel].[SupervisionDate],[dbo].[tblPersonnel].[SSAN],[dbo].[tblPersonnel].[BirthDate],[dbo].[tblPersonnel].[PayRate],[dbo].[tblPersonnel].[LaborRate1],[dbo].[tblPersonnel].[LaborRate2],[dbo].[tblPersonnel].[LaborRate3],[dbo].[tblPersonnel].[LaborRate4],[dbo].[tblPersonnel].[Status],[dbo].[tblPersonnel].[Email],[dbo].[tblPersonnel].[ResponsibleOfficer],[dbo].[tblPersonnel].[Shift],[dbo].[tblPersonnel].[PINNumber],[dbo].[tblPersonnel].[PINRequired],[dbo].[tblPersonnel].[LockedOut],[dbo].[tblPersonnel].[LockedOutReason],[dbo].[tblPersonnel].[LockedOutDate],[dbo].[tblPersonnel].[LastActivityDate],[dbo].[tblPersonnel].[CardedIn],[dbo].[tblPersonnel].[ShortCardNumber],[dbo].[tblPersonnel].[CreatedDate],[dbo].[tblPersonnel].[CreatedBy],[dbo].[tblPersonnel].[UpdatedDate],[dbo].[tblPersonnel].[UpdatedBy],[dbo].[tblPersonnel].[OnFileSignature],[dbo].[tblPersonnel].[UserData1],[dbo].[tblPersonnel].[UserData2],[dbo].[tblPersonnel].[UserData3],[dbo].[tblPersonnel].[UserData4],[dbo].[tblPersonnel].[UserData5],[dbo].[tblPersonnel].[UserData6],[dbo].[tblPersonnel].[UserData7],[dbo].[tblPersonnel].[UserData8],[dbo].[tblPersonnel].[UserData9],[dbo].[tblPersonnel].[UserData10],[dbo].[tblPersonnel].[UserData11],[dbo].[tblPersonnel].[UserData12],[dbo].[tblPersonnel].[UserData13],[dbo].[tblPersonnel].[UserData14],[dbo].[tblPersonnel].[UserData15],[dbo].[tblPersonnel].[UserData16],[dbo].[tblPersonnel].[UserData17],[dbo].[tblPersonnel].[UserData18],[dbo].[tblPersonnel].[UserData19],[dbo].[tblPersonnel].[UserData20],[dbo].[tblPersonnel].[UserData21],[dbo].[tblPersonnel].[UserData22],[dbo].[tblPersonnel].[UserData23],[dbo].[tblPersonnel].[UserData24],[dbo].[tblPersonnel].[InhibitInactivityLockout],[dbo].[tblPersonnel].[PersonnelGuid],[dbo].[tblPersonnel].[SiteGuid],[dbo].[tblPersonnel].[CompanyGuid],[dbo].[tblPersonnel].[SupervisorPersonnelGuid],[dbo].[tblPersonnel].[UserGuid],[dbo].[tblPersonnel].[AssignedEquipmentGuid],[dbo].[tblPersonnel].[_MasterRecordGuid],[dbo].[tblPersonnel].[HiddenDate], [dbo].[tblPersonnel].[_RowVersion]
            FROM [dbo].[tblPersonnel]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblPersonnel IS NULL OR 
        (@sync_batch_size_tblPersonnel IS NOT NULL AND @sync_batch_size_tblPersonnel = 0))
    BEGIN
        SET @sync_batch_size_tblPersonnel = 2147483647;
    END



        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
            SELECT [PersonID],[CardNumber],[FirstName],[MiddleName],[LastName],[Title],[Department],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone1],[Phone2],[AssignmentDate],[SupervisionDate],[SSAN],[BirthDate],[PayRate],[LaborRate1],[LaborRate2],[LaborRate3],[LaborRate4],[Status],[Email],[ResponsibleOfficer],[Shift],[PINNumber],[PINRequired],[LockedOut],[LockedOutReason],[LockedOutDate],[LastActivityDate],[CardedIn],[ShortCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[OnFileSignature],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[InhibitInactivityLockout],[PersonnelGuid],[SiteGuid],[CompanyGuid],[SupervisorPersonnelGuid],[UserGuid],[AssignedEquipmentGuid],[_MasterRecordGuid],[HiddenDate],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblPersonnel) WITH TIES [dbo].[tblPersonnel].[PersonID],[dbo].[tblPersonnel].[CardNumber],[dbo].[tblPersonnel].[FirstName],[dbo].[tblPersonnel].[MiddleName],[dbo].[tblPersonnel].[LastName],[dbo].[tblPersonnel].[Title],[dbo].[tblPersonnel].[Department],[dbo].[tblPersonnel].[Address1],[dbo].[tblPersonnel].[Address2],[dbo].[tblPersonnel].[City],[dbo].[tblPersonnel].[State],[dbo].[tblPersonnel].[Zip],[dbo].[tblPersonnel].[Country],[dbo].[tblPersonnel].[Phone1],[dbo].[tblPersonnel].[Phone2],[dbo].[tblPersonnel].[AssignmentDate],[dbo].[tblPersonnel].[SupervisionDate],[dbo].[tblPersonnel].[SSAN],[dbo].[tblPersonnel].[BirthDate],[dbo].[tblPersonnel].[PayRate],[dbo].[tblPersonnel].[LaborRate1],[dbo].[tblPersonnel].[LaborRate2],[dbo].[tblPersonnel].[LaborRate3],[dbo].[tblPersonnel].[LaborRate4],[dbo].[tblPersonnel].[Status],[dbo].[tblPersonnel].[Email],[dbo].[tblPersonnel].[ResponsibleOfficer],[dbo].[tblPersonnel].[Shift],[dbo].[tblPersonnel].[PINNumber],[dbo].[tblPersonnel].[PINRequired],[dbo].[tblPersonnel].[LockedOut],[dbo].[tblPersonnel].[LockedOutReason],[dbo].[tblPersonnel].[LockedOutDate],[dbo].[tblPersonnel].[LastActivityDate],[dbo].[tblPersonnel].[CardedIn],[dbo].[tblPersonnel].[ShortCardNumber],[dbo].[tblPersonnel].[CreatedDate],[dbo].[tblPersonnel].[CreatedBy],[dbo].[tblPersonnel].[UpdatedDate],[dbo].[tblPersonnel].[UpdatedBy],[dbo].[tblPersonnel].[OnFileSignature],[dbo].[tblPersonnel].[UserData1],[dbo].[tblPersonnel].[UserData2],[dbo].[tblPersonnel].[UserData3],[dbo].[tblPersonnel].[UserData4],[dbo].[tblPersonnel].[UserData5],[dbo].[tblPersonnel].[UserData6],[dbo].[tblPersonnel].[UserData7],[dbo].[tblPersonnel].[UserData8],[dbo].[tblPersonnel].[UserData9],[dbo].[tblPersonnel].[UserData10],[dbo].[tblPersonnel].[UserData11],[dbo].[tblPersonnel].[UserData12],[dbo].[tblPersonnel].[UserData13],[dbo].[tblPersonnel].[UserData14],[dbo].[tblPersonnel].[UserData15],[dbo].[tblPersonnel].[UserData16],[dbo].[tblPersonnel].[UserData17],[dbo].[tblPersonnel].[UserData18],[dbo].[tblPersonnel].[UserData19],[dbo].[tblPersonnel].[UserData20],[dbo].[tblPersonnel].[UserData21],[dbo].[tblPersonnel].[UserData22],[dbo].[tblPersonnel].[UserData23],[dbo].[tblPersonnel].[UserData24],[dbo].[tblPersonnel].[InhibitInactivityLockout],[dbo].[tblPersonnel].[PersonnelGuid],[dbo].[tblPersonnel].[SiteGuid],[dbo].[tblPersonnel].[CompanyGuid],[dbo].[tblPersonnel].[SupervisorPersonnelGuid],[dbo].[tblPersonnel].[UserGuid],[dbo].[tblPersonnel].[AssignedEquipmentGuid],[dbo].[tblPersonnel].[_MasterRecordGuid],[dbo].[tblPersonnel].[HiddenDate],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblPersonnel]
                        INNER JOIN (SELECT [PersonnelToSiteGuid],[PersonnelGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedPersonnelListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblPersonnel].[PersonnelGuid] = data.[PersonnelGuid]
                        INNER JOIN [track].[tblPersonnel] CT
                            ON CT.PK_PersonnelGuid = [dbo].[tblPersonnel].[PersonnelGuid] 
                        INNER JOIN [track].[tblEntityPersonnelToSite] MAPCT
                            ON MAPCT.PK_PersonnelToSiteGuid = data.[PersonnelToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
