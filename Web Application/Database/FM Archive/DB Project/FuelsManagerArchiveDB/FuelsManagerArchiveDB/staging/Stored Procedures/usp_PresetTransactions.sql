/*
	DROP PROCEDURE [Staging].[usp_PresetTransactions]
 
	EXEC [staging].[usp_PresetTransactions] 1000
 
*/
CREATE PROCEDURE [staging].[usp_PresetTransactions]
(
	@AuditKey bigint
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_PresetTransactions]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Set the Archiving meta-data fields for the Transaction records.
-- Notes:
-- 1. @AuditKey: Main tblETLAudit.AuditKey value of the ETL process under which this operation is running.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @archiveDate DatetimeOffset(7)
		SELECT @archiveDate = ExecStartDT FROM dbo.tblETLAudit
		WHERE AuditKey = @AuditKey

		IF (@archiveDate IS NULL)
		BEGIN
			SET @archiveDate = GETDATE()
		END

		--Transaction Headers
		UPDATE a 
		SET a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey,
		a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](a.InventoryDate)
		FROM staging.tblTransactions a
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction LineItems
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionLineItems a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction LineItems UserData
		UPDATE a 
		SET a.TransactionGuid = c.TransactionGuid, a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](c.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionLineItemUserData a
		INNER JOIN staging.tblTransactionLineItems b
		ON b.TransactionLineItemGuid = a.TransactionLineItemGuid
		INNER JOIN staging.tblTransactions c
		ON c.TransactionGuid = b.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction Links
		UPDATE a 
		SET a.TransactionGuid = c.TransactionGuid, a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](c.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionLinks a
		INNER JOIN staging.tblTransactionLineItems b
		ON b.TransactionLineItemGuid = a.TransactionLineItemGuid
		INNER JOIN staging.tblTransactions c
		ON c.TransactionGuid = b.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		UPDATE a 
		SET a.TransactionGuid = c.TransactionGuid, a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](c.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionLinks a
		INNER JOIN staging.tblTransactionLineItems b
		ON b.TransactionLineItemGuid = a.LinkedTransactionLineItemGuid
		INNER JOIN staging.tblTransactions c
		ON c.TransactionGuid = b.TransactionGuid
		WHERE a.TransactionGuid IS NULL
		AND a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction Notes
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionNotes a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction PIDX
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionPIDX a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction Signature
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionSignature a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid	
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0
 
		--Transaction SubLineItems
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](c.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN staging.tblTransactionLineItems b
		ON b.TransactionLineItemGuid = a.TransactionLineItemGuid
		INNER JOIN staging.tblTransactions c
		ON c.TransactionGuid = b.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction Transport LineItems
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionTransportLineItems a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Transaction UserData
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionUserData a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0
 
		--Transaction Weight Readings
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblTransactionWeightReadings a
		INNER JOIN staging.tblTransactions b
		ON b.TransactionGuid = a.TransactionGuid
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Export Result Details
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](b.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblExportResultDetails a
		INNER JOIN staging.tblTransactions b
		ON b.TransId = a.RecordId
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0

		--Export Results
		UPDATE a 
		SET a.InventoryDateKey = [staging].[udf_DateTimeToDateKey](c.InventoryDate), 
		a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey
		FROM staging.tblExportResults a
		INNER JOIN staging.tblExportResultDetails b
		ON b.ExportResultGuid = a.ExportResultGuid
		INNER JOIN staging.tblTransactions c
		ON c.TransId = b.RecordId
		WHERE a.IsProcessed = 0
		AND a.IgnoreRecord = 0


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
		+ 'Procedure Name: [staging].[usp_PresetTransactions]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
