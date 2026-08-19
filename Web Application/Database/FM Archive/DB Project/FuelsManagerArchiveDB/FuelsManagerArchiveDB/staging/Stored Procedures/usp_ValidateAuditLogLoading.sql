/*
	DROP PROCEDURE [staging].[usp_ValidateAuditLogLoading]

	EXEC [staging].[usp_ValidateAuditLogLoading]
	
*/
CREATE PROCEDURE [staging].[usp_ValidateAuditLogLoading]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ValidateAuditLogLoading]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Verifies that the AuditLog records in Staging can all be located in the target Archive table.
  -- Notes:
  -- 1. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
	
    IF 
	(
		(
			SELECT COUNT(*) FROM [staging].[tblAuditLog] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblAuditLog]  b
				WHERE b.AuditLogGuid = a.AuditLogGuid
			)
		) > 0
	)	
	BEGIN
		RAISERROR('AuditLog loading validation failure. Not all the AuditLog Staging records were loaded in the target Archive tables.',16,1); 
		RETURN;
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
    + 'Procedure Name: [staging].[usp_ValidateAuditLogLoading]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END