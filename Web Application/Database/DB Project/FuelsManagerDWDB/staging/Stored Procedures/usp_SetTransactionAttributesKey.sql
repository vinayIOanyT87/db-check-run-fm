/*
	DROP PROCEDURE [Staging].[usp_SetTransactionAttributesKey]

	EXEC [staging].[usp_SetTransactionAttributesKey]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionAttributesKey]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionAttributesKey]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Update the TransactionAttrubutes Key for the Transactions processed in the current batch.
  -- Notes:
  -- 1. The Transaction ExportInterfaceResults UserData Table is a Level 3 table. Level 3 tables are those tables that have a foreign key dependency to a level 2 table.
  -- 2. The Level 2 references have to be first sorted out before Level 3 tables can be safely loaded from staging into the OLAP database.
  -- 3. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 4. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 5. No historical data maintained for FactTransaction. Simply update the existing record if found, otherwise insert a new one.
  --    If the transaction record is soft deleted or physically deleted in the OLTP system, then it is physically deleted in FactTransaction.
  -- 6. This operation does not update the Transaction _RecordUpdatedDate field when a new EBSExportInterfaceResults record is received against the Transaction. 
  --    EBS processing/operation dates are captured in separate fields of the FactTransaction, and do not have to overwrite the Transaction Line/SubLine item dates.
  ------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
		DECLARE @shortDummyId varchar(4) = '<NA>'
		DECLARE @veryShortDummyId varchar(2) = 'NA'

		UPDATE a
		SET a.DimTransactionAttributesSKey = b.SKey
		FROM staging.tblTransactionAttributes a
		INNER JOIN dbo.DimTransactionAttributes b
		ON b.DeleteFlag = ISNULL(a.DeleteFlag, 0)
		AND b.ReversalType = ISNULL(a.ReversalType, @veryShortDummyId)
		AND b.SubType = ISNULL(a.SubType, @dummyId)
		AND b.TransactionStatusName = ISNULL(a.TransactionStatusName, @dummyId)
		AND b.InvalidTerminalTime = ISNULL(a.InvalidTerminalTime, 0)
		AND b.GrossQuantitySign = ISNULL(a.GrossQuantitySign, @shortDummyId)

		AND b.IsRecordDeleted = ISNULL(a.IsRecordDeleted, 0)
		WHERE a.IgnoreRecord = 0

		IF ((SELECT COUNT(*) FROM staging.tblTransactionAttributes WHERE DimTransactionAttributesSKey IS NULL
				AND IgnoreRecord = 0) > 0)
		BEGIN
			RAISERROR ('Failure to resolve Transactions-to-TransactionAttributes references', 16, 1);
			RETURN;
		END


		UPDATE a
		SET a.DimTransactionAttributesSKey = b.SKey
		FROM staging.tblTransactionSummaryAttributes a
		INNER JOIN dbo.DimTransactionAttributes b
		ON b.DeleteFlag = ISNULL(a.DeleteFlag, 0)
		AND b.ReversalType = ISNULL(a.ReversalType, @veryShortDummyId)
		AND b.SubType = ISNULL(a.SubType, @dummyId)
		AND b.TransactionStatusName = ISNULL(a.TransactionStatusName, @dummyId)
		AND b.InvalidTerminalTime = ISNULL(a.InvalidTerminalTime, 0)
		AND b.GrossQuantitySign = @shortDummyId
		AND b.IsRecordDeleted = ISNULL(a.IsRecordDeleted, 0)
		WHERE a.IgnoreRecord = 0

		IF ((SELECT COUNT(*) FROM staging.tblTransactionSummaryAttributes WHERE DimTransactionAttributesSKey IS NULL
				AND IgnoreRecord = 0) > 0)
		BEGIN
			RAISERROR ('Failure to resolve TransactionSummary-to-TransactionAttributes references', 16, 1);
			RETURN;
		END


		-- Update the TransactionAttributesKey of the FactTransaction records added or modified in the current batch
		UPDATE a
		SET	a.[TransactionAttributesSKey] = b.[DimTransactionAttributesSKey]
		FROM dbo.FactTransaction a
		INNER JOIN staging.tblTransactionAttributes b
		ON b.FactTransactionSKey = a.SKey

		-- Update the TransactionAttributesKey of the FactTransactionSummary records added or modified in the current batch
		UPDATE a
		SET	a.[TransactionAttributesSKey] = b.[DimTransactionAttributesSKey]
		FROM dbo.FactTransactionSummary a
		INNER JOIN staging.tblTransactionSummaryAttributes b
		ON b.FactTransactionSummarySKey = a.SKey

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
    + 'Procedure Name: [staging].[usp_SetTransactionAttributesKey]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END