/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionSummary]

	EXEC [staging].[usp_LoadTransactionSummary] 0, 0

	EXEC [staging].[usp_LoadTransactionSummary] 0, 200000
	
	EXEC [staging].[usp_LoadTransactionSummary] 200000, 0
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionSummary]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadTransactionSummary]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction Summary records from staging into the FactTransactionSummary table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionSummarySKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSummarySKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. The Transaction Summary Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency to a 
  --    level 2 table.
  -- 5. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 6. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 7. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 8. No historical data maintained for FactTransactionSummary. Simply update the existing record if found, otherwise insert a new one.
  -- 9. Transaction deletions in the OLTP database, whether soft deletions (DeleteFlag = 1) or physical deletions are translated in the 
  --    OLAP database as soft deletions.
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


	TRUNCATE TABLE staging.tblInsertedRecordsTemp
	TRUNCATE TABLE staging.tblUpdatedRecordsTemp


	INSERT INTO staging.tblUpdatedRecordsTemp ([RecordKey])
	SELECT
	src.[TransactionKey]
	FROM staging.tblTransactionSummary src
    WHERE src.TransactionKey IS NOT NULL
    AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))


	INSERT INTO staging.tblInsertedRecordsTemp ([RecordKey])
	SELECT
	src.[RecordKey]
	FROM staging.tblUpdatedRecordsTemp src
    WHERE NOT EXISTS
	(
		SELECT * FROM dbo.FactTransactionSummary tgt
		WHERE tgt.TransactionKey = src.RecordKey
	)

	DELETE a 
	FROM staging.tblUpdatedRecordsTemp a
	WHERE EXISTS
	(
		SELECT * FROM staging.tblInsertedRecordsTemp ins
		WHERE ins.RecordKey = a.RecordKey
	)
	
	INSERT INTO dbo.FactTransactionSummary
	(                            
		[BillToCompanySKey],
		[CarrierCompanySKey],
		[DeleteFlag],
		[DestinationEquipment1SKey],
		[InventoryDateSKey],
		[Line_MeterMaxStopTime],
		[Line_MeterMinStartTime],
		[Line_MeterMinStartMaxStopTimeDiff],
		[Line_MaxMeterStopTimeOutDiff],
		[Line_TimeInMinMeterStartDiff],
		[ManagerCompanySKey],
		[OperatorPersonnelSKey],
		[OwnerCompanySKey],
		[ReasonCodeSKey],
		[ReversalType],
		[ShipperCompanySKey],
		[ShipToCompanySKey],
		[SiteSKey],
		[SourceEquipment1SKey],
		[SubType],
		[SupplierCompanySKey],
		[TimeIn],		
		[TimeInDateSKey],
		[TimeInTimeSKey],
		[TimeInTimeOutDiff],
		[TimeOut],
		[TimeOutDateSKey],
		[TimeOutTimeSKey],
		[TransactionAliasSKey],
		[TransactionAttributesSKey],
		[TransDateTime],
		[TransDateSKey],
		[TransTimeSKey],
		[TransactionKey],
		[TransactionStatusIndex],
		[TransactionStatusName],
		[TransactionTypeSKey],
		[TransID],
		[_IsRecordDeleted],
		[_RecordUpdatedDate],
		[_RecordUpdatedDateSKey]
	)
	SELECT 
		ISNULL(src.[BillToCompanySKey], 0),
		ISNULL(src.[CarrierCompanySKey], 0),
		CASE WHEN (src.[DeleteFlag] = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END [DeleteFlag],
		ISNULL(src.[DestinationEquipment1SKey], 0),
		ISNULL(src.[InventoryDateSKey], @dummyDateSKey),
		[Line_MeterMaxStopTime],
		[Line_MeterMinStartTime],
		ISNULL([Line_MeterMinStartMaxStopTimeDiff], 0),
		ISNULL([Line_MaxMeterStopTimeOutDiff], 0),
		ISNULL([Line_TimeInMinMeterStartDiff], 0),
		ISNULL(src.[ManagerCompanySKey], 0),
		ISNULL(src.[OperatorPersonnelSKey], 0),
		ISNULL(src.[OwnerCompanySKey], 0),
		ISNULL(src.[ReasonCodeSKey], 0),
		ISNULL(src.[ReversalType], @veryShortDummyId),
		ISNULL(src.[ShipperCompanySKey], 0),
		ISNULL(src.[ShipToCompanySKey], 0),
		ISNULL(src.[SiteSKey], 0),
		ISNULL(src.[SourceEquipment1SKey], 0),
		ISNULL(src.[SubType], @dummyId),
		ISNULL(src.[SupplierCompanySKey], 0),
		src.[TimeIn],	
		ISNULL(src.[TimeInDateSKey], @dummyDateSKey),
		ISNULL(src.[TimeInTimeSKey], 0),
		ISNULL(src.[TimeInTimeOutDiff], 0),
		src.[TimeOut],
		ISNULL(src.[TimeOutDateSKey], @dummyDateSKey),
		ISNULL(src.[TimeOutTimeSKey], 0),
		ISNULL(src.[TransactionAliasSKey], 0),
		0 [TransactionAttributesSKey],
		src.[TransDateTime],
		ISNULL(src.[TransDateSKey], @dummyDateSKey),
		ISNULL(src.[TransTimeSKey], 0),

		src.[TransactionKey],
		src.[TransactionStatusIndex],
		ISNULL(src.[TransactionStatusName], @dummyId),
		ISNULL(src.[TransactionTypeSKey], 0),
		src.[TransID],
		ISNULL(src.[IsRecordDeleted], 0),
		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)
	FROM staging.tblTransactionSummary src
	INNER JOIN staging.tblInsertedRecordsTemp b
	ON b.RecordKey = src.TransactionKey
	WHERE src.TransactionKey IS NOT NULL
	AND src.IgnoreRecord = 0



    UPDATE tgt
	SET tgt.[BillToCompanySKey] = ISNULL(src.[BillToCompanySKey], 0),
		tgt.[CarrierCompanySKey] = ISNULL(src.[CarrierCompanySKey], 0),
		tgt.[DeleteFlag] = CASE WHEN (src.[DeleteFlag] = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END,
		tgt.[DestinationEquipment1SKey] = ISNULL(src.[DestinationEquipment1SKey], 0),
		tgt.[InventoryDateSKey] = ISNULL(src.[InventoryDateSKey], @dummyDateSKey),
		tgt.[Line_MeterMaxStopTime] = src.[Line_MeterMaxStopTime],
		tgt.[Line_MeterMinStartTime] = src.[Line_MeterMinStartTime],
		tgt.[Line_MeterMinStartMaxStopTimeDiff] = ISNULL(src.[Line_MeterMinStartMaxStopTimeDiff], 0),
		tgt.[Line_MaxMeterStopTimeOutDiff] = ISNULL(src.[Line_MaxMeterStopTimeOutDiff], 0),
		tgt.[Line_TimeInMinMeterStartDiff] = ISNULL(src.[Line_TimeInMinMeterStartDiff], 0),
		tgt.[ManagerCompanySKey] = ISNULL(src.[ManagerCompanySKey], 0),
		tgt.[OperatorPersonnelSKey] = ISNULL(src.[OperatorPersonnelSKey], 0),
		tgt.[OwnerCompanySKey] = ISNULL(src.[OwnerCompanySKey], 0),
		tgt.[ReasonCodeSKey] = ISNULL(src.[ReasonCodeSKey], 0),
		tgt.[ReversalType] = ISNULL(src.[ReversalType], @veryShortDummyId),
		tgt.[ShipperCompanySKey] = ISNULL(src.[ShipperCompanySKey], 0),
		tgt.[ShipToCompanySKey] = ISNULL(src.[ShipToCompanySKey], 0),
		tgt.[SiteSKey] = ISNULL(src.[SiteSKey], 0),
		tgt.[SourceEquipment1SKey] = ISNULL(src.[SourceEquipment1SKey], 0),
		tgt.[SubType] = ISNULL(src.[SubType], @dummyId),
		tgt.[SupplierCompanySKey] = ISNULL(src.[SupplierCompanySKey], 0),
		tgt.[TimeIn] = src.[TimeIn],
		tgt.[TimeInDateSKey] = ISNULL(src.[TimeInDateSKey], @dummyDateSKey),
		tgt.[TimeInTimeSKey] = ISNULL(src.[TimeInTimeSKey], 0),
		tgt.[TimeInTimeOutDiff] = ISNULL(src.[TimeInTimeOutDiff], 0),
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

		tgt.[_IsRecordDeleted] = ISNULL(src.[IsRecordDeleted], 0),
		tgt.[_RecordUpdatedDate] = ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		tgt.[_RecordUpdatedDateSKey] = ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactTransactionSummary tgt
	INNER JOIN staging.tblTransactionSummary src
	ON src.TransactionKey = tgt.TransactionKey
	INNER JOIN staging.tblUpdatedRecordsTemp b
	ON b.RecordKey = src.TransactionKey
    WHERE src.TransactionKey IS NOT NULL
    AND src.IsProcessed = 0
    AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))


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
    + 'Procedure Name: [staging].[usp_LoadTransactionSummary]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END