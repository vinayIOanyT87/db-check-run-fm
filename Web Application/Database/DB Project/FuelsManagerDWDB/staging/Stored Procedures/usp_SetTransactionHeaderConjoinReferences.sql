/*
	DROP PROCEDURE [Staging].[usp_SetTransactionHeaderConjoinReferences]

	EXEC [staging].[usp_SetTransactionHeaderConjoinReferences] 0, 0

	EXEC [staging].[usp_SetTransactionHeaderConjoinReferences] 0, 20000

	EXEC [staging].[usp_SetTransactionHeaderConjoinReferences] 20000, 0
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionHeaderConjoinReferences]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionHeaderConjoinReferences]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Resolves and sets the Conjoin fields for each Conjoin Transaction Header record.
  -- Notes:
  -- 1. @startSKey: TransactionSKey from which to filter the records to be updated. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSKey to which to filter the records to be updated. Leave as 0 to ignore this filter.
  -- 2. This operation assumes that both transactions in a conjoin pair are present in staging.
  -- 3. This operation sets the applicable conjoin fields on the header records, in the event that there can be 
  --    Conjoin transactions that do not have line items.
  -- 4. This operation assumes that all the relvant Id fields (BillTo, ShipTo, etc.) have already been trimmed.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Conjoin Owner
    UPDATE a
    SET a.ConjoinOwnerKey = b.OwnerCompanyKey,
        a.ConjoinOwnerSKey = b.OwnerCompanySKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblTransactions b
      ON b.TransID = a.ConjoinTransID
    WHERE a.IgnoreRecord = 0
    AND a.ConjoinTransID IS NOT NULL
    AND LEN(a.ConjoinTransID) > 0
	AND ((a.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((a.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))

    IF
    (
		(
			SELECT COUNT(*) FROM staging.tblTransactions a
			WHERE a.IgnoreRecord = 0
			AND a.ConjoinTransID IS NOT NULL
			AND LEN(a.ConjoinTransID) > 0
			AND a.TransactionAliasName = 'Inflight'
			AND ((a.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
			AND ((a.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
			AND NOT EXISTS 
			(
				SELECT * FROM staging.tblTransactions b
				WHERE b.TransID = a.ConjoinTransID
			)
		) > 0
    )
    BEGIN
      RAISERROR ('Failure to find matching conjoin Transaction Header pair', 16, 1);
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
    + 'Procedure Name: [staging].[usp_SetTransactionHeaderConjoinReferences]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END