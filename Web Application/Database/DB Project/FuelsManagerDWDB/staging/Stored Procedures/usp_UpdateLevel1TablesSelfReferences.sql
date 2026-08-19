/*
	DROP PROCEDURE [Staging].[usp_UpdateLevel1TablesSelfReferences]

	EXEC [staging].[usp_UpdateLevel1TablesSelfReferences]
	
*/
CREATE PROCEDURE [staging].[usp_UpdateLevel1TablesSelfReferences] (@IgnoreDateMismatch bit)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_UpdateLevel1TablesSelfReferences]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Updates the references that any level 1 table maintains with itself.
  -- Notes:
  -- 1. This Stored Procedure is RecordVersioning-aware, i.e. it sets the references with the specific RecordVersion key, wherever the referenced dimension supports RecordVersioning.
  -- 2. Level 1 tables are those tables that have a foreign key dependency to a level 0 table, e.g. dimProduct has a reference to dimSite.
  -- 3. All references maintained to the same table are implemented as nullable fields, allowing all the records, with the self-reference field set as null, whether the record contains a self-reference or not.
  -- 4. This procedure assumes that the Level1 records have already been inserted. This procedure is called to simply update those Level1 records which did have a self-reference using the appropriate identity keys of records previously added (either in the same ETL run or in past ETL runs).
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

		DECLARE @openEndDate datetimeoffset(7)
		SET @openEndDate = '01/01/3000'

		
		--Product
		--Identify the OLAP identity key of the TrackingProduct self-reference field.
		UPDATE a 
		SET a.TrackingProductSKey = x.RecordVersionSKey
		FROM staging.tblProducts a
		INNER JOIN 
		(
			SELECT b.ProductKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblProducts b
			INNER JOIN map.tblProductToSiteRecordVersion c
			ON c.RecordVersionKey = b.TrackingProductKey  -- a Product record will refer to a TrackingProduct using the exact RecordVersionKey (not the MasterRecordKey) because TrackingProduct is an external attribute of Product, which is also a Product, i.e. also under Record Versioining.
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.StartDate BETWEEN c.StartDate AND ISNULL(c.EndDate, @openEndDate)
			AND b.TrackingProductSKey IS NULL
			AND b.TrackingProductKey IS NOT NULL	
			GROUP BY b.ProductKey
		) x
		ON x.ProductKey = a.ProductKey
		WHERE a.IgnoreRecord = 0

		
		IF (@IgnoreDateMismatch = 1)
		BEGIN
			UPDATE a 
			SET a.TrackingProductSKey = b.RecordVersionSKey
			FROM staging.tblProducts a
			INNER JOIN 
			(
				SELECT RecordVersionKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey 
				FROM map.tblProductToSiteRecordVersion 
				GROUP BY RecordVersionKey, SiteSKey
			) b
			ON b.RecordVersionKey = a.TrackingProductKey -- a Product record will refer to a TrackingProduct using the exact RecordVersionKey (not the MasterRecordKey) because TrackingProduct is an external attribute of Product, which is also a Product, i.e. also under Record Versioining.
			AND b.SiteSKey = a.SiteSKey
			WHERE a.IgnoreRecord = 0
			AND a.TrackingProductSKey IS NULL
		END		
		
		-- Set the TrackingProductId
		UPDATE a
		SET a.TrackingProductId = b.ProductId
		FROM staging.tblProducts a
		INNER JOIN dbo.DimProduct b
		ON b.SKey = a.TrackingProductSKey
		WHERE a.IgnoreRecord = 0
		AND a.TrackingProductSKey IS NOT NULL
		AND a.TrackingProductId IS NULL									
		

		-- Set the missing TrackingProduct self-reference on the OLAP table
		UPDATE a
		SET a.TrackingProductSKey = b.TrackingProductSKey,
		a.TrackingProductId = b.TrackingProductID
		FROM dbo.DimProduct a
		INNER JOIN staging.tblProducts b
		ON b.ProductKey = a.AKey
		AND b.StartDate = a.StartDate
		WHERE b.TrackingProductSKey IS NOT NULL
		AND ISNULL(a.TrackingProductSKey, 0) = 0




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
    + 'Procedure Name: [staging].[usp_UpdateLevel1TablesSelfReferences]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
