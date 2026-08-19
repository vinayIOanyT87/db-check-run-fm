/*
    DROP PROCEDURE [Staging].[usp_SetLookupReferencesForSubLineItems]

	EXEC [staging].[usp_SetLookupReferencesForSubLineItems]
	
*/
CREATE PROCEDURE [staging].[usp_SetLookupReferencesForSubLineItems]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetLookupReferencesForSubLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets, in staging, the value of the corresponding name field for each lookup index field for table tblTransactionLineItems. 
  -- E.g. using the staging.tblProducts.ProductTypeIndex field value, look up the corresponding lookup.tblLookup.name field value, and use that value to set the staging.tblProducts.ProductTypeName field value.
  -- Notes:
  -- 1. In the data warehouse database, the majority of the lookup tables have been consolidated into a single lookup table, lookup.tblLookup, using a LookupType field to differentiate between the different lookups.
  -- 2. The OLAP database does not maintain any relationship to lookup tables. It requires all references to lookup data to have been pre-resolved.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- TransactionLineItem Lookup references		
    UPDATE a
    SET a.QualityName = b.LookupName
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.QualityIndex
    WHERE b.LookupType = 'TransactionQuality'

    UPDATE a
    SET a.TransactionStatusName = b.LookupName
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.TransactionStatusIndex
    WHERE b.LookupType = 'TransactionStatus'

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTransactionSubLineItems
            WHERE 
            (
                QualityIndex IS NOT NULL AND QualityName IS NULL
            )
            OR 
            (
                TransactionStatusIndex IS NOT NULL AND TransactionStatusName IS NULL
            )
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve TransactionSubLineItem-Lookup references', 16, 1);
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
    + 'Procedure Name: [staging].[usp_SetLookupReferencesForSubLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END