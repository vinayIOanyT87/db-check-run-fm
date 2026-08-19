/*
	DROP PROCEDURE [staging].[usp_ResetStagingTables]

	EXEC [staging].[usp_ResetStagingTables] 'AllScopes'

	EXEC [staging].[usp_ResetStagingTables] 'TransactionScope'
	
*/
CREATE PROCEDURE [staging].[usp_ResetStagingTables]
(
	@ArchiveScope VARCHAR(50)
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetStagingTables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Deletes all the records from the staging tables for a given archive scope, or for all archive scopes.
  -- Notes:
  -- 1. @ArchiveScope: 'TransactionScope', 'AuditLogScope', 'AlarmAndEventLogScope', 'AllScopes'
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

	IF ((@ArchiveScope = 'AlarmAndEventLogScope') OR (@ArchiveScope = 'AllScopes'))
	BEGIN
		TRUNCATE TABLE [staging].[tblAlarmAndEventLog]
	END

	IF ((@ArchiveScope = 'AuditLogScope') OR (@ArchiveScope = 'AllScopes'))
	BEGIN
		TRUNCATE TABLE [staging].[tblAuditLog]
	END

	IF ((@ArchiveScope = 'TransactionScope') OR (@ArchiveScope = 'AllScopes'))
	BEGIN
		TRUNCATE TABLE [staging].[tblExportResultDetails]

		TRUNCATE TABLE [staging].[tblExportResults]

		TRUNCATE TABLE [staging].[tblTransactionLineItems]

		TRUNCATE TABLE [staging].[tblTransactionLineItemUserData]

		TRUNCATE TABLE [staging].[tblTransactionLinks]

		TRUNCATE TABLE [staging].[tblTransactionNotes]

		TRUNCATE TABLE [staging].[tblTransactionPIDX]

		TRUNCATE TABLE [staging].[tblTransactions]

		TRUNCATE TABLE [staging].[tblTransactionSignature]

		TRUNCATE TABLE [staging].[tblTransactionSubLineItems]

		TRUNCATE TABLE [staging].[tblTransactionTransportLineItems]

		TRUNCATE TABLE [staging].[tblTransactionUserData]

		TRUNCATE TABLE [staging].[tblTransactionWeightReadings]
	END

	IF (@ArchiveScope = 'AllScopes')
	BEGIN
		TRUNCATE TABLE [staging].[tblInsertedRecords]

		TRUNCATE TABLE [staging].[tblUpdatedRecords]
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
    + 'Procedure Name: [staging].[usp_ResetStagingTables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END