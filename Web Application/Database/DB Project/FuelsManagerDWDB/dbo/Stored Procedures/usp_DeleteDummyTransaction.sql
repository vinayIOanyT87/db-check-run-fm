/*
	DROP PROCEDURE [dbo].[usp_DeleteDummyTransaction]

	EXEC [dbo].[usp_DeleteDummyTransaction]
	
*/
CREATE PROCEDURE [dbo].[usp_DeleteDummyTransaction]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_DeleteDummyTransaction]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Delete the dummy transaction that was included to force a DeleteFlag entry of [False], in order to support the setting of the DimTransactionAttributes.DeleteFlag.DefaultMember value during the initial ETL.
  -- Notes:
  -- Once the DeleteFlag entry of [False] has been captured in the DimTransactionAttributes dimension, the dummy entry is not required anymore in the FactTransaction table.
 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    
    --Delete the Dummy FactTransaction entry. Entry to provide a DeleteFlag entry of [False], in order to support the setting of the DimTransactionAttributes.DeleteFlag.DefaultMember value during deployment.		

    DELETE [dbo].[FactTransaction]
    WHERE TransactionKey = @dummyId


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
    + 'Procedure Name: [dbo].[usp_DeleteDummyTransaction]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
