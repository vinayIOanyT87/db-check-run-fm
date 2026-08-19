/*
  DROP PROCEDURE [staging].[usp_IgnoreTransactionIntermediateRecords]

	EXEC [staging].[usp_IgnoreTransactionIntermediateRecords]
	
*/
CREATE PROCEDURE [staging].[usp_IgnoreTransactionIntermediateRecords]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IgnoreTransactionIntermediateRecords]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the IgnoreRecord flag of each intermediate record that corresponds to an OLAP measure for which historical data 
  --          is not maintained.
  -- Notes:
  -- 1. Intermediate records are records other than the latest record change captured for a given entity record. An entity record can change 
  --    multiple times in between ETL runs. The fmcdc captures all the changes, not just the latest change. For non-historical tables, the 
  --    OLAP system is only interested in the latest change for each entity record, so this procedure sets the IgnoreRecord flag of all those 
  --    intermediate records.
  -- 2. This procedure is limited to non-historical OLAP tables, i.e. tables without a StartDate-EndDate field pair.
  -- 3. Intermediate records can only be introduced by the Change Data Capture (fmcdc) tables (not from the source tables) OR from being 
  --    artificially added by the ETL to help provide missing segments of an object that is constructed from multiple tables/segments 
  --    (e.g. FactTransaction).
  --    When introduced by fmcdc, the RecordUpdateDate of intermediate records are always set (non-null).
  --    When introduced by artificial addition by the ETL, the IsRecordAddedByETL = 1.
  --	  A record is only artificially added by the ETL if not already found in the staging tables, i.e. there will not be cases where a given 
  --    record exhibit multiple entries as a result of both conditions, fmcdc entries and artificial ETL entries.
  -- 4. The determination of the latest version captured for each record is performed by the order in which the record was captured, i.e. the 
  --    CDCSKey. Determining the lastest record version based on RecordUpdatedDate does not always work, as it can generate more than one record,
  --    e.g. in the case of tblUsers, where as the InactivityLockout flag is set, a trigger is fired  to update the InactivityLockoutDate 
  --    on the SAME record. This type of trigger-based successive updates to the same record was found to generate CDC records with the same 
  --    RecordUpdatedDate.
  ------------------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- TransactionHeader
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactions a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactions b
        INNER JOIN 
        (
            SELECT TransactionKey, MAX(CDCSKey) [CDCSKey] 
            FROM staging.tblTransactions
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionKey
        ) c
        ON c.TransactionKey = b.TransactionKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- TransactionLineItem
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionLineItems a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionLineItems b
        INNER JOIN 
        (
            SELECT TransactionLineItemKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTransactionLineItems
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionLineItemKey
        ) c
        ON c.TransactionLineItemKey = b.TransactionLineItemKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- TransactionSubLineItem
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionSubLineItems a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionSubLineItems b
        INNER JOIN 
        (
            SELECT TransactionSubLineItemKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTransactionSubLineItems
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionSubLineItemKey
        ) c
        ON c.TransactionSubLineItemKey = b.TransactionSubLineItemKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- TransactionUserData
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionUserData a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionUserData b
        INNER JOIN 
        (
            SELECT TransactionUserDataKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTransactionUserData
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionUserDataKey
        ) c
        ON c.TransactionUserDataKey = b.TransactionUserDataKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- TransactionLineItemUserData
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionLineItemUserData a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionLineItemUserData b
        INNER JOIN 
        (
            SELECT TransactionLineItemUserDataKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTransactionLineItemUserData
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionLineItemUserDataKey
        ) c
        ON c.TransactionLineItemUserDataKey = b.TransactionLineItemUserDataKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- TransactionNotes
    -- Handle multiple versions of the same record as captured by fmcdc
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionNotes a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionNotes b
        INNER JOIN 
        (
            SELECT TransactionNoteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTransactionNotes
            WHERE ISNULL(IgnoreRecord, 0) <> 1
            GROUP BY TransactionNoteKey
        ) c
        ON c.TransactionNoteKey = b.TransactionNoteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
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
    + 'Procedure Name: [staging].[usp_IgnoreTransactionIntermediateRecords]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END