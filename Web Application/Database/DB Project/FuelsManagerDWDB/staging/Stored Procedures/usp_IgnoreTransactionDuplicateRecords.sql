/*
  DROP PROCEDURE [staging].[usp_IgnoreTransactionDuplicateRecords]

	EXEC [staging].[usp_IgnoreTransactionDuplicateRecords]
	
*/
CREATE PROCEDURE [staging].[usp_IgnoreTransactionDuplicateRecords]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IgnoreTransactionDuplicateRecords]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the IgnoreRecord flag of each Transaction record in staging that comes directly from the source OLTP record when one or more fmcdc record have also been captured for the same transaction record.
  -- Notes:
  -- 1. All record changes in the OLTP database are captured by the custom Change Data Capture system (fmcdc).
  -- 2. Usually th ETL process only extracts data from the fmcdc tables, but in the case of a manual run, e.g. during an intial data loading, the ETL process also retrieves data directly from the source tables.
  --    When this happens, there is a possibility that two records will be captured in staging for a given record, one directly from the source table, and one (or more) from the fmcdc table. If an fmcdc record exist 
  --    for the entity record, then the staging record for the one captured directly from the source table can safely be ignored. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Set the IgnoreRecord flag of all non-trigger entered records for which there is a corresponding trigger-entered record (based on Identity Key matching)

    -- TransactionHeader
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactions a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactions b
        WHERE b.TransactionKey = a.TransactionKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- TransactionLineItem
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionLineItems a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionLineItems b
        WHERE b.TransactionLineItemKey = a.TransactionLineItemKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- TransactionSubLineItem
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionSubLineItems a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionSubLineItems b
        WHERE b.TransactionSubLineItemKey = a.TransactionSubLineItemKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- TransactionUserData
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionUserData a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionUserData b
        WHERE b.TransactionUserDataKey = a.TransactionUserDataKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- TransactionLineItemUserData
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionLineItemUserData a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionLineItemUserData b
        WHERE b.TransactionLineItemUserDataKey = a.TransactionLineItemUserDataKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

     -- TransactionNotes
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionNotes a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionNotes b
        WHERE b.TransactionNoteKey = a.TransactionNoteKey
        AND b.RecordUpdatedDate IS NOT NULL
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
    + 'Procedure Name: [staging].[usp_IgnoreTransactionDuplicateRecords]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END