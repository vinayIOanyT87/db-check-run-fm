/*
	DROP PROCEDURE [staging].[usp_MarkBatchAsComplete]

	EXEC [staging].[usp_MarkBatchAsComplete] 'TransactionBatch'
	
*/
CREATE PROCEDURE [staging].[usp_MarkBatchAsComplete]
(
	@ArchiveScope VARCHAR(50)
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_MarkBatchAsComplete]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Marks all the records in the staging tables for a given Archive Scope as complete.
  -- Notes:
  -- 1. @ArchiveScope: 'TransactionScope', 'AuditLogScope', 'AlarmAndEventLogScope'
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

	IF (@ArchiveScope = 'AlarmAndEventLogScope')
	BEGIN
		UPDATE [staging].[tblAlarmAndEventLog]
		SET IsProcessed = 1
	END

	IF (@ArchiveScope = 'AuditLogScope')
	BEGIN
		UPDATE [staging].[tblAuditLog]
		SET IsProcessed = 1
	END

	IF (@ArchiveScope = 'TransactionScope')
	BEGIN
		UPDATE [staging].[tblExportResultDetails]
		SET IsProcessed = 1

		UPDATE [staging].[tblExportResults]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionLineItems]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionLineItemUserData]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionLinks]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionNotes]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionPIDX]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactions]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionSignature]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionSubLineItems]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionTransportLineItems]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionUserData]
		SET IsProcessed = 1

		UPDATE [staging].[tblTransactionWeightReadings]
		SET IsProcessed = 1
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
    + 'Procedure Name: [staging].[usp_MarkBatchAsComplete]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END