/*
	DROP PROCEDURE [Staging].[usp_ResetHistoricalStartDates]

	EXEC [staging].[usp_ResetHistoricalStartDates]
	
*/
CREATE PROCEDURE [staging].[usp_ResetHistoricalStartDates]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetHistoricalStartDates]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets to the baseline date (the earliest of the transaction timestamp or historical entity timestamp) the StartDate of the
  --          first entry of each entity for which the StartDate is after the baseline date.
  -- Notes:
  -- 1. If a Transaction date precedes the start date of an entity record that it references that would indicate that the timestamp of the
  --    entity is wrong. The Transaction could not have been created before the Entity. This anomaly could happen as a result of data import, 
  --    and other actions, that took place before the Fuels Manager Change Data Capture was put in place. To eliminate this anomaly in
  --    the data warehouse, the Start Date of the first entry of all entity records is reset to the baseline date.
  -- 2. Similarly, if the timestamp of an entity-to-site mapping precedes the start date of a master entity record that it references that 
  --    would indicate that the timestamp of the entity is wrong. The entity-to-site mapping could not have been created before the master
  --    record Entity. This anomaly could happen as a result of data import, and other actions, that took place before the Fuels Manager 
  --    Change Data Capture was put in place. To eliminate this anomaly in the data warehouse, the Start Date of the first entry of all 
  --    entity records is reset to the baseline date.
  -- 3. The Start Date reset is only applicable to the first historical entry of any given entity record.
  -- 4. This Stored Procedure assumes that the minimum TransactionDateTime value and the minHistoricalTimestamp value have already been captured 
  --    into the staging.tblETLTempVariables table.
  -- 5. The anomalies addressed in this Stored Procedure should only occur in the initial version of the OLTP database, before the deployment
  --    and activation of the FMCDC, and as such it should only be executed on the initial ETL run.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @baselineDate datetimeoffset(7)

	SELECT @baselineDate = [staging].[udf_GetBaselineDate]()
				   
	IF (@baselineDate IS NULL)
	BEGIN
		SET @baselineDate = DATEFROMPARTS(1900, 01, 01)
	END

    -- Product    
    UPDATE a 	
    SET a.StartDate = @baselineDate
	FROM staging.tblProducts a
	INNER JOIN
	(
		SELECT b.ProductKey, MIN(StartDate) StartDate FROM staging.tblProducts b
		GROUP BY b.ProductKey
	)x
	ON x.ProductKey = a.ProductKey
	AND x.StartDate = a.StartDate
    WHERE a.StartDate > @baselineDate
	AND NOT EXISTS
	(
		SELECT * FROM DimProduct c
		WHERE c.AKey = a.ProductKey
	)
    

    -- Company
    UPDATE a 	
    SET a.StartDate = @baselineDate
	FROM staging.tblCompanies a
	INNER JOIN
	(
		SELECT b.CompanyKey, MIN(StartDate) StartDate FROM staging.tblCompanies b
		GROUP BY b.CompanyKey
	)x
	ON x.CompanyKey = a.CompanyKey
	AND x.StartDate = a.StartDate
    WHERE a.StartDate > @baselineDate
	AND NOT EXISTS
	(
		SELECT * FROM DimCompany c
		WHERE c.AKey= a.CompanyKey
	)


    -- Equipment
	UPDATE a 	
    SET a.StartDate = @baselineDate
	FROM staging.tblEquipment a
	INNER JOIN
	(
		SELECT b.EquipmentKey, MIN(StartDate) StartDate FROM staging.tblEquipment b
		GROUP BY b.EquipmentKey
	)x
	ON x.EquipmentKey = a.EquipmentKey
	AND x.StartDate = a.StartDate
    WHERE a.StartDate > @baselineDate
	AND NOT EXISTS
	(
		SELECT * FROM DimEquipment c
		WHERE c.AKey = a.EquipmentKey
	)


	-- Personnel
	UPDATE a 	
    SET a.StartDate = @baselineDate
	FROM staging.tblPersonnel a
	INNER JOIN
	(
		SELECT b.PersonnelKey, MIN(StartDate) StartDate FROM staging.tblPersonnel b
		GROUP BY b.PersonnelKey
	)x
	ON x.PersonnelKey = a.PersonnelKey
	AND x.StartDate = a.StartDate
    WHERE a.StartDate > @baselineDate
	AND NOT EXISTS
	(
		SELECT * FROM DimPersonnel c
		WHERE c.AKey = a.PersonnelKey
	)


    -- Transaction Alias
	UPDATE a 	
    SET a.StartDate = @baselineDate
	FROM staging.tblTransactionAliases a
	INNER JOIN
	(
		SELECT b.TransactionAliasKey, MIN(StartDate) StartDate FROM staging.tblTransactionAliases b
		GROUP BY b.TransactionAliasKey
	)x
	ON x.TransactionAliasKey = a.TransactionAliasKey
	AND x.StartDate = a.StartDate
    WHERE a.StartDate > @baselineDate
	AND NOT EXISTS
	(
		SELECT * FROM DimTransactionAlias c
		WHERE c.AKey = a.TransactionAliasKey
	)


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
    + 'Procedure Name: [staging].[usp_ResetHistoricalStartDates]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END