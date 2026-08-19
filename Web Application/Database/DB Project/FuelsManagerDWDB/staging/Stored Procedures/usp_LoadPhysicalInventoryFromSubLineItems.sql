/*
	DROP PROCEDURE [staging].[usp_LoadPhysicalInventoryFromSubLineItems]

	EXEC [staging].[usp_LoadPhysicalInventoryFromSubLineItems] 0, 0

	EXEC [staging].[usp_LoadPhysicalInventoryFromSubLineItems] 0, 200000

	EXEC [staging].[usp_LoadPhysicalInventoryFromSubLineItems] 200000, 0
	
*/
CREATE PROCEDURE [staging].[usp_LoadPhysicalInventoryFromSubLineItems]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadPhysicalInventoryFromSubLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Physical Inventory records from the SubLineItem records from staging into the FactPhysicalInventorySnapshot table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionSubLineItemSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSubLineItemSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. Physical Inventory is recorded by Manager-Owner-Site-Product-Location-InventoryDate. The Manager-Owner-Site-Product Physical Inventory for a day is 
  --    therefore the sum of all the Physical Inventory entries for the Site and Product at all the locations of that site.
  -- 5. In case there are more than one Physical Inventory entry for the same Manager-Owner-Site-Product-Location-InventoryDate combination, the last 
  --    available entry, based on the CombinedUpdatedDate is selected. If after that filtering, there are still duplicate entries for the same 
  --    Manager-Owner-Site-Product-Location-InventoryDate combination because they share the same identical CombinedUpdatedDate, then one record will be
  --    arbitrarily selected from the duplicate set.
  -- 6. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  -- 7. LineItem deletions in the OLTP database, whether soft deletions (DeleteFlag = 1) or physical deletions are translated in the 
  --    OLAP database FactPhysicalInventorySnapshot table as physical deletions.
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

	INSERT INTO staging.tblUpdatedRecordsTemp ([RecordSKey], [RecordKey])
	SELECT
		src.SKey, src.[TransactionSubLineItemKey]
	FROM staging.tblTransactionSubLineItems src
	INNER JOIN dbo.DimTransactionType b
	ON b.SKey = src.HeaderTransactionTypeSKey
	WHERE src.TransactionSubLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
	AND (b.TransactionTypeCode = 'T14_PhysicalInventory')


	DECLARE @tblLatestEntryCombination TABLE
	(
		InventoryDateSKey int,
		SiteSKey int,
		ProductSKey int,
		ManagerSKey int,
		OwnerSKey int,
		StorageLocationSKey int,
		SubLineItemLastSKey int
	)


	--For entries with an actual LocationId, select the last entry for each combination of InventoryDateSKey,SiteSKey,ProductSKey,ManagerSKey,OwnerSKey, and LocationId,
	INSERT INTO @tblLatestEntryCombination
	(
		InventoryDateSKey,
		SiteSKey,
		ProductSKey,
		ManagerSKey,
		OwnerSKey,
		StorageLocationSKey,
		SubLineItemLastSKey
	)
	SELECT x.HeaderInventoryDateSKey, x.SiteSKey, x.ProductSKey, x.HeaderManagerCompanySKey, x.HeaderOwnerCompanySKey, x.StorageLocationTankSKey, x.SKey
	FROM
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY a.HeaderInventoryDateSKey, a.SiteSKey, ISNULL(a.ProductSKey, 0), ISNULL(a.HeaderManagerCompanySKey, 0), ISNULL(a.HeaderOwnerCompanySKey, 0), ISNULL(a.StorageLocationTankSKey, 0)
		ORDER BY a.CombinedUpdatedDate DESC, a.SKey DESC) AS [RowNum], 
			a.HeaderInventoryDateSKey,
			a.SiteSKey,
			a.ProductSKey,
			a.HeaderManagerCompanySKey,
			a.HeaderOwnerCompanySKey, 
			a.StorageLocationTankSKey,			
			a.CombinedUpdatedDate,
			a.SKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN staging.tblUpdatedRecordsTemp c
		ON c.RecordSKey = a.SKey
	) x
	WHERE x.RowNum = 1


	DELETE a 
	FROM staging.tblUpdatedRecordsTemp a
	WHERE NOT EXISTS
	(
		SELECT * FROM @tblLatestEntryCombination b
		WHERE b.SubLineItemLastSKey = a.RecordSKey
	)


	UPDATE a
	SET a.IsNewRecord = 1
	FROM staging.tblUpdatedRecordsTemp a
	INNER JOIN staging.tblTransactionSubLineItems src
	ON src.TransactionSubLineItemKey = a.RecordKey
	WHERE NOT EXISTS
	(
		SELECT * FROM dbo.FactPhysicalInventorySnapshot tgt
		WHERE tgt.InventoryDateSKey = src.HeaderInventoryDateSKey
		AND tgt.SiteSKey = src.SiteSKey
		AND tgt.Line_ProductSKey = src.ProductSKey
		AND tgt.ManagerCompanySKey = src.HeaderManagerCompanySKey
		AND tgt.OwnerCompanySKey = src.HeaderOwnerCompanySKey
		AND tgt.StorageLocationTankSKey = src.StorageLocationTankSKey
	)


	
	INSERT INTO dbo.FactPhysicalInventorySnapshot
	(
		[InventoryDateSKey],
		[Line_GrossQuantitySI],
		[Line_GrossQuantityUSGallon],
		[Line_NetQuantitySI],		
		[Line_NetQuantityUSGallon],
		[Line_NetVolumeIndicator],	
		[Line_ProductSKey],
		[ManagerCompanySKey],
		[OwnerCompanySKey],
		[StorageLocationTankSKey],
		[SiteSKey],			
		[SubType],
		[TransactionAliasSKey],				
		[TransactionStatusName],
		[TransDateTime],
		[TransID],			

		[TransactionKey],
		[TransactionLineItemKey],
		[TransactionSubLineItemKey],

		[_RecordUpdatedDate],
		[_RecordUpdatedDateSKey]
	)
	SELECT 
		ISNULL(src.[HeaderInventoryDateSKey], @dummyDateSKey),
		ISNULL(src.[GrossQuantitySI], 0),
		ISNULL(src.[GrossQuantityUSGallon], 0),
		ISNULL(src.[NetQuantitySI], 0),
		ISNULL(src.[NetQuantityUSGallon], 0),
		0 [NetVolumeIndicator],  --NetVolumeIndicator is only applicable to FMAviation, which does not support SubLineItems
		ISNULL(src.[ProductSKey], 0),
		ISNULL(src.[HeaderManagerCompanySKey], 0),
		ISNULL(src.[HeaderOwnerCompanySKey], 0),
		ISNULL(src.[StorageLocationTankSKey], 0),
		ISNULL(src.[SiteSKey], 0),
		ISNULL(src.[HeaderSubType], @dummyId),
		ISNULL(src.[HeaderTransactionAliasSKey], 0),
		ISNULL(src.[HeaderTransactionStatusName], @dummyId),
		src.[HeaderTransDateTime],
		src.[HeaderTransID],		

		src.[TransactionKey],
		src.[TransactionLineItemKey],
		src.[TransactionSubLineItemKey],

		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM staging.tblTransactionSubLineItems src
	INNER JOIN staging.tblUpdatedRecordsTemp a
	ON a.RecordKey = src.TransactionSubLineItemKey	
	WHERE src.TransactionSubLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND a.IsNewRecord = 1



	UPDATE tgt 
	SET tgt.[InventoryDateSKey] = ISNULL(src.[HeaderInventoryDateSKey], @dummyDateSKey),
		tgt.[Line_GrossQuantitySI] = ISNULL(src.[GrossQuantitySI], 0),
		tgt.[Line_GrossQuantityUSGallon] = ISNULL(src.[GrossQuantityUSGallon], 0),
		tgt.[Line_NetQuantitySI] = ISNULL(src.[NetQuantitySI], 0),
		tgt.[Line_NetQuantityUSGallon] = ISNULL(src.[NetQuantityUSGallon], 0),
		--tgt.[Line_NetVolumeIndicator] = ISNULL(src.[NetVolumeIndicator], 0),
		tgt.[Line_ProductSKey] = ISNULL(src.[ProductSKey], 0),
		tgt.[ManagerCompanySKey] = ISNULL(src.[HeaderManagerCompanySKey], 0),
		tgt.[OwnerCompanySKey] = ISNULL(src.[HeaderOwnerCompanySKey], 0),
		tgt.[StorageLocationTankSKey] = ISNULL(src.[StorageLocationTankSKey], 0),
		tgt.[SiteSKey] = ISNULL(src.[SiteSKey], 0),
		tgt.[SubType] = ISNULL(src.[HeaderSubType], @dummyId),
		tgt.[TransactionAliasSKey] = ISNULL(src.[HeaderTransactionAliasSKey], 0),
		tgt.[TransactionStatusName] = ISNULL(src.[HeaderTransactionStatusName], @dummyId),
		tgt.[TransDateTime] = src.[HeaderTransDateTime],
		tgt.[TransID] = src.[HeaderTransID],

		tgt.[TransactionKey] = src.[TransactionKey],
		tgt.[TransactionLineItemKey] = src.[TransactionLineItemKey],
		tgt.[TransactionSubLineItemKey] = src.[TransactionSubLineItemKey],

		tgt.[_RecordUpdatedDate] = ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		tgt.[_RecordUpdatedDateSKey] = ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactPhysicalInventorySnapshot tgt
	INNER JOIN staging.tblTransactionSubLineItems src
	ON src.TransactionSubLineItemKey = tgt.TransactionSubLineItemKey
	INNER JOIN staging.tblUpdatedRecordsTemp b
	ON b.RecordKey = src.TransactionSubLineItemKey	
	WHERE src.TransactionSubLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND (src.CombinedUpdatedDate > tgt._RecordUpdatedDate)
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))	
	AND b.IsNewRecord <> 1



	DELETE tgt 
	FROM dbo.FactPhysicalInventorySnapshot tgt
	INNER JOIN staging.tblTransactionSubLineItems src
	ON src.TransactionSubLineItemKey = tgt.TransactionSubLineItemKey
	INNER JOIN staging.tblUpdatedRecordsTemp b
	ON b.RecordKey = src.TransactionSubLineItemKey	
	WHERE src.TransactionSubLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))	
	AND src.IsRecordDeleted = 1


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
    + 'Procedure Name: [staging].[usp_LoadPhysicalInventoryFromSubLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END