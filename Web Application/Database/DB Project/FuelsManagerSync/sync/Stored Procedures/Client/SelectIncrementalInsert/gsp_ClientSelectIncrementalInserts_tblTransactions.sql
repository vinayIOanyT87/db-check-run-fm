-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactions
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblTransactions]
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
@sync_batch_size_tblTransactions int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblTransactions int
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
        SELECT [dbo].[tblTransactions].[TransID],[dbo].[tblTransactions].[AliasName],[dbo].[tblTransactions].[SubType],[dbo].[tblTransactions].[Site],[dbo].[tblTransactions].[TransReferenceID],CONVERT(CHAR(10), [dbo].[tblTransactions].[InventoryDate], 111) AS [InventoryDate],[dbo].[tblTransactions].[ShipToID],[dbo].[tblTransactions].[ShipToCode],[dbo].[tblTransactions].[SupplierID],[dbo].[tblTransactions].[SupplierCode],[dbo].[tblTransactions].[CreatedDate],[dbo].[tblTransactions].[CreatedBy],[dbo].[tblTransactions].[RequestedDeliveryDate],[dbo].[tblTransactions].[UpdatedDate],[dbo].[tblTransactions].[UpdatedBy],[dbo].[tblTransactions].[TransDateTime],[dbo].[tblTransactions].[TransVersion],[dbo].[tblTransactions].[SCACCode],[dbo].[tblTransactions].[CardNumber],[dbo].[tblTransactions].[ShipmentNumber],[dbo].[tblTransactions].[ShipperID],[dbo].[tblTransactions].[ShipperCode],[dbo].[tblTransactions].[OwnerID],[dbo].[tblTransactions].[OwnerCode],[dbo].[tblTransactions].[ManagerID],[dbo].[tblTransactions].[ManagerCode],[dbo].[tblTransactions].[CarrierID],[dbo].[tblTransactions].[CarrierCode],[dbo].[tblTransactions].[ConjoinTransID],[dbo].[tblTransactions].[ReversedTransID],[dbo].[tblTransactions].[LinkedDocumentNumber],[dbo].[tblTransactions].[ReversalType],[dbo].[tblTransactions].[PONumber],[dbo].[tblTransactions].[TimeIn],[dbo].[tblTransactions].[TimeOut],[dbo].[tblTransactions].[TimeEnd],[dbo].[tblTransactions].[RoutingID],[dbo].[tblTransactions].[TicketSource],[dbo].[tblTransactions].[LoadID],[dbo].[tblTransactions].[BillToID],[dbo].[tblTransactions].[BillToCode],[dbo].[tblTransactions].[DriverIdentificationNumber],[dbo].[tblTransactions].[CreditAmount],[dbo].[tblTransactions].[CardExpiration],[dbo].[tblTransactions].[CardName],[dbo].[tblTransactions].[CardType],[dbo].[tblTransactions].[CashAmount],[dbo].[tblTransactions].[RouteOriginationDate],[dbo].[tblTransactions].[InternationalRouteIndicator],[dbo].[tblTransactions].[PreviousRoutingID],[dbo].[tblTransactions].[ShippingDocumentNumber],[dbo].[tblTransactions].[DocumentNumber],[dbo].[tblTransactions].[STD],[dbo].[tblTransactions].[ETD],[dbo].[tblTransactions].[STA],[dbo].[tblTransactions].[ETA],[dbo].[tblTransactions].[SFT],[dbo].[tblTransactions].[FST],[dbo].[tblTransactions].[EstimatedFuelingDuration],[dbo].[tblTransactions].[DeleteFlag],[dbo].[tblTransactions].[TicketMode],[dbo].[tblTransactions].[DestinationRegistrationID1],[dbo].[tblTransactions].[DestinationSerialNumber1],[dbo].[tblTransactions].[DestinationEquipmentType1],[dbo].[tblTransactions].[DestinationEquipmentModel1],[dbo].[tblTransactions].[DestinationCompanyEquipmentID1],[dbo].[tblTransactions].[DestinationRegistrationID2],[dbo].[tblTransactions].[DestinationSerialNumber2],[dbo].[tblTransactions].[DestinationEquipmentType2],[dbo].[tblTransactions].[DestinationEquipmentModel2],[dbo].[tblTransactions].[DestinationCompanyEquipmentID2],[dbo].[tblTransactions].[DestinationRegistrationID3],[dbo].[tblTransactions].[DestinationSerialNumber3],[dbo].[tblTransactions].[DestinationEquipmentType3],[dbo].[tblTransactions].[DestinationEquipmentModel3],[dbo].[tblTransactions].[DestinationCompanyEquipmentID3],[dbo].[tblTransactions].[SourceRegistrationID1],[dbo].[tblTransactions].[SourceSerialNumber1],[dbo].[tblTransactions].[SourceEquipmentType1],[dbo].[tblTransactions].[SourceEquipmentModel1],[dbo].[tblTransactions].[SourceCompanyEquipmentID1],[dbo].[tblTransactions].[SourceRegistrationID2],[dbo].[tblTransactions].[SourceSerialNumber2],[dbo].[tblTransactions].[SourceEquipmentType2],[dbo].[tblTransactions].[SourceEquipmentModel2],[dbo].[tblTransactions].[SourceCompanyEquipmentID2],[dbo].[tblTransactions].[SourceRegistrationID3],[dbo].[tblTransactions].[SourceSerialNumber3],[dbo].[tblTransactions].[SourceEquipmentType3],[dbo].[tblTransactions].[SourceEquipmentModel3],[dbo].[tblTransactions].[SourceCompanyEquipmentID3],[dbo].[tblTransactions].[OperatorID],[dbo].[tblTransactions].[EffectiveDate],[dbo].[tblTransactions].[ExpirationDate],[dbo].[tblTransactions].[ScheduledDate],[dbo].[tblTransactions].[AutoComplete],[dbo].[tblTransactions].[Flag01],[dbo].[tblTransactions].[Flag02],[dbo].[tblTransactions].[Flag03],[dbo].[tblTransactions].[Flag04],[dbo].[tblTransactions].[Flag05],[dbo].[tblTransactions].[Flag06],[dbo].[tblTransactions].[Number01],[dbo].[tblTransactions].[Number02],[dbo].[tblTransactions].[Number03],[dbo].[tblTransactions].[Number04],[dbo].[tblTransactions].[Number05],[dbo].[tblTransactions].[Number06],[dbo].[tblTransactions].[ContactFirstName],[dbo].[tblTransactions].[ContactSurname],[dbo].[tblTransactions].[Date01],[dbo].[tblTransactions].[Date02],[dbo].[tblTransactions].[Date03],[dbo].[tblTransactions].[Date04],[dbo].[tblTransactions].[LegacyNumber],[dbo].[tblTransactions].[Country],[dbo].[tblTransactions].[ContactInfo],[dbo].[tblTransactions].[AssociatedDocNumber],[dbo].[tblTransactions].[AssociatedCLIN],[dbo].[tblTransactions].[SubmittedToAccounting],[dbo].[tblTransactions].[FuelCardID],[dbo].[tblTransactions].[AssociatedTransportOrderNumber],[dbo].[tblTransactions].[RequestedDateTime],[dbo].[tblTransactions].[DispatchedDateTime],[dbo].[tblTransactions].[ErrorFlag],[dbo].[tblTransactions].[TransactionGuid],[dbo].[tblTransactions].[SiteGuid],[dbo].[tblTransactions].[LookupTransTypeIndex],[dbo].[tblTransactions].[LookupTransactionStatusIndex],[dbo].[tblTransactions].[LookupOriginApplicationIndex],[dbo].[tblTransactions].[TransactionAliasGuid],[dbo].[tblTransactions].[BillToCompanyGuid],[dbo].[tblTransactions].[Destination1EquipmentGuid],[dbo].[tblTransactions].[Destination2EquipmentGuid],[dbo].[tblTransactions].[Destination3EquipmentGuid],[dbo].[tblTransactions].[FinalStationIATAGuid],[dbo].[tblTransactions].[FuelCardGuid],[dbo].[tblTransactions].[ManagerCompanyGuid],[dbo].[tblTransactions].[NextStationIATAGuid],[dbo].[tblTransactions].[OperatorPersonnelGuid],[dbo].[tblTransactions].[OriginStationIATAGuid],[dbo].[tblTransactions].[OwnerCompanyGuid],[dbo].[tblTransactions].[PreviousStationIATAGuid],[dbo].[tblTransactions].[ShipperCompanyGuid],[dbo].[tblTransactions].[ShipToCompanyGuid],[dbo].[tblTransactions].[Source1EquipmentGuid],[dbo].[tblTransactions].[Source2EquipmentGuid],[dbo].[tblTransactions].[Source3EquipmentGuid],[dbo].[tblTransactions].[SupplierCompanyGuid],[dbo].[tblTransactions].[CarrierCompanyGuid],[dbo].[tblTransactions].[ReasonCodeGuid],[dbo].[tblTransactions].[OriginStationIATAID],[dbo].[tblTransactions].[PreviousStationIATAID],[dbo].[tblTransactions].[NextStationIATAID],[dbo].[tblTransactions].[FinalStationIATAID],[dbo].[tblTransactions].[OperatorName],[dbo].[tblTransactions].[FuelAdditiveFlag],[dbo].[tblTransactions].[IssuePoint],[dbo].[tblTransactions].[IssuePointNumber],[dbo].[tblTransactions].[RadioNumber],[dbo].[tblTransactions].[GateID],[dbo].[tblTransactions].[GateGuid],[dbo].[tblTransactions].[ShippingMethod],[dbo].[tblTransactions].[ReferencedTransactionGuid], [dbo].[tblTransactions].[_RowVersion]
            FROM [dbo].[tblTransactions]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTransactions IS NULL OR 
        (@sync_batch_size_tblTransactions IS NOT NULL AND @sync_batch_size_tblTransactions = 0))
    BEGIN
        SET @sync_batch_size_tblTransactions = 2147483647;
    END

        -- We only do this on tblTransactions so we know what which tblTransactions records are being processed.
        -- Synchronization will only synchronize other dependent records that are associated with the tblTransactions records
        -- that were included.
    IF (@sync_request_type = 4) -- This replaced sync_initialized since we can't control it when performing batch synchronization
    BEGIN
        INSERT INTO #SyncTable 
            SELECT TOP(@sync_batch_size_tblTransactions) WITH TIES [dbo].[tblTransactions].[TransactionGuid] AS 'PK', 'I' AS 'ChangeType' 
                FROM [dbo].[tblTransactions]
                    INNER JOIN [track].[tblTransactions] CT
                        ON CT.PK_TransactionGuid = [dbo].[tblTransactions].[TransactionGuid]
                WHERE ( [dbo].[tblTransactions].[SiteGuid] = @sync_context_site_guid)
                    AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary)   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    AND (CT.DeletedRowVersion IS NULL))
                ORDER BY CT.InsertedRowVersion ASC
    END
    ELSE
    BEGIN
        INSERT INTO #SyncTable 
            SELECT TOP(@sync_batch_size_tblTransactions) WITH TIES [dbo].[tblTransactions].[TransactionGuid] AS 'PK', 'I' AS 'ChangeType' 
                FROM [dbo].[tblTransactions]
                    INNER JOIN [track].[tblTransactions] CT
                        ON CT.PK_TransactionGuid = [dbo].[tblTransactions].[TransactionGuid]
                WHERE ( [dbo].[tblTransactions].[SiteGuid] = @sync_context_site_guid)
                    AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary)   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    AND (CT.DeletedRowVersion IS NULL))
                ORDER BY CT.InsertedRowVersion ASC
        INSERT INTO #SyncTable
            SELECT TOP(@sync_batch_size_tblTransactions) WITH TIES CT.PK_TransactionGuid AS 'PK', 'D' AS 'ChangeType' 
                FROM [track].[tblTransactions] CT
                WHERE (CT.CurrentSiteGuid = @sync_context_site_guid)
                    AND ((CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.DeletedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.DeletedContext IS NULL OR CT.DeletedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY CT.DeletedRowVersion ASC
        INSERT INTO #SyncTable
            SELECT TOP(@sync_batch_size_tblTransactions) WITH TIES [dbo].[tblTransactions].[TransactionGuid] AS 'PK', 'U' AS 'ChangeType' 
                FROM [dbo].[tblTransactions]
                    INNER JOIN [track].[tblTransactions] CT
                        ON CT.PK_TransactionGuid = [dbo].[tblTransactions].[TransactionGuid]
                WHERE ( [dbo].[tblTransactions].[SiteGuid] = @sync_context_site_guid)
                    AND ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                    AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary)   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    AND (CT.DeletedRowVersion IS NULL)
                    AND [dbo].[tblTransactions].[TransactionGuid] NOT IN (SELECT PK FROM #SyncTable))
                ORDER BY CT.UpdatedRowVersion ASC
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblTransactions) WITH TIES [dbo].[tblTransactions].[TransID],[dbo].[tblTransactions].[AliasName],[dbo].[tblTransactions].[SubType],[dbo].[tblTransactions].[Site],[dbo].[tblTransactions].[TransReferenceID],CONVERT(CHAR(10), [dbo].[tblTransactions].[InventoryDate], 111) AS [InventoryDate],[dbo].[tblTransactions].[ShipToID],[dbo].[tblTransactions].[ShipToCode],[dbo].[tblTransactions].[SupplierID],[dbo].[tblTransactions].[SupplierCode],[dbo].[tblTransactions].[CreatedDate],[dbo].[tblTransactions].[CreatedBy],[dbo].[tblTransactions].[RequestedDeliveryDate],[dbo].[tblTransactions].[UpdatedDate],[dbo].[tblTransactions].[UpdatedBy],[dbo].[tblTransactions].[TransDateTime],[dbo].[tblTransactions].[TransVersion],[dbo].[tblTransactions].[SCACCode],[dbo].[tblTransactions].[CardNumber],[dbo].[tblTransactions].[ShipmentNumber],[dbo].[tblTransactions].[ShipperID],[dbo].[tblTransactions].[ShipperCode],[dbo].[tblTransactions].[OwnerID],[dbo].[tblTransactions].[OwnerCode],[dbo].[tblTransactions].[ManagerID],[dbo].[tblTransactions].[ManagerCode],[dbo].[tblTransactions].[CarrierID],[dbo].[tblTransactions].[CarrierCode],[dbo].[tblTransactions].[ConjoinTransID],[dbo].[tblTransactions].[ReversedTransID],[dbo].[tblTransactions].[LinkedDocumentNumber],[dbo].[tblTransactions].[ReversalType],[dbo].[tblTransactions].[PONumber],[dbo].[tblTransactions].[TimeIn],[dbo].[tblTransactions].[TimeOut],[dbo].[tblTransactions].[TimeEnd],[dbo].[tblTransactions].[RoutingID],[dbo].[tblTransactions].[TicketSource],[dbo].[tblTransactions].[LoadID],[dbo].[tblTransactions].[BillToID],[dbo].[tblTransactions].[BillToCode],[dbo].[tblTransactions].[DriverIdentificationNumber],[dbo].[tblTransactions].[CreditAmount],[dbo].[tblTransactions].[CardExpiration],[dbo].[tblTransactions].[CardName],[dbo].[tblTransactions].[CardType],[dbo].[tblTransactions].[CashAmount],[dbo].[tblTransactions].[RouteOriginationDate],[dbo].[tblTransactions].[InternationalRouteIndicator],[dbo].[tblTransactions].[PreviousRoutingID],[dbo].[tblTransactions].[ShippingDocumentNumber],[dbo].[tblTransactions].[DocumentNumber],[dbo].[tblTransactions].[STD],[dbo].[tblTransactions].[ETD],[dbo].[tblTransactions].[STA],[dbo].[tblTransactions].[ETA],[dbo].[tblTransactions].[SFT],[dbo].[tblTransactions].[FST],[dbo].[tblTransactions].[EstimatedFuelingDuration],[dbo].[tblTransactions].[DeleteFlag],[dbo].[tblTransactions].[TicketMode],[dbo].[tblTransactions].[DestinationRegistrationID1],[dbo].[tblTransactions].[DestinationSerialNumber1],[dbo].[tblTransactions].[DestinationEquipmentType1],[dbo].[tblTransactions].[DestinationEquipmentModel1],[dbo].[tblTransactions].[DestinationCompanyEquipmentID1],[dbo].[tblTransactions].[DestinationRegistrationID2],[dbo].[tblTransactions].[DestinationSerialNumber2],[dbo].[tblTransactions].[DestinationEquipmentType2],[dbo].[tblTransactions].[DestinationEquipmentModel2],[dbo].[tblTransactions].[DestinationCompanyEquipmentID2],[dbo].[tblTransactions].[DestinationRegistrationID3],[dbo].[tblTransactions].[DestinationSerialNumber3],[dbo].[tblTransactions].[DestinationEquipmentType3],[dbo].[tblTransactions].[DestinationEquipmentModel3],[dbo].[tblTransactions].[DestinationCompanyEquipmentID3],[dbo].[tblTransactions].[SourceRegistrationID1],[dbo].[tblTransactions].[SourceSerialNumber1],[dbo].[tblTransactions].[SourceEquipmentType1],[dbo].[tblTransactions].[SourceEquipmentModel1],[dbo].[tblTransactions].[SourceCompanyEquipmentID1],[dbo].[tblTransactions].[SourceRegistrationID2],[dbo].[tblTransactions].[SourceSerialNumber2],[dbo].[tblTransactions].[SourceEquipmentType2],[dbo].[tblTransactions].[SourceEquipmentModel2],[dbo].[tblTransactions].[SourceCompanyEquipmentID2],[dbo].[tblTransactions].[SourceRegistrationID3],[dbo].[tblTransactions].[SourceSerialNumber3],[dbo].[tblTransactions].[SourceEquipmentType3],[dbo].[tblTransactions].[SourceEquipmentModel3],[dbo].[tblTransactions].[SourceCompanyEquipmentID3],[dbo].[tblTransactions].[OperatorID],[dbo].[tblTransactions].[EffectiveDate],[dbo].[tblTransactions].[ExpirationDate],[dbo].[tblTransactions].[ScheduledDate],[dbo].[tblTransactions].[AutoComplete],[dbo].[tblTransactions].[Flag01],[dbo].[tblTransactions].[Flag02],[dbo].[tblTransactions].[Flag03],[dbo].[tblTransactions].[Flag04],[dbo].[tblTransactions].[Flag05],[dbo].[tblTransactions].[Flag06],[dbo].[tblTransactions].[Number01],[dbo].[tblTransactions].[Number02],[dbo].[tblTransactions].[Number03],[dbo].[tblTransactions].[Number04],[dbo].[tblTransactions].[Number05],[dbo].[tblTransactions].[Number06],[dbo].[tblTransactions].[ContactFirstName],[dbo].[tblTransactions].[ContactSurname],[dbo].[tblTransactions].[Date01],[dbo].[tblTransactions].[Date02],[dbo].[tblTransactions].[Date03],[dbo].[tblTransactions].[Date04],[dbo].[tblTransactions].[LegacyNumber],[dbo].[tblTransactions].[Country],[dbo].[tblTransactions].[ContactInfo],[dbo].[tblTransactions].[AssociatedDocNumber],[dbo].[tblTransactions].[AssociatedCLIN],[dbo].[tblTransactions].[SubmittedToAccounting],[dbo].[tblTransactions].[FuelCardID],[dbo].[tblTransactions].[AssociatedTransportOrderNumber],[dbo].[tblTransactions].[RequestedDateTime],[dbo].[tblTransactions].[DispatchedDateTime],[dbo].[tblTransactions].[ErrorFlag],[dbo].[tblTransactions].[TransactionGuid],[dbo].[tblTransactions].[SiteGuid],[dbo].[tblTransactions].[LookupTransTypeIndex],[dbo].[tblTransactions].[LookupTransactionStatusIndex],[dbo].[tblTransactions].[LookupOriginApplicationIndex],[dbo].[tblTransactions].[TransactionAliasGuid],[dbo].[tblTransactions].[BillToCompanyGuid],[dbo].[tblTransactions].[Destination1EquipmentGuid],[dbo].[tblTransactions].[Destination2EquipmentGuid],[dbo].[tblTransactions].[Destination3EquipmentGuid],[dbo].[tblTransactions].[FinalStationIATAGuid],[dbo].[tblTransactions].[FuelCardGuid],[dbo].[tblTransactions].[ManagerCompanyGuid],[dbo].[tblTransactions].[NextStationIATAGuid],[dbo].[tblTransactions].[OperatorPersonnelGuid],[dbo].[tblTransactions].[OriginStationIATAGuid],[dbo].[tblTransactions].[OwnerCompanyGuid],[dbo].[tblTransactions].[PreviousStationIATAGuid],[dbo].[tblTransactions].[ShipperCompanyGuid],[dbo].[tblTransactions].[ShipToCompanyGuid],[dbo].[tblTransactions].[Source1EquipmentGuid],[dbo].[tblTransactions].[Source2EquipmentGuid],[dbo].[tblTransactions].[Source3EquipmentGuid],[dbo].[tblTransactions].[SupplierCompanyGuid],[dbo].[tblTransactions].[CarrierCompanyGuid],[dbo].[tblTransactions].[ReasonCodeGuid],[dbo].[tblTransactions].[OriginStationIATAID],[dbo].[tblTransactions].[PreviousStationIATAID],[dbo].[tblTransactions].[NextStationIATAID],[dbo].[tblTransactions].[FinalStationIATAID],[dbo].[tblTransactions].[OperatorName],[dbo].[tblTransactions].[FuelAdditiveFlag],[dbo].[tblTransactions].[IssuePoint],[dbo].[tblTransactions].[IssuePointNumber],[dbo].[tblTransactions].[RadioNumber],[dbo].[tblTransactions].[GateID],[dbo].[tblTransactions].[GateGuid],[dbo].[tblTransactions].[ShippingMethod],[dbo].[tblTransactions].[ReferencedTransactionGuid],CT.InsertedRowVersion AS '_RowVersion'
            FROM [dbo].[tblTransactions]
                INNER JOIN #SyncTable ON #SyncTable.PK = [dbo].[tblTransactions].[TransactionGuid]
                INNER JOIN [track].[tblTransactions] CT
                    ON CT.PK_TransactionGuid = [dbo].[tblTransactions].[TransactionGuid] 
            WHERE (#SyncTable.ChangeType = 'I')
                AND (CT.DeletedRowVersion IS NULL)
                AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
