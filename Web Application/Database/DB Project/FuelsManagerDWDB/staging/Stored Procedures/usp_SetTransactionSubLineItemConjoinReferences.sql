/*
	DROP PROCEDURE [Staging].[usp_SetTransactionSubLineItemConjoinReferences]

	EXEC [staging].[usp_SetTransactionSubLineItemConjoinReferences] 0, 0

	EXEC [staging].[usp_SetTransactionSubLineItemConjoinReferences] 0, 20000

	EXEC [staging].[usp_SetTransactionSubLineItemConjoinReferences] 20000, 0
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionSubLineItemConjoinReferences]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionSubLineItemConjoinReferences]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Resolves and sets the Conjoin fields for each Conjoin Transaction SubLineItem record.
  -- Notes:
  -- 1. @startSKey: TransactionSubLineItemSKey from which to filter the records to be updated. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSubLineItemSKey to which to filter the records to be updated. Leave as 0 to ignore this filter.
  -- 1. This operation assumes that both transactions in a conjoin pair are present in staging.
  -- 2. This operation assumes that the header information have already been merged into the lineitems and sublineitems.
  -- 3. This operation assumes that all the relvant Id fields (BillTo, ShipTo, etc.) have already been trimmed.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Transaction SubLineItem
    UPDATE a
    SET a.ConjoinProductKey = b.ProductKey,
        a.ConjoinProductSKey = b.ProductSKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactionSubLineItems b
    ON b.HeaderTransID = a.HeaderConjoinTransID
    AND b.SequenceID = a.SequenceID
    WHERE a.IgnoreRecord = 0
    AND a.HeaderConjoinTransID IS NOT NULL
    AND LEN(a.HeaderConjoinTransID) > 0
	AND ((a.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((a.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))

    IF
    (
		(
			SELECT COUNT(*) FROM staging.tblTransactionSubLineItems a
			WHERE a.IgnoreRecord = 0
			AND a.HeaderConjoinTransID IS NOT NULL
			AND LEN(a.HeaderConjoinTransID) > 0
			AND ((a.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
			AND ((a.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
			AND NOT EXISTS 
			(
				SELECT * FROM staging.tblTransactionSubLineItems b
				WHERE b.HeaderTransID = a.HeaderConjoinTransID
				AND b.SequenceID = a.SequenceID
			)
		) > 0
    )
    BEGIN
      RAISERROR ('Failure to find matching conjoin Transaction SubLineItem pair', 16, 1);
      RETURN;
    END


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
    + 'Procedure Name: [staging].[usp_SetTransactionLineItemConjoinReferences]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END