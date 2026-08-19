/*
    DROP PROCEDURE [staging].[usp_IdentifyMissingSegmentsForLineItemChanges]

	EXEC [staging].[usp_IdentifyMissingSegmentsForLineItemChanges]
	
*/
CREATE PROCEDURE [staging].[usp_IdentifyMissingSegmentsForLineItemChanges]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IdentifyMissingSegmentsForLineItemChanges]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Identify new LineItems against transaction headers for which there is already a FactTransaction record, but for 
  --          which the Header segment or the HeaderUSerData is missing in staging.
  -- Notes:
  -- 1. This exercise is limited to new LineItems, because for existing LineItems missing segments do not matter. New staging 
  --    records on exisitng LineItems are handled as FactTransaction updates that are restricted to the LineItem segment fields, 
  --    meaning the other segments are not required during a LineItem segment update. In addition, the LineItem segment carries 
  --    its own InventoryDate field, which allows the LineItem Type2 SCD fields to be resolved independently of the parent header 
  --    record.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

	DECLARE @tblPartialSegment TABLE
	(
		[RecordKey] [nvarchar](50) NULL,
		[TransactionKey] [nvarchar](50) NULL,
		[MissingSegmentType] [nvarchar](100) NULL,
		[SourceFactTransactionSKey] [int] NULL
	);

	DECLARE @tblSourceFactTransaction TABLE
	(
		[SKey] [int] NULL,
		[TransactionKey] [nvarchar](50) NULL
	);


	--Check for missing Header segment
	INSERT INTO @tblPartialSegment
	(RecordKey, TransactionKey, MissingSegmentType)
	SELECT DISTINCT a.TransactionLineItemKey, a.TransactionKey, 'Header' 
	FROM staging.tblTransactionLineItems a
	WHERE a.IgnoreRecord = 0
	AND NOT EXISTS
	(
		SELECT * FROM staging.tblTransactions c
		WHERE c.TransactionKey = a.TransactionKey
	)
	
	--Check for missing Header UserData segments
	INSERT INTO @tblPartialSegment
	(RecordKey, TransactionKey, MissingSegmentType)
	SELECT DISTINCT a.TransactionLineItemKey, a.TransactionKey, 'HeaderUserData' 
	FROM staging.tblTransactionLineItems a
	WHERE a.IgnoreRecord = 0
	AND NOT EXISTS
	(
		SELECT * FROM staging.tblTransactionUserData c
		WHERE c.TransactionKey = a.TransactionKey
	)
	
	--Fetch matching source FactTransaction records
	INSERT INTO @tblSourceFactTransaction
	(SKey, TransactionKey)
	SELECT MAX(SKey) SKey, a.TransactionKey
	FROM FactTransaction a
	INNER JOIN @tblPartialSegment b
	ON b.TransactionKey = a.TransactionKey
	GROUP BY a.TransactionKey


	--Identify the cloning master FactTransaction record to be used for new partial LineItem records, i.e. LineItems for which a FactTransaction record does not exist.
	UPDATE a
	SET a.SourceFactTransactionSKey = b.SKey
	FROM @tblPartialSegment a
	INNER JOIN @tblSourceFactTransaction b
	ON b.TransactionKey = a.TransactionKey
	WHERE NOT EXISTS
	(
		SELECT * FROM FactTransaction c
		WHERE c.TransactionLineItemKey = a.RecordKey
	)



	-- Identify new LineItems for which there is already a FactTransaction record for the parent header segment, and for which the Header segment is missing in staging
	INSERT INTO [staging].[tblPartialTransactionSegment]
	(RecordKey, SegmentType, SourceFactTransactionSKey, IsNewMainSegment, MissingSegmentType, InventoryDateChanged, IsProcessed)
	SELECT a.RecordKey, 'LineItem', b.SKey, 1, 'Header', 0, 0
	FROM @tblPartialSegment a
	INNER JOIN FactTransaction b
	ON b.SKey = a.SourceFactTransactionSKey
	WHERE a.MissingSegmentType = 'Header'
	AND a.SourceFactTransactionSKey IS NOT NULL


	-- Identify new LineItems for which there is already a FactTransaction record for the parent header segment, and for which the HeadeUserData segment is missing in staging
	INSERT INTO [staging].[tblPartialTransactionSegment]
	(RecordKey, SegmentType, SourceFactTransactionSKey, IsNewMainSegment, MissingSegmentType, InventoryDateChanged, IsProcessed)
	SELECT a.RecordKey, 'LineItem', b.SKey, 1, 'HeaderUserData', 0, 0
	FROM @tblPartialSegment a
	INNER JOIN FactTransaction b
	ON b.SKey = a.SourceFactTransactionSKey
	WHERE a.MissingSegmentType = 'HeaderUserData'
	AND a.SourceFactTransactionSKey IS NOT NULL
	AND b.TransactionUserDataKey IS NOT NULL


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
    + 'Procedure Name: [staging].[usp_IdentifyMissingSegmentsForLineItemChanges]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
