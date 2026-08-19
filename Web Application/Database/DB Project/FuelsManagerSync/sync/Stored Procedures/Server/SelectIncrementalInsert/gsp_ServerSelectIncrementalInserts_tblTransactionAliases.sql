-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliases
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblTransactionAliases]
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
@sync_batch_size_tblTransactionAliases int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblTransactionAliases int
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblTransactionAliases IS NOT NULL AND @sync_first_time_sync_option_tblTransactionAliases = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblTransactionAliases].[AliasName],[dbo].[tblTransactionAliases].[MeterCloseout],[dbo].[tblTransactionAliases].[BulkShipment],[dbo].[tblTransactionAliases].[DistributedImpact],[dbo].[tblTransactionAliases].[MultipleLineItems],[dbo].[tblTransactionAliases].[LimitSelectionsBasedOnHierarchy],[dbo].[tblTransactionAliases].[LineItemEditControl],[dbo].[tblTransactionAliases].[MultipleWeightReadings],[dbo].[tblTransactionAliases].[WeightReadingEditControl],[dbo].[tblTransactionAliases].[AssociatedReport],[dbo].[tblTransactionAliases].[AssociatedPreloadReport],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes1],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes2],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes3],[dbo].[tblTransactionAliases].[SourceEquipmentTypes1],[dbo].[tblTransactionAliases].[SourceEquipmentTypes2],[dbo].[tblTransactionAliases].[SourceEquipmentTypes3],[dbo].[tblTransactionAliases].[CreatedDate],[dbo].[tblTransactionAliases].[CreatedBy],[dbo].[tblTransactionAliases].[UpdatedDate],[dbo].[tblTransactionAliases].[UpdatedBy],[dbo].[tblTransactionAliases].[ShowCompanyName],[dbo].[tblTransactionAliases].[AggregateAssocTrans],[dbo].[tblTransactionAliases].[EnableTotalQuantityExceededWarning],[dbo].[tblTransactionAliases].[EnableQuantityToleranceExceededWarning],[dbo].[tblTransactionAliases].[EnableTotalValueExceededWarning],[dbo].[tblTransactionAliases].[EnableValueToleranceExceededWarning],[dbo].[tblTransactionAliases].[LevelUnitIndex],[dbo].[tblTransactionAliases].[TemperatureUnitIndex],[dbo].[tblTransactionAliases].[DensityUnitIndex],[dbo].[tblTransactionAliases].[PressureUnitIndex],[dbo].[tblTransactionAliases].[FlowUnitIndex],[dbo].[tblTransactionAliases].[VolumeUnitIndex],[dbo].[tblTransactionAliases].[MassUnitIndex],[dbo].[tblTransactionAliases].[AdditiveVolumeUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileCycleAmountUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileRateUnitIndex],[dbo].[tblTransactionAliases].[LevelDecimalPlaces],[dbo].[tblTransactionAliases].[TemperatureDecimalPlaces],[dbo].[tblTransactionAliases].[DensityDecimalPlaces],[dbo].[tblTransactionAliases].[PressureDecimalPlaces],[dbo].[tblTransactionAliases].[FlowDecimalPlaces],[dbo].[tblTransactionAliases].[VolumeDecimalPlaces],[dbo].[tblTransactionAliases].[MassDecimalPlaces],[dbo].[tblTransactionAliases].[AdditiveVolumeDecimalPlaces],[dbo].[tblTransactionAliases].[UseComboBoxControls],[dbo].[tblTransactionAliases].[MultipleTransportLineItems],[dbo].[tblTransactionAliases].[TransactionAliasGuid],[dbo].[tblTransactionAliases].[SiteGuid],[dbo].[tblTransactionAliases].[LookupTransTypeIndex],[dbo].[tblTransactionAliases].[LookupDefaultStatusIndex],[dbo].[tblTransactionAliases].[AssociatedTransactionAliasGuid],[dbo].[tblTransactionAliases].[IncludeInDispatch],[dbo].[tblTransactionAliases].[_MasterRecordGuid],[dbo].[tblTransactionAliases].[EnableAutoCompleteControls],[dbo].[tblTransactionAliases].[PermitNonReferenceData],[dbo].[tblTransactionAliases].[UseTransactionDetailWithLayout],[dbo].[tblTransactionAliases].[DefaultMeterToEquipmentID],[dbo].[tblTransactionAliases].[LimitSourceEquipmentByProduct],[dbo].[tblTransactionAliases].[RememberMeterEndForMeterID],[dbo].[tblTransactionAliases].[PopulateCompaniesFromEquipment],[dbo].[tblTransactionAliases].[PopulateGrossVolumeFromMeterValues],[dbo].[tblTransactionAliases].[UseMeterAndCompressionFactorFromMeter], [dbo].[tblTransactionAliases].[_RowVersion]
            FROM [dbo].[tblTransactionAliases]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTransactionAliases IS NULL OR 
        (@sync_batch_size_tblTransactionAliases IS NOT NULL AND @sync_batch_size_tblTransactionAliases = 0))
    BEGIN
        SET @sync_batch_size_tblTransactionAliases = 2147483647;
    END


    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT [AliasName],[MeterCloseout],[BulkShipment],[DistributedImpact],[MultipleLineItems],[LimitSelectionsBasedOnHierarchy],[LineItemEditControl],[MultipleWeightReadings],[WeightReadingEditControl],[AssociatedReport],[AssociatedPreloadReport],[DestinationEquipmentTypes1],[DestinationEquipmentTypes2],[DestinationEquipmentTypes3],[SourceEquipmentTypes1],[SourceEquipmentTypes2],[SourceEquipmentTypes3],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ShowCompanyName],[AggregateAssocTrans],[EnableTotalQuantityExceededWarning],[EnableQuantityToleranceExceededWarning],[EnableTotalValueExceededWarning],[EnableValueToleranceExceededWarning],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[UseComboBoxControls],[MultipleTransportLineItems],[TransactionAliasGuid],[SiteGuid],[LookupTransTypeIndex],[LookupDefaultStatusIndex],[AssociatedTransactionAliasGuid],[IncludeInDispatch],[_MasterRecordGuid],[EnableAutoCompleteControls],[PermitNonReferenceData],[UseTransactionDetailWithLayout],[DefaultMeterToEquipmentID],[LimitSourceEquipmentByProduct],[RememberMeterEndForMeterID],[PopulateCompaniesFromEquipment],[PopulateGrossVolumeFromMeterValues],[UseMeterAndCompressionFactorFromMeter],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblTransactionAliases) WITH TIES [dbo].[tblTransactionAliases].[AliasName],[dbo].[tblTransactionAliases].[MeterCloseout],[dbo].[tblTransactionAliases].[BulkShipment],[dbo].[tblTransactionAliases].[DistributedImpact],[dbo].[tblTransactionAliases].[MultipleLineItems],[dbo].[tblTransactionAliases].[LimitSelectionsBasedOnHierarchy],[dbo].[tblTransactionAliases].[LineItemEditControl],[dbo].[tblTransactionAliases].[MultipleWeightReadings],[dbo].[tblTransactionAliases].[WeightReadingEditControl],[dbo].[tblTransactionAliases].[AssociatedReport],[dbo].[tblTransactionAliases].[AssociatedPreloadReport],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes1],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes2],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes3],[dbo].[tblTransactionAliases].[SourceEquipmentTypes1],[dbo].[tblTransactionAliases].[SourceEquipmentTypes2],[dbo].[tblTransactionAliases].[SourceEquipmentTypes3],[dbo].[tblTransactionAliases].[CreatedDate],[dbo].[tblTransactionAliases].[CreatedBy],[dbo].[tblTransactionAliases].[UpdatedDate],[dbo].[tblTransactionAliases].[UpdatedBy],[dbo].[tblTransactionAliases].[ShowCompanyName],[dbo].[tblTransactionAliases].[AggregateAssocTrans],[dbo].[tblTransactionAliases].[EnableTotalQuantityExceededWarning],[dbo].[tblTransactionAliases].[EnableQuantityToleranceExceededWarning],[dbo].[tblTransactionAliases].[EnableTotalValueExceededWarning],[dbo].[tblTransactionAliases].[EnableValueToleranceExceededWarning],[dbo].[tblTransactionAliases].[LevelUnitIndex],[dbo].[tblTransactionAliases].[TemperatureUnitIndex],[dbo].[tblTransactionAliases].[DensityUnitIndex],[dbo].[tblTransactionAliases].[PressureUnitIndex],[dbo].[tblTransactionAliases].[FlowUnitIndex],[dbo].[tblTransactionAliases].[VolumeUnitIndex],[dbo].[tblTransactionAliases].[MassUnitIndex],[dbo].[tblTransactionAliases].[AdditiveVolumeUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileCycleAmountUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileRateUnitIndex],[dbo].[tblTransactionAliases].[LevelDecimalPlaces],[dbo].[tblTransactionAliases].[TemperatureDecimalPlaces],[dbo].[tblTransactionAliases].[DensityDecimalPlaces],[dbo].[tblTransactionAliases].[PressureDecimalPlaces],[dbo].[tblTransactionAliases].[FlowDecimalPlaces],[dbo].[tblTransactionAliases].[VolumeDecimalPlaces],[dbo].[tblTransactionAliases].[MassDecimalPlaces],[dbo].[tblTransactionAliases].[AdditiveVolumeDecimalPlaces],[dbo].[tblTransactionAliases].[UseComboBoxControls],[dbo].[tblTransactionAliases].[MultipleTransportLineItems],[dbo].[tblTransactionAliases].[TransactionAliasGuid],[dbo].[tblTransactionAliases].[SiteGuid],[dbo].[tblTransactionAliases].[LookupTransTypeIndex],[dbo].[tblTransactionAliases].[LookupDefaultStatusIndex],[dbo].[tblTransactionAliases].[AssociatedTransactionAliasGuid],[dbo].[tblTransactionAliases].[IncludeInDispatch],[dbo].[tblTransactionAliases].[_MasterRecordGuid],[dbo].[tblTransactionAliases].[EnableAutoCompleteControls],[dbo].[tblTransactionAliases].[PermitNonReferenceData],[dbo].[tblTransactionAliases].[UseTransactionDetailWithLayout],[dbo].[tblTransactionAliases].[DefaultMeterToEquipmentID],[dbo].[tblTransactionAliases].[LimitSourceEquipmentByProduct],[dbo].[tblTransactionAliases].[RememberMeterEndForMeterID],[dbo].[tblTransactionAliases].[PopulateCompaniesFromEquipment],[dbo].[tblTransactionAliases].[PopulateGrossVolumeFromMeterValues],[dbo].[tblTransactionAliases].[UseMeterAndCompressionFactorFromMeter],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblTransactionAliases]
                    INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblTransactionAliases].[TransactionAliasGuid] = data.[TransactionAliasGuid]
                    INNER JOIN [track].[tblTransactionAliases] CT
                        ON CT.PK_TransactionAliasGuid = [dbo].[tblTransactionAliases].[TransactionAliasGuid] 
                    INNER JOIN [track].[tblEntityTransactionAliasToSite] MAPCT
                        ON MAPCT.PK_TransactionAliasToSiteGuid = data.[TransactionAliasToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
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
