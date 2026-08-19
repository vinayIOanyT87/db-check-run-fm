/*
    DROP PROCEDURE [dbo].[usp_LoadTransactionTypeDimension]

	EXEC [dbo].[usp_LoadTransactionTypeDimension]
	
*/
CREATE PROCEDURE [dbo].[usp_LoadTransactionTypeDimension]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_LoadTransactionTypeDimension]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads the dimTransactionType dimension table, with data from the lookup table.
  -- Notes:
  -- 1. The TransactionType needs to be captured into a separate dimension in order to support the Book Inventory Calculated Measures that 
  --    refer to specific Transaction Types.
  -- 2. The Transaction Type is originally captured in a lookup table, where it is not expected to be changed by the user, just like with the
  --    other look up data. As such the loading of dimTransactionType is performed as a one time exercise, as part of the look up data loading.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId nvarchar(20) = '<NOT AVAILABLE>'

    TRUNCATE TABLE dbo.DimTransactionType

    SET IDENTITY_INSERT DimTransactionType ON

    INSERT INTO dbo.DimTransactionType ([SKey], [AKey], [TransactionTypeCode], [TransactionTypeName])   --AKeyz
      VALUES (0, @dummyId, @dummyId, @dummyId)

    SET IDENTITY_INSERT dbo.DimTransactionType OFF

    INSERT INTO dbo.DimTransactionType (AKey, TransactionTypeCode, TransactionTypeName)   --AKeyz
      SELECT
        CONVERT(nvarchar(50), LookupIndex),
        ISNULL(LookupCode, @dummyId),
        ISNULL(LookupName, @dummyId)
      FROM lookup.tblLookup
      WHERE LookupType = 'TransactionTypes'

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
    + 'Procedure Name: [dbo].[usp_LoadTransactionTypeDimension]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END