/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionUserData]

	EXEC [staging].[usp_LoadTransactionUserData] 0, 0

	EXEC [staging].[usp_LoadTransactionUserData] 200000, 400000
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionUserData]
(
	@startSKey  int,
	@endSKey int
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadTransactionUserData]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads Transaction UserData records from staging into the FactTransaction table in the OLAP database.
  -- Notes:
  -- 1. @startSKey: TransactionUserDataSKey from which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 2. @endSKey: TransactionUserDataSKey to which to filter the records to be loaded. Leave as 0 to ignore this filter.
  -- 3. The @startSKey and @endSKey parameters allow the loading process to be carried out in batches, if necessary.
  -- 4. The Transaction UserData Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency to a level 2 table.
  -- 5. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 6. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 7. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 8. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  --    If the transaction record is soft deleted or physically deleted in the OLTP system, then it is physically deleted in FactTransaction.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'

    DECLARE @dummyDateSKey int = 19000101
    DECLARE @defaultTimeSKey int = 0
    DECLARE @defaultBitValue bit = 0


    -- tblTransactionUserData Inserts, Updates, and Deletes are all translated into FactTransaction Updates, making it more 
    -- difficult to use the Merge statement, hence the use of regular Update statements.
    -- Process tblTransactionUserData record inserts and updates
    IF ((SELECT COUNT(*) FROM staging.tblTransactionUserData WHERE IsProcessed = 0) > 0)
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
          tgt.[TransactionUserDataKey] = src.[TransactionUserDataKey],
          tgt.[UData_UserData2] = TRIM(ISNULL(src.[UserData2], @dummyId)),
          tgt.[UData_UserData3] = TRIM(ISNULL(src.[UserData3], @dummyId)),
          tgt.[UData_UserData4SI] = ISNULL(src.[UserData4SI], 0),
          tgt.[UData_UserData4USGallon] = ISNULL(src.[UserData4USGallon], 0),
          tgt.[UData_UserData5SI] = ISNULL(src.[UserData5SI], 0),
          tgt.[UData_UserData5USGallon] = ISNULL(src.[UserData5USGallon], 0),
          tgt.[UData_UserData6SI] = ISNULL(src.[UserData6SI], 0),
          tgt.[UData_UserData6USGallon] = src.[UserData6USGallon],
          tgt.[UData_UserData23] = TRIM(ISNULL(src.[UserData23], @dummyId))
      FROM FactTransaction tgt
      INNER JOIN staging.tblTransactionUserData AS src
        ON src.TransactionKey = tgt.TransactionKey
      WHERE src.IgnoreRecord = 0
      AND src.IsRecordDeleted = 0
      AND src.IsRecordAddedByETL = 0
      AND src.IsProcessed = 0
	  AND ((src.SKey >= @startSKey) OR (ISNULL(@startSKey, 0) = 0))
	  AND ((src.SKey <= @endSKey) OR (ISNULL(@endSKey, 0) = 0))
    END

    --Process tblTransactionUserData record deletions
    IF ((SELECT COUNT(*) FROM staging.tblTransactionUserData WHERE IsRecordDeleted = 1) > 0)
    BEGIN
      UPDATE tgt
      SET tgt.[_RecordUpdatedDate] =
            CASE
                WHEN (src.[CombinedUpdatedDate] > tgt.[_RecordUpdatedDate]) THEN src.[CombinedUpdatedDate]
                ELSE tgt.[_RecordUpdatedDate]
            END,
          tgt.[TransactionUserDataKey] = NULL,
          tgt.[UData_UserData2] = @dummyId,
          tgt.[UData_UserData3] = @dummyId,
          tgt.[UData_UserData4SI] = 0,
          tgt.[UData_UserData4USGallon] = 0,
          tgt.[UData_UserData23] = @dummyId
      FROM FactTransaction tgt
      INNER JOIN staging.tblTransactionUserData AS src
        ON src.TransactionUserDataKey = tgt.TransactionUserDataKey
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
    + 'Procedure Name: [staging].[usp_LoadTransactionUserData]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END