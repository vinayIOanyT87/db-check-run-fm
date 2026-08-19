/*
	DROP PROCEDURE [Staging].[usp_SetTransactionHeaderLevel1References]

	EXEC [staging].[usp_SetTransactionHeaderLevel1References]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionHeaderLevel1References]
(
	@IgnoreDateMismatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_SetTransactionHeaderLevel1References]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Sets, in staging, the TransactionHeader fields that references Level1 tables.
	-- Notes:
	-- 1. This Stored Procedure is RecordVersioning-aware, i.e. it sets the references with the specific RecordVersion key, wherever the referenced dimension supports RecordVersioning.
	-- 2. @IgnoreDateMismatch: 
	--		0 : For historical tables, strictly use the StartDate-EndDate range of entity records for identifying the foreign keys.
	--			Raise an exception if relationships cannot be resolved because of date range mismatch.
	--		1 : For historical tables, use the StartDate-EndDate range of entity records for identifying the foreign keys where possible. 
	--			For those relationships that cannot be resolved because of a data range mismatch, force the relationships by ignoring the 
	--			date range mismatch.
	--    Start Date and End Date of the entity record are used, except when the @IgnoreDateMismatch field is set to 1 (True).
	-- 3. TransactionHeaders references to those entity types that support Record Versioning are not set right away after the entity 
	--    types are loaded (e.g. they are not set in usp_SetLevel1References), because TransactionHeaders do not come readily with 
	--    Site references. This Stored Procedure relies on the Site information to parse the entity-to-site assignment tree to resolve 
	--    the Record Versioning references. Therefore this Stored Procedure can only be executed after the TransactionHeader site 
	--    references have properly been resolved. 
	-- 4. The foreign keys are maintained in the OLAP database tables, not in the staging tables, but in order for the staging tables to 
	--    be properly loaded into the OLAP tables, the fields in the staging tables that reflect those OLAP table foreign keys have to be 
	--    preset correctly.
	-- 5. For references to historical tables, the foreign key is determined by a combination of the Identity Key (e.g. ProductKey) and the StartDate-EndDate range.
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
	-- 10. Dummy entity records, i.e.records that were artificially created in the entity tables to support transactions that had references to 
	--    entities that are not in the OLTP entity tables, do not have a supporting entity-to-site mapping, whether the entity type supports 
	--    Record Versioning or not, and as such references to those entities are resolved through direct lookup in the entity tables,
	--    without using the entity-to-site mappings tables, just as for an entity type that does not support Record Versioning. Dummy entity
	--    records might be inserted at any ETL run, not just on the intitial ETL.	
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		DECLARE @openEndedDate Datetimeoffset(7)		
		DECLARE @cdcActivationDate Datetimeoffset(7)

		SELECT @openEndedDate = DATEADD(year, 100, GETDATE())

		SET @cdcActivationDate = (SELECT TOP(1) CDCActivationDate FROM dbo.DimSystemInfo)
		IF (@cdcActivationDate IS NULL)
		BEGIN
			SET @cdcActivationDate = @openEndedDate
		END


		
		-- TransactionHeader AutoDistributionReasonCode references
		UPDATE a
		SET a.ReasonCodeSKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimAutoDistributionReasonCodes b
			ON b.AKey = a.ReasonCodeKey
		WHERE a.IgnoreRecord = 0
		AND a.ReasonCodeSKey IS NULL
				


		-- TransactionHeader Commpany references			
		UPDATE a 
		SET a.BillToCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.BillToCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.BillToCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.BillToCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.BillToCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)

		UPDATE a 
		SET a.BillToCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.BillToCompanyKey
		WHERE a.IgnoreRecord  = 0
		AND a.BillToCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.BillToCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.BillToCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.BillToCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.BillToCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1
			
		
		UPDATE a 
		SET a.CarrierCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.CarrierCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.CarrierCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.CarrierCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.CarrierCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)

		UPDATE a 
		SET a.CarrierCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.CarrierCompanyKey
		WHERE a.IgnoreRecord  = 0
		AND a.CarrierCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.CarrierCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.CarrierCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.CarrierCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.CarrierCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1

		
		UPDATE a 
		SET a.ManagerCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.ManagerCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.ManagerCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.ManagerCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.ManagerCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)

		UPDATE a 
		SET a.ManagerCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.ManagerCompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.ManagerCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.ManagerCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.ManagerCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.ManagerCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.ManagerCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1

			
		UPDATE a 
		SET a.OwnerCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.OwnerCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.OwnerCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.OwnerCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.OwnerCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)
		
		UPDATE a 
		SET a.OwnerCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.OwnerCompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.OwnerCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.OwnerCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)		

		UPDATE a 
		SET a.OwnerCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.OwnerCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.OwnerCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1
		

		UPDATE a 
		SET a.ShipperCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.ShipperCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.ShipperCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.ShipperCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.ShipperCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.ShipperCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.ShipperCompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.ShipperCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.ShipperCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.ShipperCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.ShipperCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.ShipperCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1
		

		UPDATE a 
		SET a.ShipToCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.ShipToCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.ShipToCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.ShipToCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.ShipToCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.ShipToCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.ShipToCompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.ShipToCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.ShipToCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.ShipToCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.ShipToCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.ShipToCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1
		

		UPDATE a 
		SET a.SupplierCompanySKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblCompanyToSiteRecordVersion c
			ON c.CompanyKey = b.SupplierCompanyKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.SupplierCompanySKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT CompanyKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblCompanyToSiteRecordVersion GROUP BY CompanyKey, SiteSKey) b
		ON b.CompanyKey = a.SupplierCompanyKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.SupplierCompanySKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.SupplierCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.SupplierCompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.SupplierCompanySKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite c
			WHERE c.CompanyKey = a.SupplierCompanyKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.SupplierCompanySKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
		ON b.AKey = a.SupplierCompanyKey
		INNER JOIN dbo.DimCompany c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.SupplierCompanySKey IS NULL
		AND c._IsRecordAddedByETL = 1

		


		-- TransactionHeader Equipment references		
		UPDATE a 
		SET a.DestinationEquipment1SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.DestinationEquipment1Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.DestinationEquipment1SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.DestinationEquipment1Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment1SKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.DestinationEquipment1SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.DestinationEquipment1Key
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment1SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.DestinationEquipment1Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.DestinationEquipment1SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.DestinationEquipment1Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.DestinationEquipment1SKey IS NULL
		AND c._IsRecordAddedByETL = 1

		
		UPDATE a 
		SET a.DestinationEquipment2SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.DestinationEquipment2Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.DestinationEquipment2SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.DestinationEquipment2Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment2SKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.DestinationEquipment2SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.DestinationEquipment2Key
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment2SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.DestinationEquipment2Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.DestinationEquipment2SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.DestinationEquipment2Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.DestinationEquipment2SKey IS NULL
		AND c._IsRecordAddedByETL = 1
	
		
		UPDATE a 
		SET a.DestinationEquipment3SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.DestinationEquipment3Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.DestinationEquipment3SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.DestinationEquipment3Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment3SKey IS NULL
			AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.DestinationEquipment3SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.DestinationEquipment3Key
		WHERE a.IgnoreRecord = 0
		AND a.DestinationEquipment3SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.DestinationEquipment3Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.DestinationEquipment3SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.DestinationEquipment3Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.DestinationEquipment3SKey IS NULL
		AND c._IsRecordAddedByETL = 1
		
			
		UPDATE a 
		SET a.SourceEquipment1SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.SourceEquipment1Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.SourceEquipment1SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.SourceEquipment1Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment1SKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.SourceEquipment1SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.SourceEquipment1Key
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment1SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.SourceEquipment1Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.SourceEquipment1SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.SourceEquipment1Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.SourceEquipment1SKey IS NULL
		AND c._IsRecordAddedByETL = 1
	

		UPDATE a 
		SET a.SourceEquipment2SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.SourceEquipment2Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0
		

		UPDATE a 
		SET a.SourceEquipment2SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.SourceEquipment2Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment2SKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.SourceEquipment2SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.SourceEquipment2Key
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment2SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.SourceEquipment2Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.SourceEquipment2SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.SourceEquipment2Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.SourceEquipment2SKey IS NULL
		AND c._IsRecordAddedByETL = 1

		
		UPDATE a 
		SET a.SourceEquipment3SKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblEquipmentToSiteRecordVersion c
			ON c.EquipmentKey = b.SourceEquipment3Key
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0
		
		UPDATE a 
		SET a.SourceEquipment3SKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT EquipmentKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblEquipmentToSiteRecordVersion GROUP BY EquipmentKey, SiteSKey) b
		ON b.EquipmentKey = a.SourceEquipment3Key
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment3SKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)
		
		UPDATE a 
		SET a.SourceEquipment3SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimEquipment b
		ON b.AKey = a.SourceEquipment3Key
		WHERE a.IgnoreRecord = 0
		AND a.SourceEquipment3SKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite c
			WHERE c.EquipmentKey = a.SourceEquipment3Key
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.SourceEquipment3SKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimEquipment GROUP BY AKey) b
		ON b.AKey = a.SourceEquipment3Key
		INNER JOIN dbo.DimEquipment c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.SourceEquipment3SKey IS NULL
		AND c._IsRecordAddedByETL = 1



		-- TransactionHeader Personnel references		
		UPDATE a 
		SET a.OperatorPersonnelSKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblPersonnelToSiteRecordVersion c
			ON c.PersonnelKey = b.OperatorPersonnelKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.OperatorPersonnelSKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT PersonnelKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblPersonnelToSiteRecordVersion GROUP BY PersonnelKey, SiteSKey) b
		ON b.PersonnelKey = a.OperatorPersonnelKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.OperatorPersonnelSKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.OperatorPersonnelSKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimPersonnel b
		ON b.AKey = a.OperatorPersonnelKey
		WHERE a.IgnoreRecord = 0
		AND a.OperatorPersonnelSKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityPersonnelToSite c
			WHERE c.PersonnelKey = a.OperatorPersonnelKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.OperatorPersonnelSKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimPersonnel GROUP BY AKey) b
		ON b.AKey = a.OperatorPersonnelKey
		INNER JOIN dbo.DimPersonnel c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.OperatorPersonnelSKey IS NULL
		AND c._IsRecordAddedByETL = 1



		-- TransactionHeader TransactionAlias references		
		UPDATE a 
		SET a.TransactionAliasSKey = x.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN 
		(
			SELECT b.TransactionKey, MAX(c.RecordVersionSKey) RecordVersionSKey 
			FROM staging.tblTransactions b
			INNER JOIN map.tblTransactionAliasToSiteRecordVersion c
			ON c.TransactionAliasKey = b.TransactionAliasKey
			AND c.SiteSKey = b.SiteSKey
			WHERE b.IgnoreRecord = 0
			AND b.InventoryDate BETWEEN CONVERT(DATE, c.StartDate) AND ISNULL(c.EndDate, @openEndedDate)
			GROUP BY b.TransactionKey
		) x
		ON x.TransactionKey = a.TransactionKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.TransactionAliasSKey = b.RecordVersionSKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT TransactionAliasKey, SiteSKey, MAX(RecordVersionSKey) RecordVersionSKey FROM map.tblTransactionAliasToSiteRecordVersion GROUP BY TransactionAliasKey, SiteSKey) b
		ON b.TransactionAliasKey = a.TransactionAliasKey
		AND b.SiteSKey = a.SiteSKey
		WHERE a.IgnoreRecord = 0
		AND a.TransactionAliasSKey IS NULL
		AND 
		(
			(@IgnoreDateMismatch = 1)
			OR
			a.InventoryDate < @cdcActivationDate
		)	

		UPDATE a 
		SET a.TransactionAliasSKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN dbo.DimTransactionAlias b
		ON b.AKey = a.TransactionAliasKey
		WHERE a.IgnoreRecord = 0
		AND a.TransactionAliasSKey IS NULL
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityTransactionAliasToSite c
			WHERE c.TransactionAliasKey = a.TransactionAliasKey
			AND c.SiteSKey = a.SiteSKey
		)
		AND 
		(
			(@IgnoreDateMismatch = 1)
		)	

		UPDATE a 
		SET a.TransactionAliasSKey = b.SKey
		FROM staging.tblTransactions a
		INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimTransactionAlias GROUP BY AKey) b
		ON b.aKey = a.TransactionAliasKey
		INNER JOIN dbo.DimTransactionAlias c
		ON c.SKey = b.SKey
		WHERE a.IgnoreRecord  = 0
		AND a.TransactionAliasSKey IS NULL
		--AND c._IsRecordAddedByETL = 1



		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactions WHERE ReasonCodeKey IS NOT NULL AND ReasonCodeSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR ('Failure to resolve TransactionHeader-to-ReasonCode references', 16, 1);
			RETURN;
		END
		
		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactions WHERE BillToCompanyKey IS NOT NULL AND BillToCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE CarrierCompanyKey IS NOT NULL AND CarrierCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE ManagerCompanyKey IS NOT NULL AND ManagerCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE OwnerCompanyKey IS NOT NULL AND OwnerCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE ShipperCompanyKey IS NOT NULL AND ShipperCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE ShipToCompanyKey IS NOT NULL AND ShipToCompanySKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE SupplierCompanyKey IS NOT NULL AND SupplierCompanySKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionHeader-to-Company references',16,1); 
			RETURN;
		END		

		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactions WHERE DestinationEquipment1Key IS NOT NULL AND DestinationEquipment1SKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE DestinationEquipment2Key IS NOT NULL AND DestinationEquipment2SKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE DestinationEquipment3Key IS NOT NULL AND DestinationEquipment3SKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE SourceEquipment1Key IS NOT NULL AND SourceEquipment1SKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE SourceEquipment2Key IS NOT NULL AND SourceEquipment2SKey IS NULL AND IgnoreRecord = 0) > 0
			OR (SELECT COUNT(*) FROM staging.tblTransactions WHERE SourceEquipment3Key IS NOT NULL AND SourceEquipment3SKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionHeader-to-Equipment references',16,1); 
			RETURN;
		END

		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactions WHERE OperatorPersonnelKey IS NOT NULL AND OperatorPersonnelSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve TransactionHeader-to-Personnel references',16,1); 
			RETURN;
		END
		
		IF 
		(
			(SELECT COUNT(*) FROM staging.tblTransactions WHERE TransactionAliasKey IS NOT NULL AND TransactionAliasSKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR ('Failure to resolve TransactionHeader-to-TransactionAlias references', 16, 1);
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
						+ 'Procedure Name: [staging].[usp_SetTransactionHeaderLevel1References]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END