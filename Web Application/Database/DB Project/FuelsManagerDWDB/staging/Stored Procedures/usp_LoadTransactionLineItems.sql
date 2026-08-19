/*
	DROP PROCEDURE [staging].[usp_LoadTransactionLineItems]

	EXEC [staging].[usp_LoadTransactionLineItems] 0, 0

	EXEC [staging].[usp_LoadTransactionLineItems] 0, 200000

	EXEC [staging].[usp_LoadTransactionLineItems] 200000, 0
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionLineItems]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadTransactionLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads LineItem records from staging into the FactTransaction table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionLineItemSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionLineItemSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. The Transaction LineItem Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency to a level 2 table.
  -- 5. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 6. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 7. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 8. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  -- 9. LineItem deletions in the OLTP database, whether soft deletions (DeleteFlag = 1) or physical deletions are translated in the 
  --    OLAP database as soft deletions.
  -- 10. This operation excludes conjoin transactions that have been extracted from FactTransaction itself. Those transactions are updated 
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


	TRUNCATE TABLE staging.tblUpdatedRecordsTemp

	INSERT INTO staging.tblUpdatedRecordsTemp ([RecordKey])
	SELECT
		src.[TransactionLineItemKey]
	FROM staging.tblTransactionLineItems src
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
	AND --Ignore Conjoin Transaction records that have been artificially added by the ETL. Those are updated separately from the Merge.
    (
		(src.HeaderConjoinTransID IS NULL)
		OR 
		(
			(src.HeaderConjoinTransID IS NOT NULL)
			AND (src.IsRecordAddedByETL = 0)
			AND (src.SourceFactSKey IS NULL)
		)
	)


	INSERT INTO staging.tblInsertedLineItems 
	([TransactionLineItemKey], [TransactionKey], [CombinedUpdatedDate], [IsRecordAddedByETL], [IsProcessed])
	SELECT
		src.[TransactionLineItemKey],
		src.[TransactionKey],
		src.[CombinedUpdatedDate],
		src.[IsRecordAddedByETL],
		0
	FROM staging.tblUpdatedRecordsTemp a
	INNER JOIN staging.tblTransactionLineItems src
	ON src.TransactionLineItemKey = a.RecordKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND NOT EXISTS
	(
		SELECT * FROM dbo.FactTransaction tgt
		WHERE tgt.TransactionLineItemKey = src.TransactionLineItemKey
	)


	DELETE a 
	FROM staging.tblUpdatedRecordsTemp a
	WHERE EXISTS
	(
		SELECT * FROM staging.tblInsertedLineItems ins
		WHERE ins.TransactionLineItemKey = a.RecordKey
		AND ins.IsProcessed = 0
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
		[TransactionLineItemKey],
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

		[TransactionLineItemUserDataKey],
		[TransactionUserDataKey],

		[_IsRecordDeleted],
		[_RecordUpdatedDate],
		[_RecordUpdatedDateSKey]
	)
	SELECT 
		ISNULL(src.[HeaderBillToCompanySKey], 0),
		ISNULL(src.[HeaderCarrierCompanySKey], 0),
		ISNULL(src.[HeaderConjoinOwnerSKey], 0),
		src.[HeaderConjoinTransID],
		src.[HeaderCreatedDate],
		ISNULL(src.[HeaderCreatedDateSKey], @dummyDateSKey),
		ISNULL(src.[HeaderCreatedTimeSKey], @defaultTimeSKey),
		ISNULL(src.[HeaderDate01DateSKey], @dummyDateSKey),
		ISNULL(src.[HeaderDate01TimeSKey], @defaultTimeSKey),
		CASE WHEN (src.[DeleteFlag] = 1 OR src.[HeaderDeleteFlag] = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END [DeleteFlag],
		ISNULL(src.[HeaderDestinationEquipment1SKey], 0),
		src.[HeaderDocumentNumber],
		ISNULL(src.[HeaderInternationalRouteIndicator], 0),
		ISNULL(src.[HeaderInventoryDateSKey], @dummyDateSKey),
		ISNULL(src.[ConjoinProductSKey], 0),
		ISNULL(src.[Density], 0),
		ISNULL(src.[DestinationEquipmentSKey], 0),
		ISNULL(src.[GrossQuantitySI], 0),
		ISNULL(src.[GrossQuantityUSGallon], 0),
		ISNULL(src.[LoadArmSKey], 0),
		src.[MeterID],
		ISNULL(src.[MeterStart], 0),
		src.[MeterStartDateTime],
		ISNULL(src.[MeterStop], 0),
		src.[MeterStopDateTime],		
		ISNULL(src.[MeterStartStopTimeDiff], 0),
		ISNULL(src.[NetQuantitySI], 0),
		ISNULL(src.[NetQuantityUSGallon], 0),
		ISNULL(src.[NetVolumeIndicator], 0),
		ISNULL(src.[ProductSKey], 0),
		ISNULL(src.[SequenceID], 0),
		ISNULL(src.[SourceEquipmentSKey], 0),
		ISNULL(src.[LoadingLocationStationSKey], 0),
		ISNULL(src.[StorageLocationTankSKey], 0),
		ISNULL(src.[Temperature], 0),
		ISNULL(src.[Vcf], 0),
		NULL [LineUData_UserData1],
		ISNULL(src.[HeaderManagerCompanySKey], 0),
		ISNULL(src.[HeaderNumber01], 0),
		ISNULL(src.[HeaderOperatorPersonnelSKey], 0),
		ISNULL(src.[HeaderOwnerCompanySKey], 0),
		ISNULL(src.[HeaderReasonCodeSKey], 0),
		ISNULL(src.[HeaderReversalType], @veryShortDummyId),
		src.[HeaderReversedTransID],
		src.[HeaderRoutingID],
		ISNULL(src.[HeaderShipperCompanySKey], 0),
		ISNULL(src.[HeaderShipToCompanySKey], 0),
		ISNULL(src.[SiteSKey], 0),
		ISNULL(src.[HeaderSourceEquipment1SKey], 0),
		ISNULL(src.[HeaderSubType], @dummyId),
		ISNULL(src.[HeaderSupplierCompanySKey], 0),
		src.[HeaderTimeIn],	
		ISNULL(src.[HeaderTimeInDateSKey], @dummyDateSKey),
		ISNULL(src.[HeaderTimeInTimeSKey], 0),
		src.[HeaderTimeOut],
		ISNULL(src.[HeaderTimeOutDateSKey], @dummyDateSKey),
		ISNULL(src.[HeaderTimeOutTimeSKey], 0),
		ISNULL(src.[HeaderTransactionAliasSKey], 0),
		0 [TransactionAttributesSKey],
		src.[TransactionKey],
		src.[TransactionLineItemKey],
		src.[HeaderTransactionStatusIndex],
		ISNULL(src.[HeaderTransactionStatusName], @dummyId),
		ISNULL(src.[HeaderTransactionTypeSKey], 0),
		src.[HeaderTransDateTime],
		ISNULL(src.[HeaderTransDateSKey], @dummyDateSKey),
		ISNULL(src.[HeaderTransTimeSKey], 0),
		src.[HeaderTransID],
		ISNULL(src.[HeaderTransVersion], 0),
		NULL [UData_UserData2],
		NULL [UData_UserData23],
		NULL [UData_UserData3],
		0 [UData_UserData4SI],
		0 [UData_UserData4USGallon],
		0 [UData_UserData5SI],
		0 [UData_UserData5USGallon],
		0 [UData_UserData6SI],
		0 [UData_UserData6USGallon],

		NULL [TransactionLineItemUserDataKey],
		NULL [TransactionUserDataKey],

		ISNULL(src.[IsRecordDeleted], 0),
		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM staging.tblTransactionLineItems src
	INNER JOIN staging.tblInsertedLineItems b
	ON b.TransactionLineItemKey = src.TransactionLineItemKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND b.IsProcessed = 0





	UPDATE tgt 
	SET tgt.[DeleteFlag] = CASE WHEN (src.[DeleteFlag] = 1 OR ISNULL(src.[HeaderDeleteFlag], 0) = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END,
		tgt.[Line_ConjoinProductSKey] = ISNULL(src.[ConjoinProductSKey], 0),
		tgt.[Line_Density] = ISNULL(src.[Density], 0),
		tgt.[Line_DestinationEquipmentSKey] = ISNULL(src.[DestinationEquipmentSKey], 0),
		tgt.[Line_GrossQuantitySI] = ISNULL(src.[GrossQuantitySI], 0),
		tgt.[Line_GrossQuantityUSGallon] = ISNULL(src.[GrossQuantityUSGallon], 0),
		tgt.[Line_LoadArmSKey] = ISNULL(src.[LoadArmSKey], 0),
		tgt.[Line_MeterID] = src.[MeterID],
		tgt.[Line_MeterStart] = ISNULL(src.[MeterStart], 0),
		tgt.[Line_MeterStartDateTime] = src.[MeterStartDateTime],		
		tgt.[Line_MeterStop] = ISNULL(src.[MeterStop], 0),
		tgt.[Line_MeterStopDateTime] = src.[MeterStopDateTime],		
		tgt.[Line_MeterStartStopTimeDiff] = ISNULL(src.[MeterStartStopTimeDiff], 0),	
		tgt.[Line_NetQuantitySI] = ISNULL(src.[NetQuantitySI], 0),
		tgt.[Line_NetQuantityUSGallon] = ISNULL(src.[NetQuantityUSGallon], 0),
		tgt.[Line_NetVolumeIndicator] = ISNULL(src.[NetVolumeIndicator], 0),
		tgt.[Line_ProductSKey] = ISNULL(src.[ProductSKey], 0),
		tgt.[Line_SequenceID] = ISNULL(src.[SequenceID], 0),
		tgt.[Line_SourceEquipmentSKey] = ISNULL(src.[SourceEquipmentSKey], 0),
		tgt.[Line_StationSKey] = ISNULL(src.[LoadingLocationStationSKey], 0),
		tgt.[Line_StorageLocationTankSKey] = ISNULL(src.[StorageLocationTankSKey], 0),
		tgt.[Line_Temperature] = ISNULL(src.[Temperature], 0),
		tgt.[Line_Vcf] = ISNULL(src.[Vcf], 0),
		tgt.[TransactionKey] = src.[TransactionKey],
		tgt.[TransactionLineItemKey] = src.[TransactionLineItemKey],

		tgt.[_IsRecordDeleted] = ISNULL(src.[IsRecordDeleted], 0),
		tgt.[_RecordUpdatedDate] = ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		tgt.[_RecordUpdatedDateSKey] = ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactTransaction tgt
	INNER JOIN staging.tblTransactionLineItems src
	ON src.TransactionLineItemKey = tgt.TransactionLineItemKey
	INNER JOIN staging.tblUpdatedRecordsTemp b
	ON b.RecordKey = src.TransactionLineItemKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0
	AND tgt.TransactionSubLineItemKey IS NULL
	AND (src.CombinedUpdatedDate > tgt._RecordUpdatedDate)
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
	AND --Ignore Conjoin Transaction records that have been artificially added by the ETL. Those are updated separately from the Merge.
    (
		(src.HeaderConjoinTransID IS NULL)
		OR 
		(
			(src.HeaderConjoinTransID IS NOT NULL)
			AND (src.IsRecordAddedByETL = 0)
			AND (src.SourceFactSKey IS NULL)
		)
	)	

	UPDATE staging.tblInsertedLineItems
	SET IsProcessed = 1


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
    + 'Procedure Name: [staging].[usp_LoadTransactionLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END