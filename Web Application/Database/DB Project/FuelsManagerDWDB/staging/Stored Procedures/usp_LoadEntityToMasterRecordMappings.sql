/*
	DROP PROCEDURE [staging].[usp_LoadEntityToMasterRecordMappings]

	EXEC [staging].[usp_LoadEntityToMasterRecordMappings]
	
*/
CREATE PROCEDURE [staging].[usp_LoadEntityToMasterRecordMappings]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_LoadEntityToMasterRecordMappings]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Loads records from staging into EntityToMasterRecord Mappings tables in the OLAP database.
	-- Notes:
	-- 1. EntityToMasterRecord mapping tables are used for Record Versioning. They provide a time-based mapping of MasterRecord, Site, and RecordVersion key.
	-- 2. The EntityToMasterRecord mapping tables are only concerned with Record Versioning Key references, and not with the exact SKey historical entity reference.
	-- 3. The Level 0 references have to be first sorted out before the EntityToMasterRecord tables can be safely loaded from staging into the OLAP database.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY		
			
		--CompanyToMasterRecord
		-- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
		INSERT INTO map.tblCompanyToMasterRecord
		([CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[CompanyKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblCompanies a
		WHERE a.EndDate IS NOT NULL
		AND a.IgnoreRecord = 0
		ORDER BY a.CompanyKey, COALESCE(a.RecordUpdatedDate, a.UpdatedDate)
		
		-- EndDate records for which a later revision has been recorded
		-- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
		-- In map.tblCompanyToMasterRecord, the SiteSKey corresponds to the Owner site
		INSERT INTO map.tblCompanyToMasterRecord
		([CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT [CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate]
		FROM 
		(
			MERGE map.tblCompanyToMasterRecord AS tgt
			USING  
			(
				SELECT [CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate], [IsRecordDeleted]
				FROM staging.tblCompanies a	
				WHERE a.EndDate IS NULL AND a.CompanyKey IS NOT NULL AND a.IgnoreRecord = 0
			) AS src
			ON tgt.CompanyKey = src.CompanyKey
			WHEN NOT MATCHED AND ISNULL(IsRecordDeleted, 0) = 0 THEN
				INSERT ([CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
				VALUES (src.[CompanyKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], NULL)
			WHEN MATCHED AND tgt.EndDate IS NULL AND src.StartDate > tgt.StartDate THEN
				UPDATE SET tgt.EndDate = src.StartDate
			OUTPUT $Action Action_Out, src.[CompanyKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
		) AS Merge_out
		WHERE Merge_Out.Action_Out = 'UPDATE' AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;		

		-- Cover the case where an equipment record is being captured for the first time in the OLAP database, but has still been modified at least one time before the ETL. The MERGE operation above only covers insertions of open-enddated records where an open-enddated record already exists in the mapping table.
		INSERT INTO map.tblCompanyToMasterRecord
		([CompanyKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[CompanyKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblCompanies a
		WHERE a.EndDate IS NULL
		AND a.IgnoreRecord = 0
		AND ISNULL(a.IsRecordDeleted, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblCompanyToMasterRecord b
			WHERE b.CompanyKey = a.CompanyKey
			AND b.EndDate IS NULL
		)
			
		--EquipmentToMasterRecord
		-- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
		INSERT INTO map.tblEquipmentToMasterRecord
		([EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[EquipmentKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblEquipment a
		WHERE a.EndDate IS NOT NULL
		AND a.IgnoreRecord = 0
		ORDER BY a.EquipmentKey, COALESCE(a.RecordUpdatedDate, a.UpdatedDate)
		
		-- EndDate records for which a later revision has been recorded
		-- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
		-- In map.tblEquipmentToMasterRecord, the SiteSKey corresponds to the Owner site
		INSERT INTO map.tblEquipmentToMasterRecord
		([EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT [EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate]
		FROM 
		(
			MERGE map.tblEquipmentToMasterRecord AS tgt
			USING  
			(
				SELECT [EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate], [IsRecordDeleted]
				FROM staging.tblEquipment a	
				WHERE a.EndDate IS NULL AND a.EquipmentKey IS NOT NULL AND a.IgnoreRecord = 0
			) AS src
			ON tgt.EquipmentKey = src.EquipmentKey
			WHEN NOT MATCHED AND ISNULL(IsRecordDeleted, 0) = 0 THEN
				INSERT ([EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
				VALUES (src.[EquipmentKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], NULL)
			WHEN MATCHED AND tgt.EndDate IS NULL AND src.StartDate > tgt.StartDate THEN
				UPDATE SET tgt.EndDate = src.StartDate
			OUTPUT $Action Action_Out, src.[EquipmentKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
		) AS Merge_out
		WHERE Merge_Out.Action_Out = 'UPDATE' AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;		

		-- Cover the case where an equipment record is being captured for the first time in the OLAP database, but has still been modified at least one time before the ETL. The MERGE operation above only covers insertions of open-enddated records where an open-enddated record already exists in the mapping table.
		INSERT INTO map.tblEquipmentToMasterRecord
		([EquipmentKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[EquipmentKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblEquipment a
		WHERE a.EndDate IS NULL
		AND a.IgnoreRecord = 0
		AND ISNULL(a.IsRecordDeleted, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEquipmentToMasterRecord b
			WHERE b.EquipmentKey = a.EquipmentKey
			AND b.EndDate IS NULL
		)


		--ProductToMasterRecord
		-- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
		INSERT INTO map.tblProductToMasterRecord
		([ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[ProductKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblProducts a
		WHERE a.EndDate IS NOT NULL
		AND a.IgnoreRecord = 0
		ORDER BY a.ProductKey, COALESCE(a.RecordUpdatedDate, a.UpdatedDate)
		
		-- EndDate records for which a later revision has been recorded
		-- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
		-- In map.tblProductToMasterRecord, the SiteSKey corresponds to the Owner site
		INSERT INTO map.tblProductToMasterRecord
		([ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT [ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate]
		FROM 
		(
			MERGE map.tblProductToMasterRecord AS tgt
			USING  
			(
				SELECT [ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate], [IsRecordDeleted]
				FROM staging.tblProducts a	
				WHERE a.EndDate IS NULL AND a.ProductKey IS NOT NULL AND a.IgnoreRecord = 0
			) AS src
			ON tgt.ProductKey = src.ProductKey
			WHEN NOT MATCHED AND ISNULL(IsRecordDeleted, 0) = 0 THEN
				INSERT ([ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
				VALUES (src.[ProductKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], NULL)
			WHEN MATCHED AND tgt.EndDate IS NULL AND src.StartDate > tgt.StartDate THEN
				UPDATE SET tgt.EndDate = src.StartDate
			OUTPUT $Action Action_Out, src.[ProductKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
		) AS Merge_out
		WHERE Merge_Out.Action_Out = 'UPDATE' AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;
		
		-- Cover the case where a product record is being captured for the first time in the OLAP database, but has still been modified at least one time before the ETL. The MERGE operation above only covers insertions of open-enddated records where an open-enddated record already exists in the mapping table.
		INSERT INTO map.tblProductToMasterRecord
		([ProductKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[ProductKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblProducts a
		WHERE a.EndDate IS NULL
		AND a.IgnoreRecord = 0
		AND ISNULL(a.IsRecordDeleted, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblProductToMasterRecord b
			WHERE b.ProductKey = a.ProductKey
			AND b.EndDate IS NULL
		)
				
				
		--PersonnelToMasterRecord
		-- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
		INSERT INTO map.tblPersonnelToMasterRecord
		([PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[PersonnelKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblPersonnel a
		WHERE a.EndDate IS NOT NULL
		AND a.IgnoreRecord = 0
		ORDER BY a.PersonnelKey, COALESCE(a.RecordUpdatedDate, a.UpdatedDate)
		
		-- EndDate records for which a later revision has been recorded
		-- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
		-- In map.tblPersonnelToMasterRecord, the SiteSKey corresponds to the Owner site
		INSERT INTO map.tblPersonnelToMasterRecord
		([PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT [PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate]
		FROM 
		(
			MERGE map.tblPersonnelToMasterRecord AS tgt
			USING  
			(
				SELECT [PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate], [IsRecordDeleted]
				FROM staging.tblPersonnel a	
				WHERE a.EndDate IS NULL AND a.PersonnelKey IS NOT NULL AND a.IgnoreRecord = 0
			) AS src
			ON tgt.PersonnelKey = src.PersonnelKey
			WHEN NOT MATCHED AND ISNULL(IsRecordDeleted, 0) = 0 THEN
				INSERT ([PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
				VALUES (src.[PersonnelKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], NULL)
			WHEN MATCHED AND tgt.EndDate IS NULL AND src.StartDate > tgt.StartDate THEN
				UPDATE SET tgt.EndDate = src.StartDate
			OUTPUT $Action Action_Out, src.[PersonnelKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
		) AS Merge_out
		WHERE Merge_Out.Action_Out = 'UPDATE' AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;
		
		-- Cover the case where a Personnel record is being captured for the first time in the OLAP database, but has still been modified at least one time before the ETL. The MERGE operation above only covers insertions of open-enddated records where an open-enddated record already exists in the mapping table.
		INSERT INTO map.tblPersonnelToMasterRecord
		([PersonnelKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[PersonnelKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblPersonnel a
		WHERE a.EndDate IS NULL
		AND a.IgnoreRecord = 0
		AND ISNULL(a.IsRecordDeleted, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblPersonnelToMasterRecord b
			WHERE b.PersonnelKey = a.PersonnelKey
			AND b.EndDate IS NULL
		)


		--TransactionAliasToMasterRecord
		-- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
		INSERT INTO map.tblTransactionAliasToMasterRecord
		([TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[TransactionAliasKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblTransactionAliases a
		WHERE a.EndDate IS NOT NULL
		AND a.IgnoreRecord = 0
		ORDER BY a.TransactionAliasKey, COALESCE(a.RecordUpdatedDate, a.UpdatedDate)
		
		-- EndDate records for which a later revision has been recorded
		-- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
		-- In map.tblTransactionAliasToMasterRecord, the SiteSKey corresponds to the Owner site
		INSERT INTO map.tblTransactionAliasToMasterRecord
		([TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT [TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate]
		FROM 
		(
			MERGE map.tblTransactionAliasToMasterRecord AS tgt
			USING  
			(
				SELECT [TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate], [IsRecordDeleted]
				FROM staging.tblTransactionAliases a	
				WHERE a.EndDate IS NULL AND a.TransactionAliasKey IS NOT NULL AND a.IgnoreRecord = 0
			) AS src
			ON tgt.TransactionAliasKey = src.TransactionAliasKey
			WHEN NOT MATCHED AND ISNULL(IsRecordDeleted, 0) = 0 THEN
				INSERT ([TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
				VALUES (src.[TransactionAliasKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], NULL)
			WHEN MATCHED AND tgt.EndDate IS NULL AND src.StartDate > tgt.StartDate THEN
				UPDATE SET tgt.EndDate = src.StartDate
			OUTPUT $Action Action_Out, src.[TransactionAliasKey], src.[MasterRecordKey], src.[SiteSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
		) AS Merge_out
		WHERE Merge_Out.Action_Out = 'UPDATE' AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;
		
		-- Cover the case where a TransactionAlias record is being captured for the first time in the OLAP database, but has still been modified at least one time before the ETL. The MERGE operation above only covers insertions of open-enddated records where an open-enddated record already exists in the mapping table.
		INSERT INTO map.tblTransactionAliasToMasterRecord
		([TransactionAliasKey], [MasterRecordKey], [SiteSKey], [StartDate], [EndDate])
		SELECT a.[TransactionAliasKey], a.[MasterRecordKey], a.[SiteSKey], a.[StartDate], a.[EndDate]
		FROM staging.tblTransactionAliases a
		WHERE a.EndDate IS NULL
		AND a.IgnoreRecord = 0
		AND ISNULL(a.IsRecordDeleted, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblTransactionAliasToMasterRecord b
			WHERE b.TransactionAliasKey = a.TransactionAliasKey
			AND b.EndDate IS NULL
		)
					
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
						+ 'Procedure Name: [staging].[usp_LoadEntityToMasterRecordMappings]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END