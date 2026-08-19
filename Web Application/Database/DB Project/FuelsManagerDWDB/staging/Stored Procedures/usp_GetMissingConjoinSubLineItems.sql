/*
    DROP PROCEDURE [staging].[usp_GetMissingConjoinSubLineItems]

	EXEC [staging].[usp_GetMissingConjoinSubLineItems]
	
*/
CREATE PROCEDURE [staging].[usp_GetMissingConjoinSubLineItems]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_GetMissingConjoinSubLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: For all Conjoin Transaction Header records in staging, retrieve from the FactTransaction, the SubLineItem records 
  -- of the Conjoin transaction pair not found in staging.
  -- Notes:
  -- 1. The purpose of loading the missing Conjoin SubLineItem records in staging is to allow the ETL process to modify the Conjoin
  --    historical dimension references (e.g. ConjoinProductSKey, etc.) of the LineItems of the 
  --    Conjoin Transaction pair if necessary. 
  -- 2. Since the purpose of this extraction is limited to the update of the Conjoin fields of existing FactTransaction LineItem records, 
  --    the field list extracted is limited accordingly.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyGuid uniqueidentifier = CAST(CAST(0 as binary) as uniqueidentifier)
    DECLARE @dummyGuidKey [nvarchar](50) = CONVERT(nvarchar(50), @dummyGuid)

    SELECT
      a.SKey,
      a.TransID,
      a.TransactionAliasSKey,
      a.TransactionKey,
      a.TransactionLineItemKey,
      a.TransactionSubLineItemKey,
      a.Line_ProductSKey,
      a.Line_SequenceId,
      a.ConjoinTransID,
      a.Line_ConjoinProductSKey,
      b.CombinedUpdatedDate ConjoinHeaderCombinedUpdatedDate

    FROM FactTransaction a
    INNER JOIN staging.tblTransactions b
    ON b.ConjoinTransID = a.TransID
    INNER JOIN staging.tblTransactionLineItems c
    ON c.TransactionKey = b.TransactionKey
    INNER JOIN staging.tblTransactionSubLineItems d
    ON d.TransactionLineItemKey = c.TransactionLineItemKey
    AND d.SequenceID = a.Line_SequenceID
    WHERE b.ConjoinTransID IS NOT NULL
    AND LEN(b.ConjoinTransId) > 0
    AND a.TransactionSubLineItemKey IS NOT NULL
    AND a.TransactionSubLineItemKey <> @dummyGuidKey
    AND b.IgnoreRecord = 0
    AND c.IgnoreRecord = 0
    AND d.IgnoreRecord = 0
    AND NOT EXISTS 
    (
        SELECT *  FROM staging.tblTransactionSubLineItems d
        WHERE d.TransactionSubLineItemKey = a.TransactionSubLineItemKey
    )

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
    + 'Procedure Name: [staging].[usp_GetMissingConjoinSubLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
