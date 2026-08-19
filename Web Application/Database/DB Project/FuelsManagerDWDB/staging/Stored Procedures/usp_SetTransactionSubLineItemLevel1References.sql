/*
	DROP PROCEDURE [Staging].[usp_SetTransactionSubLineItemLevel1References]

	EXEC [staging].[usp_SetTransactionSubLineItemLevel1References]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionSubLineItemLevel1References]
(
	@IgnoreDateMismatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_SetTransactionSubLineItemLevel1References]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Sets, in staging, the TransactionSubLineItem fields that references Level1 tables.
	-- Notes:
	-- 1. This Stored Procedure is RecordVersioning-aware, i.e. it sets the references with the specific RecordVersion key, wherever the referenced dimension supports RecordVersioning.
	-- 2. @IgnoreDateMismatch: 
	--		0 : For historical tables, strictly use the StartDate-EndDate range of entity records for identifying the foreign keys.
	--			Raise an exception if relationships cannot be resolved because of date range mismatch.
	--		1 : For historical tables, use the StartDate-EndDate range of entity records for identifying the foreign keys where possible. 
	--			For those relationships that cannot be resolved because of a data range mismatch, force the relationships by ignoring the 
	--			date range mismatch.
	--    Start Date and End Date of the entity record are used, except when the @IgnoreDateMismatch field is set to 1 (True).
	-- 3. TransactionSubLineItems references to those entity types that support Record Versioning are not set right away after the entity 
	--    types are loaded (e.g. they are not set in usp_SetLevel1References), because TransactionSubLineItems do not come readily with 
	--    Site references. This Stored Procedure relies on the Site information to parse the entity-to-site assignment tree to resolve 
	--    the Record Versioning references. Therefore this Stored Procedure can only be executed after the TransactionSubLineItem site 
	--    references have properly been resolved. 
	-- 4. The foreign keys are maintained in the OLAP database tables, not in the staging tables, but in order for the staging tables to 
	--    be properly loaded into the OLAP tables, the fields in the staging tables that reflect those OLAP table foreign keys have to be 
	--    preset correctly.
	-- 5. For references to historical tables, the foreign key is determined by a combination of the Identity Key (e.g. ProductKey, i.e. 
	--    ProductKey or ProductIndex) and the StartDate-EndDate range.
	-- 6. For references to non-historical tables, the foreign key is determined solely on the Identity Key.
	-- 7. With the help of the FuelsManager Change Data Capture (FMCDC) system, all record changes on the OLTP system are properly captured 
	--    and time-stamped separately.
	--    However, on a system where the FMCDC has not yet been deployed, only the latest version of each record is available, and the only
	--    time-stamp available is the UpdatedDate, which only reflects the time of the last record change. 
	--    If an entity record (e.g Company) is referenced by another entity record (e.g FuelCard), then it is very well possible for the 
	--    referenced entity record (Company) to be have been modified after it was linked to the dependant record (FueldCard), but before the
	--    initial ETL execution.
	--    In this case the UpdatedDate of the referenced record (Company) will be greater than that of the dependant record (FuelCard). 
	--    In the absence of the FMCDC capturing all versions of the record changes, trying to identity the exact version of the referenced 
	--    record by date will not be possible, hence the need to ignore date range mismatch when identifying foreign keys on a system 
	--    initially, before the FMCDC has had a chance to capture intermediate record changes.	
	-- 8. The problem of the date mismatch is even more relevant for Transactions whose entity references are resolved using the 
	--    Inventory Date instead of the Transaction timestamp. Since the a Transaction can be added/updated with a past Inventory Date, 
	--    this issue, in the case of transactions, can happen on subsequent loads as well as on initial load. To address this issue, 
	--    the decision of whether of not to ignore date range mismatch when identifying foreign keys on Transactions relies not only 
	--    on the condition of an initial load, but also on whether the Inventory Date preceeds the first time the ETL process was executed.
	-- 9. For entity types that support Record Versioning, if a transaction has a reference to an entity that is no longer mapped to the
	--    transaction site, then it will not be possible to find the right Record Version for the entity. In this case, the Master Record is
	--    simply used for the entity reference. This senario is only applicable to the intial ETL run, before the implementation of the 
	--    FMCDC tracking.
	-- 10. Dummy entity records that were artificially created in the entity tables to support transactions that had references to entities
	--    that are not in the OLTP entity tables, do not have a supporting entity-to-site mapping, whether the entity type supports 
	--    Record Versioning or not, and as such references to those entities are resolved through direct lookup in the entity tables,
	--    without using the entity-to-site mappings tables, just as for an entity type that does not support Record Versioning. Dummy entity
	--    records might be inserted at any ETL run, not just on the intitial ETL.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		DECLARE @openEndedDate Datetimeoffset(7)		
		DECLARE @cdcActivationDate Datetimeoffset(7)
		DECLARE @dummyAKeyPrefix varchar(30)

		SELECT @openEndedDate = DATEADD(year, 100, GETDATE())

		SET @cdcActivationDate = (SELECT TOP(1) CDCActivationDate FROM dbo.DimSystemInfo)
		IF (@cdcActivationDate IS NULL)
		BEGIN
			SET @cdcActivationDate = @openEndedDate
		END
	
		SELECT @dummyAKeyPrefix = 'DummyAKey_'


		-- TransactionSubLineItem Product references			
		UPDATE a 
		SET a.ProductSKey = x.RecordVersionSKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN 
		(
			SELECT b.TransactionSubLineItemKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactionSubLineItems b
			INNER JOIN map.tblProductToSiteRecordVersion c
			ON c.ProductKey = b.ProductKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.TransactionInventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionSubLineItemKey
		) x
		ON x.TransactionSubLineItemKey = a.TransactionSubLineItemKey
		WHERE a.IgnoreRecord = 0	

		UPDATE a 
		SET a.ProductSKey = b.RecordVersionSKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN (SELECT ProductKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblProductToSiteRecordVersion GROUP BY ProductKey, SiteSKey) b
		ON b.ProductKey = a.ProductKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.ProductSKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.TransactionInventoryDate < @cdcActivationDate
		)	
	
		UPDATE a 
		SET a.ProductSKey = b.SKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN dbo.DimProduct b
		ON b.AKey = a.ProductKey
		WHERE a.IgnoreRecord = 0
		AND a.ProductSKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityProductToSite c
			WHERE c.ProductKey = a.ProductKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	


		UPDATE a 
		SET a.ProductSKey = b.SKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimProduct GROUP BY aKey) b
		ON b.AKey = a.ProductKey
		INNER JOIN dbo.DimProduct c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.ProductSKey IS NULL
		AND c._IsRecordAddedByETL = 1


		-- TransactionSubLineItem LoadArm references			
		UPDATE a
		SET a.LoadArmSKey = b.SKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN dbo.DimLoadArm b
		ON b.AKey = a.LoadArmKey
		WHERE a.IgnoreRecord = 0
		AND a.LoadArmSKey IS NULL


		-- TransactionSubLineItem Tasnk references			
		UPDATE a
		SET a.StorageLocationTankSKey = b.SKey
		FROM staging.tblTransactionSubLineItems a
		INNER JOIN dbo.DimTank b
		ON b.AKey = a.StorageLocationTankKey
		WHERE a.IgnoreRecord = 0
		AND a.StorageLocationTankSKey IS NULL


		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactionSubLineItems WHERE ProductKey IS NOT NULL AND ProductSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionSubLineItem-to-Product references',16,1); 
			RETURN;
		END

		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactionSubLineItems WHERE LoadArmKey IS NOT NULL AND LoadArmSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionSubLineItem-to-LoadArm references',16,1); 
			RETURN;
		END

		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactionSubLineItems WHERE StorageLocationTankKey IS NOT NULL AND StorageLocationTankSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionSubLineItem-to-StorageTank references',16,1); 
			RETURN;
		END
		
					
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [staging].[usp_SetTransactionSubLineItemLevel1References]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END