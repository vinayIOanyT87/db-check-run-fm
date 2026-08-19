/*
	DROP PROCEDURE [Staging].[usp_ResetEntityToSiteMappings]

	EXEC [staging].[usp_ResetEntityToSiteMappings]
	
*/
CREATE PROCEDURE [staging].[usp_ResetEntityToSiteMappings]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetEntityToSiteMappings]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the CreatedDate of each entity-to-site mapping to the CreatedDate of the corresponding child record version, whenever
  --          the CreatedDate of the entity-to-site mapping is greater than the CreatedDate of the corresponding child record version.
  -- Notes:
  -- 1. If the start date of a child record version precedes the entity-to-site mapping for that child record version that would indicate that 
  --    the Updated Dates in the OLTP system is wrong. The child record version could not have been created before the Entity-to-Site mapping. 
  --    This anomaly could happen as a result of data import, and other actions, that took place before the Fuels Manager Change Data Capture 
  --    was put in place. To eliminate this anomaly in the data warehouse, the timestamp of all the invalid entity-to-site mappings is reset 
  --    to the CreatedDate of the corresponding child record version.
  -- 2. The anomalies addressed in this Stored Procedure should only occur in the initial version of the OLTP database, before the deployment
  --    and activation of the FMCDC, and as such it should only be executed on the initial ETL run.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Product
	UPDATE a 
	SET a.CreatedDate = b.CreatedDate
	FROM staging.tblEntityProductToSite a
	INNER JOIN staging.tblProducts b
	ON b.MasterRecordKey = a.ProductKey
	AND b.SiteKey = a.SiteKey
	WHERE b.ProductKey <> b.MasterRecordKey
    AND a.CreatedDate > b.CreatedDate
	AND a.ProductKey IS NOT NULL 
	AND a.SiteSKey IS NOT NULL 
	AND a.IgnoreRecord = 0


    -- Company
	UPDATE a 
	SET a.CreatedDate = b.CreatedDate
	FROM staging.tblEntityCompanyToSite a
	INNER JOIN staging.tblCompanies b
	ON b.MasterRecordKey = a.CompanyKey
	AND b.SiteKey = a.SiteKey
	WHERE b.CompanyKey <> b.MasterRecordKey
    AND a.CreatedDate > b.CreatedDate
	AND a.CompanyKey IS NOT NULL 
	AND a.SiteSKey IS NOT NULL 
	AND a.IgnoreRecord = 0



    -- Equipment
	UPDATE a 
	SET a.CreatedDate = b.CreatedDate
	FROM staging.tblEntityEquipmentToSite a
	INNER JOIN staging.tblEquipment b
	ON b.MasterRecordKey = a.EquipmentKey
	AND b.SiteKey = a.SiteKey
	WHERE b.EquipmentKey <> b.MasterRecordKey
    AND a.CreatedDate > b.CreatedDate
	AND a.EquipmentKey IS NOT NULL 
	AND a.SiteSKey IS NOT NULL 
	AND a.IgnoreRecord = 0


	-- Personnel
	UPDATE a 
	SET a.CreatedDate = b.CreatedDate
	FROM staging.tblEntityPersonnelToSite a
	INNER JOIN staging.tblPersonnel b
	ON b.MasterRecordKey = a.PersonnelKey
	AND b.SiteKey = a.SiteKey
	WHERE b.PersonnelKey <> b.MasterRecordKey
    AND a.CreatedDate > b.CreatedDate
	AND a.PersonnelKey IS NOT NULL 
	AND a.SiteSKey IS NOT NULL 
	AND a.IgnoreRecord = 0


	-- Transaction Alias
	UPDATE a 
	SET a.CreatedDate = b.CreatedDate
	FROM staging.tblEntityTransactionAliasToSite a
	INNER JOIN staging.tblTransactionAliases b
	ON b.MasterRecordKey = a.TransactionAliasKey
	AND b.SiteKey = a.SiteKey
	WHERE b.TransactionAliasKey <> b.MasterRecordKey
    AND a.CreatedDate > b.CreatedDate
	AND a.TransactionAliasKey IS NOT NULL 
	AND a.SiteSKey IS NOT NULL 
	AND a.IgnoreRecord = 0

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
    + 'Procedure Name: [staging].[usp_ResetEntityToSiteMappings]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
