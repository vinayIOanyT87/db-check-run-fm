/*
  DROP PROCEDURE [dbo].[usp_UpdateCDCActivationDate]

  DECLARE @dt VarChar(50)
  SELECT @dt = Convert(Varchar(50), sysdatetimeoffset ( )) 
  EXEC [dbo].[usp_UpdateCDCActivationDate] @dt

  SELECT * FROM dbo.DimSystemInfo
	
*/
CREATE PROCEDURE [dbo].[usp_UpdateCDCActivationDate] (@cdcActivationDate VARCHAR(50))
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_UpdateCDCActivationDate]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Update the CDCActivationDate field with a given datetimeoffset value provided the CDCActivationDate has not already been set.
  -- Notes:
  -- 1. @cdcActivationDate: Value to be used to set the CDCActivationdate. The value is passed as a string because the current version of SSIS does not support datetimeoffset variables.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
	DECLARE @dt datetimeoffset
	SELECT @dt = CONVERT(DATETIMEOFFSET, @cdcActivationDate) 
    UPDATE dbo.DimSystemInfo
    SET CDCActivationDate = @dt
    WHERE CDCActivationDate IS NULL
	AND @dt IS NOT NULL

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
    + 'Procedure Name: [dbo].[usp_UpdateCDCActivationDate]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
