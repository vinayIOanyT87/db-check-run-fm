/*
	DROP PROCEDURE [Staging].[usp_UpdateConjoinTransactionsForHeaders]

	EXEC [staging].[usp_UpdateConjoinTransactionsForHeaders] 0, 0

	EXEC [staging].[usp_UpdateConjoinTransactionsForHeaders] 200000, 400000
	
*/
CREATE PROCEDURE [staging].[usp_UpdateConjoinTransactionsForHeaders]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_UpdateConjoinTransactionsForHeaders]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Update the Conjoin fields of existing records of the FactTransaction records that have previously been extracted from the
  --          FactTransaction table into the staging.tblTransactions.
  -- Notes:
  -- 1. @startSKey: TransactionSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. Updating the Conjoin fields of the Conjoin Transactions that were artificially added to the staging transaction tables, separately 
  --   from the Merge operation in the Header Loading operations, allows us to only update the Conjoin fields without touching the other 
  --   fields of the FactTransaction records.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    UPDATE tgt
    SET tgt.[ConjoinOwnerSKey] = ISNULL(src.[ConjoinOwnerSKey], 0),
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate]
    FROM dbo.FactTransaction tgt
    INNER JOIN staging.tblTransactions src
      ON src.SourceFactSKey = tgt.SKey
    WHERE src.ConjoinTransID IS NOT NULL
    AND LEN(src.ConjoinTransID) > 0
    AND src.IsRecordAddedByETL = 1
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
    + 'Procedure Name: [staging].[usp_UpdateConjoinTransactionsForHeaders]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END