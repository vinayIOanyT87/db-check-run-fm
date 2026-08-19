/*
	DROP PROCEDURE [Staging].[usp_LoadPartialTransactionLineItems]

	EXEC [staging].[usp_LoadPartialTransactionLineItems]
	
*/
CREATE PROCEDURE [staging].[usp_LoadPartialTransactionLineItems]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadPartialTransactionLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction LineItem records from staging into the FactTransaction table in the OLAP database for LineItems for which 
  --		  the Header segment was not captured by the CDC.
  -- Notes:
  -- 1. New LineItems for which the Header segment was also captured in the CDC tables, are loaded through the regular LineItem loading operation.
  -- 2. This operation is for new LineItems with a missing Header segment. They are loaded by cloning a FactTransaction record with the same Header 
  --    key as that of the new LineItem, and then setting the LineItem fields of the new FactTransaction record with the new LineItem data.
  -- 3. This operation should be executed before the regular LineItem loading operation, which is not configured to handle missing segments.
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

	DECLARE @tblSegmentFactSKey TABLE
	(
		[RecordKey] [nvarchar](50) NULL,
		[CloneMasterFactTransactionSKey] int NULL
	);

    IF ((SELECT COUNT(*) FROM staging.tblTransactionLineItems WHERE IsProcessed = 0) = 0)
    BEGIN
      RETURN
    END

	IF 
	(
		(
			SELECT COUNT(*) FROM staging.tblPartialTransactionSegment 
			WHERE SegmentType = 'LineItem' 
			AND IsNewMainSegment = 1 
			AND IsProcessed = 0 
		) = 0
	)
    BEGIN
      RETURN
    END

	INSERT INTO @tblSegmentFactSKey
	(RecordKey, CloneMasterFactTransactionSKey)
	SELECT RecordKey, MAX(SourceFactTransactionSKey) FROM staging.tblPartialTransactionSegment 
	WHERE SegmentType = 'LineItem' 
	AND IsNewMainSegment = 1 
	AND IsProcessed = 0 
	GROUP BY RecordKey



	-- For new Line items with missing Header segment, clone a FactTransaction record with the same TransactionKey, and set the LineItem fields with the new LineItem data
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
		clone.[BillToCompanySKey],
		clone.[CarrierCompanySKey],
		clone.[ConjoinOwnerSKey],
		clone.[ConjoinTransID],
		clone.[CreatedDate],
		clone.[CreatedDateSKey],
		clone.[CreatedTimeSKey],
		clone.[Date01DateSKey],
		clone.[Date01TimeSKey],
		clone.[DeleteFlag],
		clone.[DestinationEquipment1SKey],
		clone.[DocumentNumber],
		clone.[InternationalRouteIndicator],
		clone.[InventoryDateSKey],
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
		clone.[ManagerCompanySKey],
		clone.[Number01],
		clone.[OperatorPersonnelSKey],
		clone.[OwnerCompanySKey],
		clone.[ReasonCodeSKey],
		clone.[ReversalType],
		clone.[ReversedTransID],
		clone.[RoutingID],
		clone.[ShipperCompanySKey],
		clone.[ShipToCompanySKey],
		clone.[SiteSKey],
		clone.[SourceEquipment1SKey],
		clone.[SubType],
		clone.[SupplierCompanySKey],
		clone.[TimeIn],	
		clone.[TimeInDateSKey],
		clone.[TimeInTimeSKey],
		clone.[TimeOut],
		clone.[TimeOutDateSKey],
		clone.[TimeOutTimeSKey],
		clone.[TransactionAliasSKey],
		clone.[TransactionAttributesSKey],
		clone.[TransactionKey],
		clone.[TransactionLineItemKey],
		clone.[TransactionStatusIndex],
		clone.[TransactionStatusName],
		clone.[TransactionTypeSKey],
		clone.[TransDateTime],
		clone.[TransDateSKey],
		clone.[TransTimeSKey],
		clone.[TransID],
		clone.[TransVersion],
		clone.[UData_UserData2],
		clone.[UData_UserData23],
		clone.[UData_UserData3],
		clone.[UData_UserData4SI],
		clone.[UData_UserData4USGallon],
		clone.[UData_UserData5SI],
		clone.[UData_UserData5USGallon],
		clone.[UData_UserData6SI],
		clone.[UData_UserData6USGallon],

		clone.[TransactionLineItemUserDataKey],
		clone.[TransactionUserDataKey],

		clone.[_IsRecordDeleted],
		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactTransaction clone
	INNER JOIN @tblSegmentFactSKey b
	ON b.CloneMasterFactTransactionSKey = clone.SKey
	INNER JOIN staging.tblTransactionLineItems src
	ON src.TransactionLineItemKey = b.RecordKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND src.IsProcessed = 0



	UPDATE a 
	SET a.IsProcessed = 1
	FROM staging.tblTransactionLineItems a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.TransactionLineItemKey

	INSERT INTO staging.tblInsertedLineItems 
	([TransactionLineItemKey], [TransactionKey], [CombinedUpdatedDate], [IsRecordAddedByETL], [IsProcessed])
	SELECT
		a.[TransactionLineItemKey],
		a.[TransactionKey],
		a.[CombinedUpdatedDate],
		a.[IsRecordAddedByETL],
		1
	FROM staging.tblTransactionLineItems a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.TransactionLineItemKey
	WHERE NOT EXISTS
	(
		SELECT * FROM staging.tblInsertedLineItems  c
		WHERE c.TransactionLineItemKey = a.TransactionLineItemKey
	)

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblPartialTransactionSegment a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.RecordKey
	WHERE a.SegmentType = 'LineItem'
	
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
    + 'Procedure Name: [staging].[usp_LoadPartialTransactionLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
