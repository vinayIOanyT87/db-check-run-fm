/*
	DROP PROCEDURE [Staging].[usp_UpdateTransactionHeaderProcessedFlags]

	EXEC [staging].[usp_UpdateTransactionHeaderProcessedFlags] 0, 0

	EXEC [staging].[usp_UpdateTransactionHeaderProcessedFlags] 200000, 400000
	
*/
CREATE PROCEDURE [staging].[usp_UpdateTransactionHeaderProcessedFlags]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_UpdateTransactionHeaderProcessedFlags]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the IsProcessed flag of transaction Header records that have already been indirectly loaded through LineItem/SubLineItem processing.
  -- Notes:
  -- 1. @startSKey: TransactionSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. When a LineItem/SubLineItem record is used to insert (NOT update) a FactTransaction record the full elements of the FactTransaction
  --    records are set, including the Header, UserData, LineItemUserData, ExportResults, and ExportResultDetails data, making it
  --    redundant to process those same ancilliary data again during the separate processing of those entities. The IsProcessed flag
  --    provides a quick way for those separate processes to ignore those records that have already been processed.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
    -- Flag Header records that have already been processed through Line Item insertions
    UPDATE a
    SET a.IsProcessed = 1
    FROM staging.tblTransactions a
    INNER JOIN staging.tblInsertedLineItems b
    ON b.TransactionKey = a.TransactionKey
	WHERE ((a.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	AND ((a.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))


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
    + 'Procedure Name: [staging].[usp_UpdateTransactionHeaderProcessedFlags]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END