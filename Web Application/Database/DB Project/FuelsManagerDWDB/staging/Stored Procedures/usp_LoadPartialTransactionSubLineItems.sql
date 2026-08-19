/*
	DROP PROCEDURE [Staging].[usp_LoadPartialTransactionSubLineItems]

	EXEC [staging].[usp_LoadPartialTransactionSubLineItems]
	
*/
CREATE PROCEDURE [staging].[usp_LoadPartialTransactionSubLineItems]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadPartialTransactionSubLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction SubLineItem records from staging into the FactTransaction table in the OLAP database for SubLineItems for which 
  --		  the Header segment and/or the SubLineItem segment were not captured by the CDC.
  -- Notes:
  -- 1. New SubLineItems for which the Header segment and the LineItem segment were also captured in the CDC tables, are loaded through the regular SubLineItem loading operation.
  -- 2. This operation is for new SubLineItems with either a missing Header segment or a missing LineItem segment. They are loaded by cloning a FactTransaction record with 
  --    the same LineItem key as that of the new SubLineItem, and then setting the LineItem fields of the new record with the new LineItem data.
  -- 3. This operation should be executed before the regular SubLineItem loading operation, which is not configured to handle missing segments.
  -- 4. The setting of the HeaderUserData fields and the LineItemUserData fields for new SubLineItems with a missing HeaderUserData segment and a missing LineItemUserData segment
  --    is performed in separate operations, after the regular SubLineItem loading operation.
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

    IF ((SELECT COUNT(*) FROM staging.tblTransactionSubLineItems WHERE IsProcessed = 0) = 0)
    BEGIN
      RETURN
    END

	IF 
	(
		(
			SELECT COUNT(*) FROM staging.tblPartialTransactionSegment 
			WHERE SegmentType = 'SubLineItem' 
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
	WHERE SegmentType = 'SubLineItem' 
	AND IsNewMainSegment = 1 
	AND IsProcessed = 0 
	GROUP BY RecordKey



	-- For new SubLine items with a missing Header segment and/or Line segment in staging, clone a FactTransaction record with the same TransactionKey, and set the LineItem fields with the new LineItem data
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
		[TransactionSubLineItemKey],
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
		clone.BillToCompanySKey,
		clone.CarrierCompanySKey,
		clone.ConjoinOwnerSKey,
		clone.ConjoinTransID,
		clone.CreatedDate,
		clone.CreatedDateSKey,
		clone.CreatedTimeSKey,
		clone.Date01DateSKey,
		clone.Date01TimeSKey,
		CASE WHEN (src.[DeleteFlag] = 1 OR ISNULL(src.[HeaderDeleteFlag], 0) = 1 OR src.[IsRecordDeleted] = 1) THEN 1 ELSE 0 END [DeleteFlag],
		clone.DestinationEquipment1SKey,
		clone.DocumentNumber,
		clone.InternationalRouteIndicator,
		clone.InventoryDateSKey,
		ISNULL(src.[ConjoinProductSKey], 0),
		ISNULL(src.[Density], 0),
		clone.Line_DestinationEquipmentSKey,
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
		clone.Line_NetVolumeIndicator,
		ISNULL(src.[ProductSKey], 0),
		ISNULL(src.[SequenceID], 0),
		clone.Line_SourceEquipmentSKey,
		clone.Line_StationSKey,
		ISNULL(src.StorageLocationTankSKey, 0),
		ISNULL(src.[Temperature], 0),
		ISNULL(src.[Vcf], 0),
		clone.LineUData_UserData1,
		clone.ManagerCompanySKey,
		clone.Number01,
		clone.OperatorPersonnelSKey,
		clone.OwnerCompanySKey,
		clone.ReasonCodeSKey,
		clone.ReversalType,
		clone.ReversedTransID,
		clone.RoutingID,
		clone.ShipperCompanySKey,
		clone.ShipToCompanySKey,
		clone.SiteSKey,
		clone.SourceEquipment1SKey,
		clone.SubType,
		clone.SupplierCompanySKey,
		clone.TimeIn,	
		clone.TimeInDateSKey,
		clone.TimeInTimeSKey,
		clone.TimeOut,
		clone.TimeOutDateSKey,
		clone.TimeOutTimeSKey,
		clone.TransactionAliasSKey,
		clone.TransactionAttributesSKey,
		src.[TransactionKey],
		src.[TransactionLineItemKey],
		src.[TransactionSubLineItemKey],
		src.[HeaderTransactionStatusIndex],
		clone.TransactionStatusName,
		clone.TransactionTypeSKey,
		clone.TransDateTime,
		clone.TransDateSKey,
		clone.TransTimeSKey,
		clone.TransID,
		clone.TransVersion,
		clone.UData_UserData2,
		clone.UData_UserData23,
		clone.UData_UserData3,
		clone.UData_UserData4SI,
		clone.UData_UserData4USGallon,
		clone.UData_UserData5SI,
		clone.UData_UserData5USGallon,
		clone.UData_UserData6SI,
		clone.UData_UserData6USGallon,

		clone.TransactionLineItemUserDataKey,
		clone.TransactionUserDataKey,

		ISNULL(src.[IsRecordDeleted], 0),		
		ISNULL(src.[CombinedUpdatedDate], @dummyDateTime),
		ISNULL(src.[CombinedUpdatedDateSKey], @dummyDateSKey)

	FROM dbo.FactTransaction clone
	INNER JOIN @tblSegmentFactSKey b
	ON b.CloneMasterFactTransactionSKey = clone.SKey
	INNER JOIN staging.tblTransactionSubLineItems src
	ON src.TransactionLineItemKey = b.RecordKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
	AND src.IsProcessed = 0

	UPDATE a 
	SET a.IsProcessed = 1
	FROM staging.tblTransactionSubLineItems a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.TransactionSubLineItemKey


	--Update the sublineitems FactTransaction records that were created above, with the corresponding staging LineItem segments if present
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
		tgt.[_IsRecordDeleted] = ISNULL(src.[IsRecordDeleted], 0)
		--RecordUpdatedKey of the partial SubLineItem FactTransaction record already set during the Insert statement above
	FROM dbo.FactTransaction tgt
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = tgt.TransactionSubLineItemKey
	INNER JOIN staging.tblTransactionLineItems src
	ON src.TransactionLineItemKey = tgt.TransactionLineItemKey
	WHERE src.TransactionLineItemKey IS NOT NULL
	AND src.IgnoreRecord = 0
    AND src.IsProcessed = 0


	--Update the sublineitems FactTransaction records that were created above, with the corresponding staging header segments if present
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
		tgt.[_IsRecordDeleted] = ISNULL(src.[IsRecordDeleted], 0)
		--RecordUpdatedKey of the partial SubLineItem FactTransaction record already set during the Insert statement above
	FROM dbo.FactTransaction tgt
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = tgt.TransactionSubLineItemKey
	INNER JOIN staging.tblTransactions src
	ON src.TransactionKey = tgt.TransactionKey
    WHERE src.TransactionKey IS NOT NULL
	AND src.IgnoreRecord = 0
    AND src.IsProcessed = 0


	UPDATE a 
	SET a.IsProcessed = 1
	FROM staging.tblTransactionSubLineItems a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.TransactionSubLineItemKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblPartialTransactionSegment a
	INNER JOIN @tblSegmentFactSKey b
	ON b.RecordKey = a.RecordKey
	WHERE a.SegmentType = 'SubLineItem'
	
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
    + 'Procedure Name: [staging].[usp_LoadPartialTransactionSubLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END