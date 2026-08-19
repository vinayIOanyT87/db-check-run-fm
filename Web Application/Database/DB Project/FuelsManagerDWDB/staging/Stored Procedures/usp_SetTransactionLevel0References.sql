/*
	DROP PROCEDURE [Staging].[usp_SetTransactionLevel0References]

	EXEC [staging].[usp_SetTransactionLevel0References]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionLevel0References]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionLevel0References]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets, in staging, the Transaction fields that reflect foreign key references to all Level 0 tables.
  -- Notes:
  -- 1. The foreign keys are maintained in the OLAP database tables, not in the staging tables, but in order for the staging tables to be properly loaded into the 
  --    OLAP tables, the fields in the staging tables that reflect those OLAP table foreign keys have to be preset correctly.
  -- 2. For references to historical tables, the foreign key is determined by a combination of the Identity Key (e.g. ProductKey) and the StartDate-EndDate range.
  -- 3. For references to non-historical tables, the foreign key is determined solely on the Identity Key.
  -- 4. In the case of records artificially added by the ETL, the Level 0 non-historical references might have been set already and should not be reset.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- TransactionHeader-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimSite b
      ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.SiteSKey IS NULL
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTransactions
            WHERE SiteKey IS NOT NULL
            AND SiteSKey IS NULL
            AND IgnoreRecord = 0
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve TransactionHeader-to-Site references', 16, 1);
      RETURN;
    END


    -- TransactionHeader-to-TransactionType references
    UPDATE a
    SET a.TransactionTypeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimTransactionType b
    ON b.AKey = a.TransactionTypeKey
    WHERE a.IgnoreRecord = 0
    AND a.TransactionTypeSKey IS NULL
    AND b.SKey > 0

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTransactions
            WHERE TransactionTypeKey IS NOT NULL
            AND TransactionTypeSKey IS NULL
            AND IgnoreRecord = 0
         ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve TransactionHeader-to-TransactionType references', 16, 1);
      RETURN;
    END


    -- TransactionLineItem-to-Site references
	-- TransactionLineItem does not have a Site reference. It is added here and replicated from the transaction header to help support Record Versioning dereferencing on the LineItems entity elements.
    UPDATE a
    SET a.SiteSKey = b.SiteSKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE a.IgnoreRecord = 0
    AND a.SiteSKey IS NULL
	AND b.IgnoreRecord = 0

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTransactionLineItems
            WHERE SiteSKey IS NULL
            AND IgnoreRecord = 0
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve TransactionLineItem-to-Site references', 16, 1);
      RETURN;
    END


    -- TransactionSubLineItem-to-Site references
	-- TransactionSubLineItem does not have a Site reference. It is added here and replicated from the transaction header to help support Record Versioning dereferencing on the SubLineItems entity elements.
    UPDATE a
    SET a.SiteSKey = b.SiteSKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE a.IgnoreRecord = 0
    AND a.SiteSKey IS NULL
	AND b.IgnoreRecord = 0

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTransactionSubLineItems
            WHERE SiteSKey IS NULL
            AND IgnoreRecord = 0
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve TransactionSubLineItem-to-Site references', 16, 1);
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
    + 'Procedure Name: [staging].[usp_SetTransactionLevel0References]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END