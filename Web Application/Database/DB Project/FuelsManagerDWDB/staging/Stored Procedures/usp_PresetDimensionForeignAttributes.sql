/*
	DROP PROCEDURE [Staging].[usp_PresetDimensionForeignAttributes]

	EXEC [staging].[usp_PresetDimensionForeignAttributes]
	
*/
CREATE PROCEDURE [staging].[usp_PresetDimensionForeignAttributes]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_PresetDimensionForeignAttributes]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Set the extra attribute fields that were added to the dimension tables but that are not native to the 
  --          underlying OLTP tables.
  -- Notes:
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Product
    UPDATE a
    SET a.ProductTypeName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
    ON b.LookupIndex = a.LookupProductTypeIndex
    WHERE a.IgnoreRecord = 0
	AND b.LookupType = 'ProductType'
	AND a.ProductTypeName IS NULL

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
    + 'Procedure Name: [staging].[usp_PresetDimensionForeignAttributes]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO