-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCompanies
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblCompanies]
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
@sync_batch_size_tblCompanies int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblCompanies int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblCompanies IS NOT NULL AND @sync_first_time_sync_option_tblCompanies = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblCompanies].[ID],[dbo].[tblCompanies].[Code],[dbo].[tblCompanies].[Name],[dbo].[tblCompanies].[ShortName],[dbo].[tblCompanies].[Address1],[dbo].[tblCompanies].[Address2],[dbo].[tblCompanies].[City],[dbo].[tblCompanies].[State],[dbo].[tblCompanies].[Zip],[dbo].[tblCompanies].[Country],[dbo].[tblCompanies].[Phone],[dbo].[tblCompanies].[FAX],[dbo].[tblCompanies].[EmergencyContact],[dbo].[tblCompanies].[EmergencyPhone],[dbo].[tblCompanies].[FlightPrefix],[dbo].[tblCompanies].[EffectiveDate],[dbo].[tblCompanies].[ExpirationDate],[dbo].[tblCompanies].[OnHold],[dbo].[tblCompanies].[PickupFLights],[dbo].[tblCompanies].[StockTrack],[dbo].[tblCompanies].[SufferLossGain],[dbo].[tblCompanies].[LowStockWarning],[dbo].[tblCompanies].[LockedOut],[dbo].[tblCompanies].[LockedOutReason],[dbo].[tblCompanies].[LockedOutDate],[dbo].[tblCompanies].[ReceivableAccount],[dbo].[tblCompanies].[RefinerCode],[dbo].[tblCompanies].[LastActivityDate],[dbo].[tblCompanies].[CreditOK],[dbo].[tblCompanies].[AdditiveAccounting],[dbo].[tblCompanies].[PurchaseOrderRequired],[dbo].[tblCompanies].[EPANumber],[dbo].[tblCompanies].[FederalID],[dbo].[tblCompanies].[FederalID2],[dbo].[tblCompanies].[FederalID3],[dbo].[tblCompanies].[FederalID4],[dbo].[tblCompanies].[FederalID5],[dbo].[tblCompanies].[StateID],[dbo].[tblCompanies].[TaxNumber],[dbo].[tblCompanies].[FlushPermitted],[dbo].[tblCompanies].[PumpOffPermitted],[dbo].[tblCompanies].[DeliveryToTerminalPermitted],[dbo].[tblCompanies].[LicenseNumber],[dbo].[tblCompanies].[LicenseExpiration],[dbo].[tblCompanies].[InsuranceCompany],[dbo].[tblCompanies].[InsurancePolicy],[dbo].[tblCompanies].[LiabilityAmount],[dbo].[tblCompanies].[HazardousMaterialExclusion],[dbo].[tblCompanies].[InsuranceExpiration],[dbo].[tblCompanies].[AllowDriverEntry],[dbo].[tblCompanies].[PINRequired],[dbo].[tblCompanies].[MaximumVehicleWeight],[dbo].[tblCompanies].[WeightUnits],[dbo].[tblCompanies].[AccountNumber],[dbo].[tblCompanies].[SCACCode],[dbo].[tblCompanies].[DisableOwnerAllocationsCheck],[dbo].[tblCompanies].[DisableShipperAllocationsCheck],[dbo].[tblCompanies].[DisableBillToAllocationsCheck],[dbo].[tblCompanies].[DisableShipToAllocationsCheck],[dbo].[tblCompanies].[LoadRackDisplayText],[dbo].[tblCompanies].[UserData1],[dbo].[tblCompanies].[UserData2],[dbo].[tblCompanies].[UserData3],[dbo].[tblCompanies].[UserData4],[dbo].[tblCompanies].[UserData5],[dbo].[tblCompanies].[UserData6],[dbo].[tblCompanies].[UserData7],[dbo].[tblCompanies].[UserData8],[dbo].[tblCompanies].[CreatedDate],[dbo].[tblCompanies].[CreatedBy],[dbo].[tblCompanies].[UpdatedDate],[dbo].[tblCompanies].[UpdatedBy],[dbo].[tblCompanies].[CompanyGuid],[dbo].[tblCompanies].[SiteGuid],[dbo].[tblCompanies].[IATAGuid],[dbo].[tblCompanies].[ShipperTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerBillToTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerShipToTypeApplicationStringGuid],[dbo].[tblCompanies].[Contact1Name],[dbo].[tblCompanies].[Contact1Address1],[dbo].[tblCompanies].[Contact1Address2],[dbo].[tblCompanies].[Contact1City],[dbo].[tblCompanies].[Contact1State],[dbo].[tblCompanies].[Contact1Zip],[dbo].[tblCompanies].[Contact1Country],[dbo].[tblCompanies].[Contact1PhoneOffice],[dbo].[tblCompanies].[Contact1Fax],[dbo].[tblCompanies].[Contact1EmailAddress],[dbo].[tblCompanies].[Contact2Name],[dbo].[tblCompanies].[Contact2Address1],[dbo].[tblCompanies].[Contact2Address2],[dbo].[tblCompanies].[Contact2City],[dbo].[tblCompanies].[Contact2State],[dbo].[tblCompanies].[Contact2Zip],[dbo].[tblCompanies].[Contact2Country],[dbo].[tblCompanies].[Contact2PhoneOffice],[dbo].[tblCompanies].[Contact2Fax],[dbo].[tblCompanies].[Contact2EmailAddress],[dbo].[tblCompanies].[Contact1PhoneMobile],[dbo].[tblCompanies].[Contact2PhoneMobile],[dbo].[tblCompanies].[_MasterRecordGuid],[dbo].[tblCompanies].[Note],[dbo].[tblCompanies].[HiddenDate],[dbo].[tblCompanies].[ScullyRequired],[dbo].[tblCompanies].[ConsortiumTypeIndex],[dbo].[tblCompanies].[CompanyIATACode],[dbo].[tblCompanies].[CompanyICAOCode], [dbo].[tblCompanies].[_RowVersion]
            FROM [dbo].[tblCompanies]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblCompanies IS NULL OR 
        (@sync_batch_size_tblCompanies IS NOT NULL AND @sync_batch_size_tblCompanies = 0))
    BEGIN
        SET @sync_batch_size_tblCompanies = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT [ID],[Code],[Name],[ShortName],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmergencyContact],[EmergencyPhone],[FlightPrefix],[EffectiveDate],[ExpirationDate],[OnHold],[PickupFLights],[StockTrack],[SufferLossGain],[LowStockWarning],[LockedOut],[LockedOutReason],[LockedOutDate],[ReceivableAccount],[RefinerCode],[LastActivityDate],[CreditOK],[AdditiveAccounting],[PurchaseOrderRequired],[EPANumber],[FederalID],[FederalID2],[FederalID3],[FederalID4],[FederalID5],[StateID],[TaxNumber],[FlushPermitted],[PumpOffPermitted],[DeliveryToTerminalPermitted],[LicenseNumber],[LicenseExpiration],[InsuranceCompany],[InsurancePolicy],[LiabilityAmount],[HazardousMaterialExclusion],[InsuranceExpiration],[AllowDriverEntry],[PINRequired],[MaximumVehicleWeight],[WeightUnits],[AccountNumber],[SCACCode],[DisableOwnerAllocationsCheck],[DisableShipperAllocationsCheck],[DisableBillToAllocationsCheck],[DisableShipToAllocationsCheck],[LoadRackDisplayText],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[CompanyGuid],[SiteGuid],[IATAGuid],[ShipperTypeApplicationStringGuid],[CustomerBillToTypeApplicationStringGuid],[CustomerShipToTypeApplicationStringGuid],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[_MasterRecordGuid],[Note],[HiddenDate],[ScullyRequired],[ConsortiumTypeIndex],[CompanyIATACode],[CompanyICAOCode],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblCompanies) WITH TIES [dbo].[tblCompanies].[ID],[dbo].[tblCompanies].[Code],[dbo].[tblCompanies].[Name],[dbo].[tblCompanies].[ShortName],[dbo].[tblCompanies].[Address1],[dbo].[tblCompanies].[Address2],[dbo].[tblCompanies].[City],[dbo].[tblCompanies].[State],[dbo].[tblCompanies].[Zip],[dbo].[tblCompanies].[Country],[dbo].[tblCompanies].[Phone],[dbo].[tblCompanies].[FAX],[dbo].[tblCompanies].[EmergencyContact],[dbo].[tblCompanies].[EmergencyPhone],[dbo].[tblCompanies].[FlightPrefix],[dbo].[tblCompanies].[EffectiveDate],[dbo].[tblCompanies].[ExpirationDate],[dbo].[tblCompanies].[OnHold],[dbo].[tblCompanies].[PickupFLights],[dbo].[tblCompanies].[StockTrack],[dbo].[tblCompanies].[SufferLossGain],[dbo].[tblCompanies].[LowStockWarning],[dbo].[tblCompanies].[LockedOut],[dbo].[tblCompanies].[LockedOutReason],[dbo].[tblCompanies].[LockedOutDate],[dbo].[tblCompanies].[ReceivableAccount],[dbo].[tblCompanies].[RefinerCode],[dbo].[tblCompanies].[LastActivityDate],[dbo].[tblCompanies].[CreditOK],[dbo].[tblCompanies].[AdditiveAccounting],[dbo].[tblCompanies].[PurchaseOrderRequired],[dbo].[tblCompanies].[EPANumber],[dbo].[tblCompanies].[FederalID],[dbo].[tblCompanies].[FederalID2],[dbo].[tblCompanies].[FederalID3],[dbo].[tblCompanies].[FederalID4],[dbo].[tblCompanies].[FederalID5],[dbo].[tblCompanies].[StateID],[dbo].[tblCompanies].[TaxNumber],[dbo].[tblCompanies].[FlushPermitted],[dbo].[tblCompanies].[PumpOffPermitted],[dbo].[tblCompanies].[DeliveryToTerminalPermitted],[dbo].[tblCompanies].[LicenseNumber],[dbo].[tblCompanies].[LicenseExpiration],[dbo].[tblCompanies].[InsuranceCompany],[dbo].[tblCompanies].[InsurancePolicy],[dbo].[tblCompanies].[LiabilityAmount],[dbo].[tblCompanies].[HazardousMaterialExclusion],[dbo].[tblCompanies].[InsuranceExpiration],[dbo].[tblCompanies].[AllowDriverEntry],[dbo].[tblCompanies].[PINRequired],[dbo].[tblCompanies].[MaximumVehicleWeight],[dbo].[tblCompanies].[WeightUnits],[dbo].[tblCompanies].[AccountNumber],[dbo].[tblCompanies].[SCACCode],[dbo].[tblCompanies].[DisableOwnerAllocationsCheck],[dbo].[tblCompanies].[DisableShipperAllocationsCheck],[dbo].[tblCompanies].[DisableBillToAllocationsCheck],[dbo].[tblCompanies].[DisableShipToAllocationsCheck],[dbo].[tblCompanies].[LoadRackDisplayText],[dbo].[tblCompanies].[UserData1],[dbo].[tblCompanies].[UserData2],[dbo].[tblCompanies].[UserData3],[dbo].[tblCompanies].[UserData4],[dbo].[tblCompanies].[UserData5],[dbo].[tblCompanies].[UserData6],[dbo].[tblCompanies].[UserData7],[dbo].[tblCompanies].[UserData8],[dbo].[tblCompanies].[CreatedDate],[dbo].[tblCompanies].[CreatedBy],[dbo].[tblCompanies].[UpdatedDate],[dbo].[tblCompanies].[UpdatedBy],[dbo].[tblCompanies].[CompanyGuid],[dbo].[tblCompanies].[SiteGuid],[dbo].[tblCompanies].[IATAGuid],[dbo].[tblCompanies].[ShipperTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerBillToTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerShipToTypeApplicationStringGuid],[dbo].[tblCompanies].[Contact1Name],[dbo].[tblCompanies].[Contact1Address1],[dbo].[tblCompanies].[Contact1Address2],[dbo].[tblCompanies].[Contact1City],[dbo].[tblCompanies].[Contact1State],[dbo].[tblCompanies].[Contact1Zip],[dbo].[tblCompanies].[Contact1Country],[dbo].[tblCompanies].[Contact1PhoneOffice],[dbo].[tblCompanies].[Contact1Fax],[dbo].[tblCompanies].[Contact1EmailAddress],[dbo].[tblCompanies].[Contact2Name],[dbo].[tblCompanies].[Contact2Address1],[dbo].[tblCompanies].[Contact2Address2],[dbo].[tblCompanies].[Contact2City],[dbo].[tblCompanies].[Contact2State],[dbo].[tblCompanies].[Contact2Zip],[dbo].[tblCompanies].[Contact2Country],[dbo].[tblCompanies].[Contact2PhoneOffice],[dbo].[tblCompanies].[Contact2Fax],[dbo].[tblCompanies].[Contact2EmailAddress],[dbo].[tblCompanies].[Contact1PhoneMobile],[dbo].[tblCompanies].[Contact2PhoneMobile],[dbo].[tblCompanies].[_MasterRecordGuid],[dbo].[tblCompanies].[Note],[dbo].[tblCompanies].[HiddenDate],[dbo].[tblCompanies].[ScullyRequired],[dbo].[tblCompanies].[ConsortiumTypeIndex],[dbo].[tblCompanies].[CompanyIATACode],[dbo].[tblCompanies].[CompanyICAOCode],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblCompanies]
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblCompanies].[CompanyGuid] = data.[CompanyGuid]
                    INNER JOIN [track].[tblCompanies] CT
                        ON CT.PK_CompanyGuid = [dbo].[tblCompanies].[CompanyGuid] 
                    INNER JOIN [track].[tblEntityCompanyToSite] MAPCT
                        ON MAPCT.PK_CompanyToSiteGuid = data.[CompanyToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
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
