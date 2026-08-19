/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionHeaders]

	EXEC [staging].[usp_LoadTransactionHeaders] 0, 0

	EXEC [staging].[usp_LoadTransactionHeaders] 0, 200000
	
	EXEC [staging].[usp_LoadTransactionHeaders] 200000, 0
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionHeaders]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadTransactionHeaders]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction Header records from staging into the FactTransaction table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. The Transaction Header Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency to a 
  --    level 2 table.
  -- 5. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 6. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 7. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 8. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  -- 9. Transaction deletions in the OLTP database, whether soft deletions (DeleteFlag = 1) or physical deletions are translated in the 
  --    OLAP database as soft deletions.
  -- 10. For headers for which LineItems / SubLineItems already exist or are added/modified at the same time that the header is changed,
  --	the LineItems and SubLineItems for modified headers are pulled, refreshed, and merged with the modified header. In that case the 
  --	header modification is simply processed at the same time as the LineItem and SubLineItem records are loaded in FactTransaction
  --    and does not need to be re-processed here. Ignoring those header records in the routine below also helps ensure that FactTransaction 
  --    fields that can be set by both LineItem/SubLineItem and Header (e.g. FactTransaction.DeleteFlag), and which have already been
  --    set through the staging.transactionLineItems /staging.transactionSubLineItems are not overriden by the staging.tblTransactions.
  --	The operation below handles the following cases:
  --	a. The case where ONLY the header portion of an existing transaction has been inserted. Transactions created with only a 
  --		Transaction Header, i.e transactions for which the Line Items have not yet been added, still need to be added to the data 
  --        warehouse, because by the time the LineItems are created, the CDC will not necessarily carry the Transaction Headers.
  --	b. The case where a Transaction does not have LineItems, e.g EndOfDay transactions. 
  --		This Merge is applied after the LineItems and SubLineItems are loaded so as not to unnecessarily process records coming from 
  --		LineItems or SubLineItems CDC entries, which would have already been processed and stamped with the latest _RecordUpdatedDate 
  --		value in the earlier Merge operations.
  -- 11. This operation excludes conjoin transactions that have been extracted from FactTransaction itself. Those transactions are updated 
  --    in a separate operation where only the relevant conjoin-sensitive fields are updated.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'
    DECLARE @veryShortDummyId varchar(2) = 'NA'

    DECLARE @dummyDateSKey int = 19000101
    DECLARE @dummyDateTime datetimeoffset(7) = '1/1/1900'
    DECLARE @defaultTimeSKey int = 0
    DECLARE @defaultBitValue bit = 0

    IF ((SELECT
        COUNT(*)
      FROM staging.tblTransactions
      WHERE IsProcessed = 0)
      = 0)
    BEGIN
      RETURN
    END


	TRUNCATE TABLE staging.tblInsertedRecordsTemp
	TRUNCATE TABLE staging.tblUpdatedRecordsTemp


	INSERT INTO staging.tblUpdatedRecordsTemp ([RecordKey])
	SELECT
	src.[TransactionKey]
	FROM staging.tblTransactions src
    WHERE src.TransactionKey IS NOT NULL
    AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
    AND --Ignore Conjoin Transaction records that have been artificially added by the ETL. Those are updated separately from the Merge.
    (
		(src.ConjoinTransID IS NULL)
		OR 
		(
			(src.ConjoinTransID IS NOT NULL)
			AND (src.IsRecordAddedByETL = 0)
			AND (src.SourceFactSKey IS NULL)
		)
    )

	INSERT INTO staging.tblInsertedRecordsTemp ([RecordKey])
	SELECT
	src.[RecordKey]
	FROM staging.tblUpdatedRecordsTemp src
    WHERE NOT EXISTS
	(
		SELECT * FROM dbo.FactTransaction tgt
		WHERE tgt.TransactionKey = src.RecordKey
	)

	DELETE a 
	FROM staging.tblUpdatedRecordsTemp a
	WHERE EXISTS
	(
		SELECT * FROM staging.tblInsertedRecordsTemp ins
		WHERE ins.RecordKey = a.RecordKey
	)
	
	INSERT INTO dbo.FactTransaction
	(                            
		[BillToCompanySKey],
		[CarrierCompanySKey],
		[ConjoinOwnerSKey],
		[ConjoinTransID],
		[CreatedDate],
		[CreatedDateSKey],
		[CreatedTimeSKey],
		[Date01DateSKey],
		[Date01TimeSKey],
		[DeleteFlag],
		[DestinationEquipment1SKey],
		[DocumentNumber],
		[InternationalRouteIndicator],
		[InventoryDateSKey],
		[Line_ConjoinProductSKey],
		[Line_Density],
		[Line_DestinationEquipmentSKey],
		[Line_GrossQuantitySI],
		[Line_GrossQuantityUSGallon],
		[Line_LoadArmSKey],
		[Line_MeterID],		
		[Line_MeterStart],
		[Line_MeterStartDateTime],
		[Line_MeterStop],
		[Line_MeterStopDateTime],	
		[Line_MeterStartStopTimeDiff],
		[Line_NetQuantitySI],
		[Line_NetQuantityUSGallon],
		[Line_NetVolumeIndicator],
		[Line_ProductSKey],
		[Line_SequenceID],
		[Line_SourceEquipmentSKey],
		[Line_StationSKey],
		[Line_StorageLocationTankSKey],
		[Line_Temperature],
		[Line_Vcf],
		[LineUData_UserData1],
		[ManagerCompanySKey],
		[Number01],
		[OperatorPersonnelSKey],
		[OwnerCompanySKey],
		[ReasonCodeSKey],
		[ReversalType],
		[ReversedTransID],
		[RoutingID],
		[ShipperCompanySKey],
		[ShipToCompanySKey],
		[SiteSKey],
		[SourceEquipment1SKey],
		[SubType],
		[SupplierCompanySKey],
		[TimeIn],	
		[TimeInDateSKey],
		[TimeInTimeSKey],
		[TimeOut],
		[TimeOutDateSKey],
		[TimeOutTimeSKey],
		[TransactionAliasSKey],
		[TransactionAttributesSKey],
		[TransactionKey],
		[TransactionStatusIndex],
		[TransactionStatusName],
		[TransactionTypeSKey],
		[TransDateTime],
		[TransDateSKey],
		[TransTimeSKey],
		[TransID],
		[TransVersion],
		[UData_UserData2],
		[UData_UserData23],
		[UData_UserData3],
		[UData_UserData4SI],
		[UData_UserData4USGallon],
		[UData_UserData5SI],
		[UData_UserData5USGallon],
		[UData_UserData6SI],
		[UData_UserData6USGallon],
		[TransactionLineItemKey],
		[TransactionLineItemUserDataKey],
		[TransactionSubLineItemKey],
		[TransactionUserDataKey],
		[_IsRecordDeleted],
		[_RecordUpdatedDate],
		[_RecordUpdatedDateSKey]
	)
	SELECT 
		ISNULL(src.[BillToCompanySKey], 0),
		ISNULL(src.[CarrierCompanySKey], 0),
		ISNULL(src.[ConjoinOwnerSKey], 0),
		src.[ConjoinTransID],
		src.[CreatedDate],
		ISNULL(src.[CreatedDateSKey], @dummyDateSKey),
		ISNULL(src.[CreatedTimeSKey], @defaultTimeSKey),
		ISNULL(src.[Date01DateSKey], @dummyDateSKey),
		ISNULL(src.[Date01TimeSKey], @defaultTimeSKey),
		CASE WHEN (src.[DeleteFlag] = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END [DeleteFlag],
		ISNULL(src.[DestinationEquipment1SKey], 0),
		src.[DocumentNumber],
		ISNULL(src.[InternationalRouteIndicator], 0),
		ISNULL(src.[InventoryDateSKey], @dummyDateSKey),
		0 [Line_ConjoinProductSKey],
		0 [Line_Density],
		0 [Line_DestinationEquipmentSKey],
		0 [Line_GrossQuantitySI],
		0 [Line_GrossQuantityUSGallon],
		0 [Line_LoadArmSKey],
		NULL [Line_MeterID],
		0 [Line_MeterStart],
		NULL [Line_MeterStartDateTime],				
		0 [Line_MeterStop],
		NULL [Line_MeterStopDateTime],
		0 [Line_MeterStartStopTimeDiff],
		0 [Line_NetQuantitySI],
		0 [Line_NetQuantityUSGallon],
		0 [Line_NetVolumeIndicator],
		0 [Line_ProductSKey],
		0 [Line_SequenceID],
		0 [Line_SourceEquipmentSKey],
		0 [Line_StationSKey],
		0 [Line_StorageLocationTankSKey],
		0 [Line_Temperature],
		0 [Line_Vcf],
		NULL [LineUData_UserData1],
		ISNULL(src.[ManagerCompanySKey], 0),
		ISNULL(src.[Number01], 0),
		ISNULL(src.[OperatorPersonnelSKey], 0),
		ISNULL(src.[OwnerCompanySKey], 0),
		ISNULL(src.[ReasonCodeSKey], 0),
		ISNULL(src.[ReversalType], @veryShortDummyId),
		src.[ReversedTransID],
		src.[RoutingID],
		ISNULL(src.[ShipperCompanySKey], 0),
		ISNULL(src.[ShipToCompanySKey], 0),
		ISNULL(src.[SiteSKey], 0),
		ISNULL(src.[SourceEquipment1SKey], 0),
		ISNULL(src.[SubType], @dummyId),
		ISNULL(src.[SupplierCompanySKey], 0),
		src.[TimeIn],
		ISNULL(src.[TimeInDateSKey], @dummyDateSKey),
		ISNULL(src.[TimeInTimeSKey], 0),
		src.[TimeOut],
		ISNULL(src.[TimeOutDateSKey], @dummyDateSKey),
		ISNULL(src.[TimeOutTimeSKey], 0),
		ISNULL(src.[TransactionAliasSKey], 0),
		0 [TransactionAttributesSKey],
		src.[TransactionKey],
		src.[TransactionStatusIndex],
		ISNULL(src.[TransactionStatusName], @dummyId),
		ISNULL(src.[TransactionTypeSKey], 0),
		src.[TransDateTime],
		ISNULL(src.[TransDateSKey], @dummyDateSKey),
		ISNULL(src.[TransTimeSKey], 0),
		src.[TransID],
		ISNULL(src.[TransVersion], 0),
		NULL [UData_UserData2],
		NULL [UData_UserData23],
		NULL [UData_UserData3],
		0 [UData_UserData4SI],
		0 [UData_UserData4USGallon],
		0 [UData_UserData5SI],
		0 [UData_UserData5USGallon],
		0 [UData_UserData6SI],
		0 [UData_UserData6USGallon],

		NULL [TransactionLineItemKey],
		NULL [TransactionLineItemUserDataKey],
		NULL [TransactionSubLineItemKey],
		NULL [TransactionUserDataKey],

		ISNULL(src.[IsRecordDeleted], 0),
		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)
	FROM staging.tblTransactions src
	INNER JOIN staging.tblInsertedRecordsTemp b
	ON b.RecordKey = src.TransactionKey
	WHERE src.TransactionKey IS NOT NULL
	AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0



    UPDATE tgt
	SET tgt.[BillToCompanySKey] = ISNULL(src.[BillToCompanySKey], 0),
		tgt.[CarrierCompanySKey] = ISNULL(src.[CarrierCompanySKey], 0),
		tgt.[ConjoinOwnerSKey] = ISNULL(src.[ConjoinOwnerSKey], 0),
		tgt.[ConjoinTransID] = src.[ConjoinTransID],
		tgt.[CreatedDate] = src.[CreatedDate],
		tgt.[CreatedDateSKey] = ISNULL(src.[CreatedDateSKey], @dummyDateSKey),
		tgt.[CreatedTimeSKey] = ISNULL(src.[CreatedTimeSKey], @defaultTimeSKey),
		tgt.[Date01DateSKey] = ISNULL(src.[Date01DateSKey], @dummyDateSKey),
		tgt.[Date01TimeSKey] = ISNULL(src.[Date01TimeSKey], @defaultTimeSKey),
		tgt.[DeleteFlag] = CASE WHEN (src.[DeleteFlag] = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END,
		tgt.[DestinationEquipment1SKey] = ISNULL(src.[DestinationEquipment1SKey], 0),
		tgt.[DocumentNumber] = src.[DocumentNumber],
		tgt.[InternationalRouteIndicator] = ISNULL(src.[InternationalRouteIndicator], 0),
		tgt.[InventoryDateSKey] = ISNULL(src.[InventoryDateSKey], @dummyDateSKey),
		tgt.[ManagerCompanySKey] = ISNULL(src.[ManagerCompanySKey], 0),
		tgt.[Number01] = ISNULL(src.[Number01], 0),
		tgt.[OperatorPersonnelSKey] = ISNULL(src.[OperatorPersonnelSKey], 0),
		tgt.[OwnerCompanySKey] = ISNULL(src.[OwnerCompanySKey], 0),
		tgt.[ReasonCodeSKey] = ISNULL(src.[ReasonCodeSKey], 0),
		tgt.[ReversalType] = ISNULL(src.[ReversalType], @veryShortDummyId),
		tgt.[ReversedTransID] = src.[ReversedTransID],
		tgt.[RoutingID] = src.[RoutingID],
		tgt.[ShipperCompanySKey] = ISNULL(src.[ShipperCompanySKey], 0),
		tgt.[ShipToCompanySKey] = ISNULL(src.[ShipToCompanySKey], 0),
		tgt.[SiteSKey] = ISNULL(src.[SiteSKey], 0),
		tgt.[SourceEquipment1SKey] = ISNULL(src.[SourceEquipment1SKey], 0),
		tgt.[SubType] = ISNULL(src.[SubType], @dummyId),
		tgt.[SupplierCompanySKey] = ISNULL(src.[SupplierCompanySKey], 0),
		tgt.[TimeIn] = src.[TimeIn],
		tgt.[TimeInDateSKey] = ISNULL(src.[TimeInDateSKey], @dummyDateSKey),
		tgt.[TimeInTimeSKey] = ISNULL(src.[TimeInTimeSKey], 0),
		tgt.[TimeOut] = src.[TimeOut],
		tgt.[TimeOutDateSKey] = ISNULL(src.[TimeOutDateSKey], @dummyDateSKey),
		tgt.[TimeOutTimeSKey] = ISNULL(src.[TimeOutTimeSKey], 0),
		tgt.[TransactionAliasSKey] = ISNULL(src.[TransactionAliasSKey], 0),
		tgt.[TransactionKey] = src.[TransactionKey],
		tgt.[TransactionStatusIndex] = src.[TransactionStatusIndex],
		tgt.[TransactionStatusName] = ISNULL(src.[TransactionStatusName], @dummyId),
		tgt.[TransactionTypeSKey] = ISNULL(src.[TransactionTypeSKey], 0),
		tgt.[TransDateTime] = src.[TransDateTime],
		tgt.[TransDateSKey] = ISNULL(src.[TransDateSKey], @dummyDateSKey),
		tgt.[TransTimeSKey] = ISNULL(src.[TransTimeSKey], 0),
		tgt.[TransID] = src.[TransID],
		tgt.[TransVersion] = ISNULL(src.[TransVersion], 0),

		tgt.[_IsRecordDeleted] = ISNULL(src.[IsRecordDeleted], 0),
		tgt.[_RecordUpdatedDate] = ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		tgt.[_RecordUpdatedDateSKey] = ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactTransaction tgt
	INNER JOIN staging.tblTransactions src
	ON src.TransactionKey = tgt.TransactionKey
	INNER JOIN staging.tblUpdatedRecordsTemp b
	ON b.RecordKey = src.TransactionKey
    WHERE src.TransactionKey IS NOT NULL
    AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
    AND --Ignore Conjoin Transaction records that have been artificially added by the ETL. Those are updated separately from the Merge.
    (
		(src.ConjoinTransID IS NULL)
		OR 
		(
			(src.ConjoinTransID IS NOT NULL)
			AND (src.IsRecordAddedByETL = 0)
			AND (src.SourceFactSKey IS NULL)
		)
    )	
	AND NOT EXISTS  --Ignore header updates for which there is a valid lineItem record in staging (whose own FactTransaction update would cover the header fields already).
	(
		SELECT * FROM staging.tblTransactionLineItems c
		WHERE c.TransactionLineItemKey = tgt.TransactionLineItemKey
		AND c.IgnoreRecord = 0
		AND c.IsRecordAddedByETL = 0
		AND c.CombinedUpdatedDate >= src.CombinedUpdatedDate
	)

  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [staging].[usp_LoadTransactionHeaders]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END