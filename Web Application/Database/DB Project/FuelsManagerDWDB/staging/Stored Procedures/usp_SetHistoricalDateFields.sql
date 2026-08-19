/*
	DROP PROCEDURE [Staging].[usp_SetHistoricalDateFields]

	EXEC [staging].[usp_SetHistoricalDateFields]
	
*/
CREATE PROCEDURE [staging].[usp_SetHistoricalDateFields]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetHistoricalDateFields]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the StartDate and EndDate fields for each historical record captured in staging.
  -- Notes:
  -- 1. This process is limited to tables for which historical records are captured on the OLTP database.
  -- 2. Those historical tables have some characteristics that require them to be maintained differently in the data-warehouse database:
  --    a. They can carry more than one record change history for each entity, where each change history
  --       other than the last one has to be date bound and end-dated. This is different to the regular
  --       entity tables, where we are only concerned with the latest change to each record.
  --    b. They carry two date fields, StartDate and EndDate. Those fields have to be set using preferably the RecordedUpdatedDate if available. 
  --       If the RecordUpdatedDate is unavailable, e.g. for a manual ETL run, the UpdatedDate field needs to be used to set the StartDate and EndDate fields.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Product
    TRUNCATE TABLE staging.tblEntityDateRange
    -- Set the StartDate and EndDate in the Staging table with the use of a dynamic table that is populated in the order in which the changes were actually recorded in the OLTP database for each Entity record
    INSERT INTO staging.tblEntityDateRange (EntitySKey, EntityKey, UpdatedDate)
      SELECT
        SKey,
        ProductKey,
        COALESCE(RecordUpdatedDate, UpdatedDate)
      FROM staging.tblProducts
      WHERE IgnoreRecord = 0
      ORDER BY ProductKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE a
    SET a.EndDate = b.UpdatedDate
    FROM staging.tblEntityDateRange a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex + 1)

    UPDATE a
    SET a.StartDate = b.UpdatedDate,
        a.EndDate = b.EndDate
    FROM staging.tblProducts a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntitySKey = a.SKey
    WHERE (ISNULL(a.IsRecordAddedByETL, 0) = 0) --Ignore artificially added entity records whose StartDate and EndDate have already been set.
    OR ((ISNULL(a.IsRecordAddedByETL, 0) = 1)
    AND (a.StartDate IS NULL)
    AND (a.EndDate IS NULL))

	-- Set the StartDate of the first version of a record as its CreatedDate
	UPDATE a 	
    SET a.StartDate = a.CreatedDate
	FROM staging.tblProducts a
	INNER JOIN
	(
		SELECT b.ProductKey, MIN(StartDate) StartDate FROM staging.tblProducts b
		GROUP BY b.ProductKey
	)x
	ON x.ProductKey = a.ProductKey
	AND x.StartDate = a.StartDate
    WHERE NOT EXISTS
	(
		SELECT * FROM DimProduct c
		WHERE c.AKey = a.ProductKey
	)
	AND a.CreatedDate < a.StartDate



    -- Company
    TRUNCATE TABLE staging.tblEntityDateRange
    -- Set the StartDate and EndDate in the Staging table with the use of a dynamic table that is populated in the order in which the changes were actually recorded in the OLTP database for each Entity record
    INSERT INTO staging.tblEntityDateRange (EntitySKey, EntityKey, UpdatedDate)
      SELECT
        SKey,
        CompanyKey,
        COALESCE(RecordUpdatedDate, UpdatedDate)
      FROM staging.tblCompanies
      WHERE IgnoreRecord = 0
      ORDER BY CompanyKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE a
    SET a.EndDate = b.UpdatedDate
    FROM staging.tblEntityDateRange a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex + 1)

    UPDATE a
    SET a.StartDate = b.UpdatedDate,
        a.EndDate = b.EndDate
    FROM staging.tblCompanies a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntitySKey = a.SKey
    WHERE (ISNULL(a.IsRecordAddedByETL, 0) = 0) --Ignore artificially added entity records whose StartDate and EndDate have already been set.
    OR ((ISNULL(a.IsRecordAddedByETL, 0) = 1)
    AND (a.StartDate IS NULL)
    AND (a.EndDate IS NULL))

	-- Set the StartDate of the first version of a record as its CreatedDate
	UPDATE a 	
    SET a.StartDate = a.CreatedDate
	FROM staging.tblCompanies a
	INNER JOIN
	(
		SELECT b.CompanyKey, MIN(StartDate) StartDate FROM staging.tblCompanies b
		GROUP BY b.CompanyKey
	)x
	ON x.CompanyKey = a.CompanyKey
	AND x.StartDate = a.StartDate
    WHERE NOT EXISTS
	(
		SELECT * FROM DimCompany c
		WHERE c.AKey = a.CompanyKey
	)
	AND a.CreatedDate < a.StartDate


    -- Equipment
    TRUNCATE TABLE staging.tblEntityDateRange
    -- Set the StartDate and EndDate in the Staging table with the use of a dynamic table that is populated in the order in which the changes were actually recorded in the OLTP database for each Entity record
    INSERT INTO staging.tblEntityDateRange (EntitySKey, EntityKey, UpdatedDate)
      SELECT
        SKey,
        EquipmentKey,
        COALESCE(RecordUpdatedDate, UpdatedDate)
      FROM staging.tblEquipment
      WHERE IgnoreRecord = 0
      ORDER BY EquipmentKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE a
    SET a.EndDate = b.UpdatedDate
    FROM staging.tblEntityDateRange a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex + 1)

    UPDATE a
    SET a.StartDate = b.UpdatedDate,
        a.EndDate = b.EndDate
    FROM staging.tblEquipment a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntitySKey = a.SKey
    WHERE (ISNULL(a.IsRecordAddedByETL, 0) = 0) --Ignore artificially added entity records whose StartDate and EndDate have already been set.
    OR ((ISNULL(a.IsRecordAddedByETL, 0) = 1)
    AND (a.StartDate IS NULL)
    AND (a.EndDate IS NULL))

	-- Set the StartDate of the first version of a record as its CreatedDate
	UPDATE a 	
    SET a.StartDate = a.CreatedDate
	FROM staging.tblEquipment a
	INNER JOIN
	(
		SELECT b.EquipmentKey, MIN(StartDate) StartDate FROM staging.tblEquipment b
		GROUP BY b.EquipmentKey
	)x
	ON x.EquipmentKey = a.EquipmentKey
	AND x.StartDate = a.StartDate
    WHERE NOT EXISTS
	(
		SELECT * FROM DimEquipment c
		WHERE c.AKey = a.EquipmentKey
	)
	AND a.CreatedDate < a.StartDate



    -- Transaction Alias
    TRUNCATE TABLE staging.tblEntityDateRange
    -- Set the StartDate and EndDate in the Staging table with the use of a dynamic table that is populated in the order in which the changes were actually recorded in the OLTP database for each Entity record
    INSERT INTO staging.tblEntityDateRange (EntitySKey, EntityKey, UpdatedDate)
      SELECT
        SKey,
        TransactionAliasKey,
        COALESCE(RecordUpdatedDate, UpdatedDate)
      FROM staging.tblTransactionAliases
      WHERE IgnoreRecord = 0
      ORDER BY TransactionAliasKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE a
    SET a.EndDate = b.UpdatedDate
    FROM staging.tblEntityDateRange a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex + 1)

    UPDATE a
    SET a.StartDate = b.UpdatedDate,
        a.EndDate = b.EndDate
    FROM staging.tblTransactionAliases a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntitySKey = a.SKey
    WHERE (ISNULL(a.IsRecordAddedByETL, 0) = 0) --Ignore artificially added entity records whose StartDate and EndDate have already been set.
    OR ((ISNULL(a.IsRecordAddedByETL, 0) = 1)
    AND (a.StartDate IS NULL)
    AND (a.EndDate IS NULL))

	-- Set the StartDate of the first version of a record as its CreatedDate
	UPDATE a 	
    SET a.StartDate = a.CreatedDate
	FROM staging.tblTransactionAliases a
	INNER JOIN
	(
		SELECT b.TransactionAliasKey, MIN(StartDate) StartDate FROM staging.tblTransactionAliases b
		GROUP BY b.TransactionAliasKey
	)x
	ON x.TransactionAliasKey = a.TransactionAliasKey
	AND x.StartDate = a.StartDate
    WHERE NOT EXISTS
	(
		SELECT * FROM DimTransactionAlias c
		WHERE c.AKey = a.TransactionAliasKey
	)
	AND a.CreatedDate < a.StartDate


    -- Personnel
    TRUNCATE TABLE staging.tblEntityDateRange
    -- Set the StartDate and EndDate in the Staging table with the use of a dynamic table that is populated in the order in which the changes were actually recorded in the OLTP database for each Entity record
    INSERT INTO staging.tblEntityDateRange (EntitySKey, EntityKey, UpdatedDate)
      SELECT
        SKey,
        PersonnelKey,
        COALESCE(RecordUpdatedDate, UpdatedDate)
      FROM staging.tblPersonnel
      WHERE IgnoreRecord = 0
      ORDER BY PersonnelKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    UPDATE a
    SET a.EndDate = b.UpdatedDate
    FROM staging.tblEntityDateRange a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex + 1)

    UPDATE a
    SET a.StartDate = b.UpdatedDate,
        a.EndDate = b.EndDate
    FROM staging.tblPersonnel a
    INNER JOIN staging.tblEntityDateRange b
      ON b.EntitySKey = a.SKey
    WHERE (ISNULL(a.IsRecordAddedByETL, 0) = 0) --Ignore artificially added entity records whose StartDate and EndDate have already been set.
    OR ((ISNULL(a.IsRecordAddedByETL, 0) = 1)
    AND (a.StartDate IS NULL)
    AND (a.EndDate IS NULL))

	-- Set the StartDate of the first version of a record as its CreatedDate
	UPDATE a 	
    SET a.StartDate = a.CreatedDate
	FROM staging.tblPersonnel a
	INNER JOIN
	(
		SELECT b.PersonnelKey, MIN(StartDate) StartDate FROM staging.tblPersonnel b
		GROUP BY b.PersonnelKey
	)x
	ON x.PersonnelKey = a.PersonnelKey
	AND x.StartDate = a.StartDate
    WHERE NOT EXISTS
	(
		SELECT * FROM DimPersonnel c
		WHERE c.AKey = a.PersonnelKey
	)
	AND a.CreatedDate < a.StartDate

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
    + 'Procedure Name: [staging].[usp_SetHistoricalDateFields]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END