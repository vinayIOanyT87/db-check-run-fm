/*
  DROP PROCEDURE [dbo].[usp_UpdateDimSystemInfo]
  
  EXEC [dbo].[usp_UpdateDimSystemInfo] 23344
  
  SELECT * FROM dbo.DimSystemInfo

*/
CREATE PROCEDURE [dbo].[usp_UpdateDimSystemInfo] 
(
	@AuditKey bigint
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_UpdateDimSystemInfo]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Update the DimSystemInfo attributes according to the status of the current ETL execution.
  -- Notes:
  -- 1. @AuditKey: tblETLAudit.AuditKey field value of the current ETL execution.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
    DECLARE @execStartDT datetimeoffset(7)
    DECLARE @isCompleted bit

    SET @execStartDT = NULL
    SET @isCompleted = 0

    SELECT @execStartDT = ExecStartDT, 
			@isCompleted = (CASE WHEN (ExecStopDT IS NOT NULL AND SuccessfulProcessingInd = 'Y') THEN 1
								ELSE 0
							END)
    FROM dbo.tblETLAudit
    WHERE PkgName = 'Main'
    AND Operation = 'FMDataWarehouse'
    AND AuditKey = @AuditKey

    IF (@execStartDT IS NOT NULL)
    BEGIN
		UPDATE dbo.DimSystemInfo
		SET LastLoadDate = @execStartDT,
        LastLoadDateStr = FORMAT(@execStartDT, 'dddd, yyyy-MM-dd hh:mm:ss tt')  
		WHERE (LastLoadDate IS NULL
		OR LastLoadDate < @execStartDT)

		UPDATE dbo.DimSystemInfo
		SET FirstLoadDate = @execStartDT
		WHERE FirstLoadDate IS NULL
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
    + 'Procedure Name: [dbo].[usp_UpdateDimSystemInfo]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END