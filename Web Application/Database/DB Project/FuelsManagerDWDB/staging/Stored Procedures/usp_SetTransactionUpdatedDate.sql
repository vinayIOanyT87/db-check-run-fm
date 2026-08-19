/*
    DROP PROCEDURE [Staging].[usp_SetTransactionUpdatedDate]

	EXEC [staging].[usp_SetTransactionUpdatedDate]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionUpdatedDate]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionUpdatedDate]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the CombinedUpdatedDate field for each Transaction record (non-historical) captured in staging.
  -- Notes:
  -- 1. This process is limited to tables for which non-historical records are captured on the OLTP database.
  -- 2. The updated date of a record can come from two sources: The UpdatedDate field of the source record, or the RecordUpdatedDate 
  --    field of the fmcdc record entry for the record. In the case of historical records, those two date sources are combined into 
  --    a single value when setting the StartDate. This Stored Procedure does the same thing for non-historical records, which do not 
  --    have a StartDate.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Transaction Header
    UPDATE staging.tblTransactions
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactions
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)

    -- Transaction Line Item
    UPDATE staging.tblTransactionLineItems
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactionLineItems
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)

    -- Transaction Sub Line Item
    UPDATE staging.tblTransactionSubLineItems
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactionSubLineItems
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)

    -- Transaction User Data
    UPDATE staging.tblTransactionUserData
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactionUserData
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)

    -- Transaction Line Item User Data
    UPDATE staging.tblTransactionLineItemUserData
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactionLineItemUserData
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)

    -- Transaction Note
    UPDATE staging.tblTransactionNotes
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE staging.tblTransactionNotes
    SET CombinedUpdatedDateSKey = [dbo].[udf_ConvertToDateKey](CombinedUpdatedDate)


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
    + 'Procedure Name: [staging].[usp_SetTransactionUpdatedDate]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END