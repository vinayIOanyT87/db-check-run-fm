/*
	DROP PROCEDURE [dbo].[usp_CloseAuditLogRecord]

    EXEC [dbo].[usp_CloseAuditLogRecord] 73553, NULL
	EXEC [dbo].[usp_CloseAuditLogRecord] 73553, 'Test'
	
*/
CREATE PROCEDURE [dbo].[usp_CloseAuditLogRecord]
(
	@AuditKey  bigint,
	@AuditNote nvarchar(max)
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_CloseAuditLogRecord]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Closes a given tblETLAudit record.
  -- Notes:
  -- 1. @AuditKey: The AuditKey of the record to be closed.
  -- 2. @AuditNote: A text to be used to set the AuditNote on the record to be closed
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
	DECLARE @note varchar(1000) 

    IF (LEN(@AuditNote) > 1000)
    BEGIN
        SET @note = SUBSTRING(@AuditNote, 1, 995) + '[...]'
    END
    ELSE
    BEGIN
        SET @note = @AuditNote
    END

	UPDATE dbo.tblETLAudit SET ExecStopDT = getdate()
	, SuccessfulProcessingInd = 'N'
	, AuditNote = @note
	WHERE 
	AuditKey = @AuditKey

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
    + 'Procedure Name: [dbo].[usp_CloseAuditLogRecord]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END