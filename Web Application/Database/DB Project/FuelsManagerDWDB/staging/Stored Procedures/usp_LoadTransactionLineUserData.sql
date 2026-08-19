/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionLineUserData]

	EXEC [staging].[usp_LoadTransactionLineUserData]

	EXEC [staging].[usp_LoadTransactionLineUserData] 200000, 400000
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionLineUserData]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadTransactionLineUserData]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction LineItem UserData records from staging into the FactTransaction table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionLineItemUserDataSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionLineItemUserDataSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 1. The Transaction LineItem UserData Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency 
  --    to a level 2 table.
  -- 2. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 3. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 4. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 5. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  --    If the transaction record is soft deleted or physically deleted in the OLTP system, then it is physically deleted in FactTransaction.
  -- 6. Unlike LineItems, SubLineItems, and stand-alone headers (i.e. headers without lineItems) which translate into individual 
  --    FactTransaction records, tblTransactionLineItemUserData are considered as supporting LineItem data, and 
  --    tblTransactionLineItemUserData additions are never translated into new FactTransaction records. 
  --    tblTransactionLineItemUserData Inserts, Updates, and Deletes are all translated into FactTransaction Updates, hence the use 
  --    of regular Update statements (instead of the Merge statement) to process tblTransactionLineItemUserData record changes.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'

    DECLARE @dummyDateSKey int = 19000101
    DECLARE @defaultTimeSKey int = 0


    IF ((SELECT COUNT(*) FROM staging.tblTransactionLineItemUserData WHERE IsProcessed = 0) > 0)
    BEGIN
      UPDATE tgt
      SET tgt.[_RecordUpdatedDate] =
            CASE
                WHEN (src.[CombinedUpdatedDate] > tgt.[_RecordUpdatedDate]) THEN src.[CombinedUpdatedDate]
                ELSE tgt.[_RecordUpdatedDate]
            END,
          tgt.[_RecordUpdatedDateSKey] =
            CASE
                WHEN (src.[CombinedUpdatedDate] > tgt.[_RecordUpdatedDate]) THEN src.[CombinedUpdatedDateSKey]
                ELSE tgt.[_RecordUpdatedDateSKey]
            END,
          tgt.[TransactionLineItemUserDataKey] = src.[TransactionLineItemUserDataKey],
          tgt.[LineUData_UserData1] = TRIM(ISNULL(src.[UserData1], @dummyId))
      FROM FactTransaction tgt
      INNER JOIN staging.tblTransactionLineItemUserData AS src
        ON src.TransactionLineItemKey = tgt.TransactionLineItemKey
      WHERE src.IgnoreRecord = 0
      AND src.IsRecordDeleted = 0
      AND src.IsRecordAddedByETL = 0
      AND src.IsProcessed = 0
	  AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	  AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
    END

    --Process tblTransactionLineItemUserData record deletions
    IF ((SELECT COUNT(*) FROM staging.tblTransactionLineItemUserData WHERE IsRecordDeleted = 1) > 0)
    BEGIN
      UPDATE tgt
      SET tgt.[_RecordUpdatedDate] =
            CASE
                WHEN (src.[CombinedUpdatedDate] > tgt.[_RecordUpdatedDate]) THEN src.[CombinedUpdatedDate]
                ELSE tgt.[_RecordUpdatedDate]
            END,
          tgt.[TransactionLineItemUserDataKey] = NULL,
          tgt.[LineUData_UserData1] = @dummyId
      FROM FactTransaction tgt
      INNER JOIN staging.tblTransactionLineItemUserData AS src
        ON src.TransactionLineItemUserDataKey = tgt.TransactionLineItemUserDataKey
      WHERE src.IgnoreRecord = 0
      AND src.IsRecordDeleted = 1
      AND src.IsRecordAddedByETL = 0
	  AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	  AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
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
    + 'Procedure Name: [staging].[usp_LoadTransactionLineUserData]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END