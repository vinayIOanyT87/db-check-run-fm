/*
	DROP PROCEDURE [Staging].[usp_SetDimensionLevel1References]

	EXEC [staging].[usp_SetDimensionLevel1References]
	
*/
CREATE PROCEDURE [staging].[usp_SetDimensionLevel1References]
(
	@IgnoreDateMismatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_SetDimensionLevel1References]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Sets, in staging, the Dimension fields that reflect foreign key references to all Level 1 tables.
	-- Notes:
	-- 1. This Stored Procedure is RecordVersioning-aware, i.e. it sets the references with the specific RecordVersion key, wherever the referenced dimension supports RecordVersioning.
	-- 2. @IgnoreDateMismatch: 
	--		0 : For historical tables, strictly use the StartDate-EndDate range of entity records for identifying the foreign keys.
	--			Raise an exception if relationships cannot be resolved because of date range mismatch.
	--		1 : For historical tables, use the StartDate-EndDate range of entity records for identifying the foreign keys where possible. 
	--			For those relationships that cannot be resolved because of a data range mismatch, force the relationships by ignoring the 
	--			date range mismatch.
	--    Start Date and End Date of the entity record are used, except when the @IgnoreDateMismatch field is set to 1 (True).
	-- 3. The foreign keys are maintained in the OLAP database tables, not in the staging tables, but in order for the staging tables to 
	--    be properly loaded into the OLAP tables, the fields in the staging tables that reflect those OLAP table foreign keys have to be 
	--    preset correctly.
	-- 4. For references to historical tables, the foreign key is determined by a combination of the Identity Key (e.g. ProductKey, i.e. 
	--    ProductGuid or ProductIndex) and the StartDate-EndDate range.
	-- 5. For references to non-historical tables, the foreign key is determined solely on the Identity Key.
	-- 6. With the help of the FuelsManager Change Data Capture (FMCDC) system, all record changes on the OLTP system are properly captured 
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
	-- 7. The problem of the date mismatch is even more relevant for Transactions whose entity references are resolved using the 
	--    Inventory Date instead of the Transaction timestamp. Since the a Transaction can be added/updated with a past Inventory Date, 
	--    this issue, in the case of transactions, can happen on subsequent loads as well as on initial load. To address this issue, 
	--    the decision of whether of not to ignore date range mismatch when identifying foreign keys on Transactions relies not only 
	--    on the condition of an initial load, but also on whether the Inventory Date preceeds the first time the ETL process was executed.
	-- 8. For entity types that support Record Versioning, if an entity has a reference to an entity that is no longer mapped to the
	--    entity site, then it will not be possible to find the right Record Version for the entity. In this case, the Master Record is
	--    simply used for the entity reference. This senario is only applicable to the intial ETL run, before the implementation of the 
	--    FMCDC tracking.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		DECLARE @openEndedDate Datetimeoffset(7)		

		SELECT @openEndedDate = DATEADD(year, 100, GETDATE())

	
		-- Level 1 Self-references
		EXEC [staging].[usp_UpdateLevel1TablesSelfReferences] @IgnoreDateMismatch
		
		
		-- CompanyToUserGroup references
		-- CompanyToUserGroup is an external attribute of Company. The mappings are maintained separately for each Company record version, and therefore their Company references can be resolved through a direct mapping to DimCompany, without requiring the help of the Company-To-Site assignment hierarchy.
		UPDATE a 
		SET a.CompanySKey = b.SKey
		FROM staging.tblCompanyToUserGroup a
		INNER JOIN dbo.DimCompany b
		ON b.AKey = a.CompanyKey
		WHERE a.IgnoreRecord = 0
		AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
		
		IF (@IgnoreDateMismatch = 1)
		BEGIN
			UPDATE a 
			SET a.CompanySKey = b.SKey
			FROM staging.tblCompanyToUserGroup a
			INNER JOIN (SELECT AKey, MAX(SKey) SKey FROM dbo.DimCompany GROUP BY AKey) b
			ON b.AKey = a.CompanyKey
			WHERE a.IgnoreRecord = 0
			AND a.CompanySKey IS NULL
		END													
		
		IF 
		(
			(SELECT COUNT(*) FROM staging.tblCompanyToUserGroup WHERE CompanyKey IS NOT NULL AND CompanySKey IS NULL AND IgnoreRecord = 0) > 0
		)
		BEGIN
			RAISERROR('Failure to resolve Company-to-UserGroup references',16,1); 
			RETURN;
		END


		-- LoadArm references
		UPDATE a 
		SET a.BayAStationSKey = b.SKey
		FROM staging.tblLoadArms a
		INNER JOIN dbo.DimStation b
		ON b.AKey = a.BayAStationKey
		WHERE a.IgnoreRecord = 0

		UPDATE a 
		SET a.BayBStationSKey = b.SKey
		FROM staging.tblLoadArms a
		INNER JOIN dbo.DimStation b
		ON b.AKey = a.BayBStationKey
		WHERE a.IgnoreRecord = 0													
		
		IF 
		(
			((SELECT COUNT(*) FROM staging.tblLoadArms WHERE BayAStationKey IS NOT NULL AND BayAStationSKey IS NULL AND IgnoreRecord = 0) > 0)
			OR
			((SELECT COUNT(*) FROM staging.tblLoadArms WHERE BayBStationKey IS NOT NULL AND BayBStationSKey IS NULL AND IgnoreRecord = 0) > 0)
		)
		BEGIN
			RAISERROR('Failure to resolve LoadArm-to-Station references',16,1); 
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
						+ 'Procedure Name: [staging].[usp_SetDimensionLevel1References]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO
