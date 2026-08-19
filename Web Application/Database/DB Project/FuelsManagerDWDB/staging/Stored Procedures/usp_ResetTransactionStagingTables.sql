/*
	DROP PROCEDURE [staging].[usp_ResetTransactionStagingTables]

	EXEC [staging].[usp_ResetTransactionStagingTables]
	
*/
CREATE PROCEDURE [staging].[usp_ResetTransactionStagingTables]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetTransactionStagingTables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Deletes all the records from the staging Transaction tables that are used to support the data loading
  --          operation into the Fact Transaction table.
  -- Notes:
  -- 1. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    TRUNCATE TABLE [staging].[tblTransactions]

    TRUNCATE TABLE [staging].[tblTransactionLineItems]

    TRUNCATE TABLE [staging].[tblTransactionSubLineItems]

    TRUNCATE TABLE [staging].[tblTransactionUserData]

    TRUNCATE TABLE [staging].[tblTransactionLineItemUserData]

    TRUNCATE TABLE [staging].[tblTransactionNotes]

    TRUNCATE TABLE [staging].[tblInsertedLineItems]

	TRUNCATE TABLE [staging].[tblEditedFactTransaction]

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
    + 'Procedure Name: [staging].[usp_ResetTransactionStagingTables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END