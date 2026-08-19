/*
	DROP PROCEDURE [Staging].[usp_PresetAuditLog]
 
	EXEC [staging].[usp_PresetAuditLog] 1000
 
*/
CREATE PROCEDURE [staging].[usp_PresetAuditLog]
(
	@AuditKey bigint
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_PresetAuditLog]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Set the Archiving meta-data fields for the Audit Log Records.
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

		UPDATE a 
		SET a.ArchiveDate = @archiveDate, a.ETLProcessKey = @AuditKey,
		a.[AuditedDateKey] = [staging].[udf_DateTimeToDateKey] (a.AuditedDate)
		FROM staging.tblAuditLog a		
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
		+ 'Procedure Name: [staging].[usp_PresetAuditLog]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
