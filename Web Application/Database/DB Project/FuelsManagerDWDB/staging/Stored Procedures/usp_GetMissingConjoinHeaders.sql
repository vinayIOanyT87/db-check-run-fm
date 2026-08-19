/*
    DROP PROCEDURE [staging].[usp_GetMissingConjoinHeaders]

	EXEC [staging].[usp_GetMissingConjoinHeaders]
	
*/
CREATE PROCEDURE [staging].[usp_GetMissingConjoinHeaders]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_GetMissingConjoinHeaders]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: For all Conjoin Transaction Header records in staging, retrieve from the FactTransaction, the Header information 
  -- of the Conjoin transaction pair not found in staging.
  -- Notes:
  -- 1. The purpose of loading the missing Conjoin Header records in staging is to allow the ETL process to modify the Conjoin
  --    historical dimension references (SupplierDODAAC, BuyerDODAAC) of the Header of the Conjoin Transaction pair if necessary. 
  -- 2. This extraction is limited to Transactions of the Inflight Transaction Alias because this is the only alias which
  --    supports conjoin fields in the header.
  -- 2. Since the purpose of this extraction is limited to the update of the Conjoin fields of existing FactTransaction Header records, 
  --    the field list extracted is limited accordingly.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'

    SELECT
      a.SKey,
      a.TransID,
      a.TransactionKey,
      a.TransactionAliasSKey,
      a.OwnerCompanySkey,
      a.ConjoinTransID,
      b.CombinedUpdatedDate ConjoinHeaderCombinedUpdatedDate
    FROM FactTransaction a
    INNER JOIN staging.tblTransactions b
      ON b.ConjoinTransID = a.TransID
    WHERE b.ConjoinTransID IS NOT NULL
    AND LEN(b.ConjoinTransId) > 0
    AND b.IgnoreRecord = 0
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactions d
        WHERE d.TransactionKey = a.TransactionKey
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
    + 'Procedure Name: [staging].[usp_GetMissingConjoinHeaders]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END