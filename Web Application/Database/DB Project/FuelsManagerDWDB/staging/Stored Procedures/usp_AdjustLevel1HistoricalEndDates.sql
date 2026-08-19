/*
	DROP PROCEDURE [Staging].[usp_AdjustLevel1HistoricalEndDates]

	EXEC [staging].[usp_AdjustLevel1HistoricalEndDates]
	
*/
CREATE PROCEDURE [staging].[usp_AdjustLevel1HistoricalEndDates]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_AdjustLevel1HistoricalEndDates]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Adjusts the EndDate fields for each level 1 historical record captured in staging to account for any intermediate records that 
  --          might have been marked as Ignored after the StartDate and EndDate were set.
  -- Notes:
  -- 1. This process is limited to tables for which historical records are captured on the OLTP database.
  -- 2. This operation should follow the process that runs to eliminate changes to records which do not affect the supported fields in OLAP, and 
  --    which marks those records with an IgnoreRecord value of 1. The current operation adjusts the EndDate on the historical records so that
  --    no gaps are left in between the record change history, even after some of the records were makred to be Ignored.
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
    SET a.EndDate = NULL
    FROM staging.tblEntityDateRange a
    INNER JOIN 
	(
		SELECT EntityKey, MAX(RowIndex) RowIndex FROM staging.tblEntityDateRange
		GROUP BY  EntityKey
	) b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = a.RowIndex

	UPDATE a
    SET a.EndDate = b.EndDate
    FROM staging.tblProducts a
    INNER JOIN staging.tblEntityDateRange b
    ON b.EntitySKey = a.SKey


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
    SET a.EndDate = NULL
    FROM staging.tblEntityDateRange a
    INNER JOIN 
	(
		SELECT EntityKey, MAX(RowIndex) RowIndex FROM staging.tblEntityDateRange
		GROUP BY  EntityKey
	) b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = a.RowIndex

	UPDATE a
    SET a.EndDate = b.EndDate
    FROM staging.tblCompanies a
    INNER JOIN staging.tblEntityDateRange b
    ON b.EntitySKey = a.SKey


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
    SET a.EndDate = NULL
    FROM staging.tblEntityDateRange a
    INNER JOIN 
	(
		SELECT EntityKey, MAX(RowIndex) RowIndex FROM staging.tblEntityDateRange
		GROUP BY  EntityKey
	) b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = a.RowIndex

	UPDATE a
    SET a.EndDate = b.EndDate
    FROM staging.tblEquipment a
    INNER JOIN staging.tblEntityDateRange b
    ON b.EntitySKey = a.SKey


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
    SET a.EndDate = NULL
    FROM staging.tblEntityDateRange a
    INNER JOIN 
	(
		SELECT EntityKey, MAX(RowIndex) RowIndex FROM staging.tblEntityDateRange
		GROUP BY  EntityKey
	) b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = a.RowIndex

	UPDATE a
    SET a.EndDate = b.EndDate
    FROM staging.tblPersonnel a
    INNER JOIN staging.tblEntityDateRange b
    ON b.EntitySKey = a.SKey


    -- TransactionAlias
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
    SET a.EndDate = NULL
    FROM staging.tblEntityDateRange a
    INNER JOIN 
	(
		SELECT EntityKey, MAX(RowIndex) RowIndex FROM staging.tblEntityDateRange
		GROUP BY  EntityKey
	) b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = a.RowIndex

	UPDATE a
    SET a.EndDate = b.EndDate
    FROM staging.tblTransactionAliases a
    INNER JOIN staging.tblEntityDateRange b
    ON b.EntitySKey = a.SKey

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
    + 'Procedure Name: [staging].[usp_AdjustLevel1HistoricalEndDates]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END