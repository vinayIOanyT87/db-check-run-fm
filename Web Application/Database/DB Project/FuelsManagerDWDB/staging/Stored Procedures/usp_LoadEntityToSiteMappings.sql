/*
	DROP PROCEDURE [Staging].[usp_LoadEntityToSiteMappings]

	EXEC [staging].[usp_LoadEntityToSiteMappings]
	
*/
CREATE PROCEDURE [staging].[usp_LoadEntityToSiteMappings]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_LoadEntityToSiteMappings]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Loads EntityToSite mapping records from staging into the EntityToSite tables in the DW database.
	-- Notes:
	-- 1. No historical data (StartDate/EndDate) is maintained for the EntityToSite mappings, but since on the OLTP side, those mappings are either 
	--    created or deleted, and not updated, it means that by just capturing the creation and deletion, the procedure is covering all the 
	--    historical data capture needs of the mappings.
	-- 2. Updates for which the IsRecordDeleted flag is not set are ignored. Effectively, the only inputs processed for each mapping is 
	--    the new record entry, and the record deletion.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY		
		
		--EntityCompanyToSite
		-- No historical data maintained for map.tblEntityCompanyToSite. Simply flag the existing record as deleted if found, otherwise insert a new one.
		MERGE map.tblEntityCompanyToSite AS tgt
		USING  (SELECT [CompanyToSiteKey], [CompanyKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsRecordDeleted], [RecordUpdatedDate], [CombinedUpdatedDate] 
				FROM staging.tblEntityCompanyToSite WHERE CompanyKey IS NOT NULL AND SiteSKey IS NOT NULL AND IgnoreRecord = 0) AS src
		ON tgt.CompanyKey = src.CompanyKey AND tgt.SiteSKey = src.SiteSKey
		WHEN NOT MATCHED THEN
			INSERT ([CompanyToSiteKey], [CompanyKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [EndedBy], [EndedDate])
			VALUES (src.[CompanyToSiteKey], src.[CompanyKey], src.[SiteSKey], src.[AssignedFromSiteSKey], src.[CreatedBy], src.[CreatedDate],
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[UpdatedBy] ELSE NULL END,
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[CombinedUpdatedDate] ELSE NULL END)
		WHEN MATCHED AND (tgt.EndedDate IS NULL) AND (ISNULL(src.IsRecordDeleted, 0) = 1) THEN
			UPDATE SET tgt.[EndedBy] = src.[UpdatedBy], 
			tgt.[EndedDate] = src.[CombinedUpdatedDate];


		--EntityEquipmentToSite
		-- No historical data maintained for map.tblEntityEquipmentToSite. Simply flag the existing record as deleted if found, otherwise insert a new one.
		MERGE map.tblEntityEquipmentToSite AS tgt
		USING  (SELECT [EquipmentToSiteKey], [EquipmentKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsRecordDeleted], [RecordUpdatedDate], [CombinedUpdatedDate] 
				FROM staging.tblEntityEquipmentToSite WHERE EquipmentKey IS NOT NULL AND SiteSKey IS NOT NULL AND IgnoreRecord = 0) AS src
		ON tgt.EquipmentKey = src.EquipmentKey AND tgt.SiteSKey = src.SiteSKey
		WHEN NOT MATCHED THEN
			INSERT ([EquipmentToSiteKey], [EquipmentKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [EndedBy], [EndedDate])
			VALUES (src.[EquipmentToSiteKey], src.[EquipmentKey], src.[SiteSKey], src.[AssignedFromSiteSKey], src.[CreatedBy], src.[CreatedDate],
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[UpdatedBy] ELSE NULL END,
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[CombinedUpdatedDate] ELSE NULL END)
		WHEN MATCHED AND (tgt.EndedDate IS NULL) AND (src.IsRecordDeleted = 1) THEN
			UPDATE SET tgt.[EndedBy] = src.[UpdatedBy], 
			tgt.[EndedDate] = src.[CombinedUpdatedDate];


		--EntityPersonnelToSite
		-- No historical data maintained for map.tblEntityPersonnelToSite. Simply flag the existing record as deleted if found, otherwise insert a new one.
		MERGE map.tblEntityPersonnelToSite AS tgt
		USING  (SELECT [PersonnelToSiteKey], [PersonnelKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsRecordDeleted], [RecordUpdatedDate], [CombinedUpdatedDate] 
				FROM staging.tblEntityPersonnelToSite WHERE PersonnelKey IS NOT NULL AND SiteSKey IS NOT NULL AND IgnoreRecord = 0) AS src
		ON tgt.PersonnelKey = src.PersonnelKey AND tgt.SiteSKey = src.SiteSKey
		WHEN NOT MATCHED THEN
			INSERT ([PersonnelToSiteKey], [PersonnelKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [EndedBy], [EndedDate])
			VALUES (src.[PersonnelToSiteKey], src.[PersonnelKey], src.[SiteSKey], src.[AssignedFromSiteSKey], src.[CreatedBy], src.[CreatedDate],
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[UpdatedBy] ELSE NULL END,
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[CombinedUpdatedDate] ELSE NULL END)
		WHEN MATCHED AND (tgt.EndedDate IS NULL) AND (src.IsRecordDeleted = 1) THEN
			UPDATE SET tgt.[EndedBy] = src.[UpdatedBy], 
			tgt.[EndedDate] = src.[CombinedUpdatedDate];


		--EntityProductToSite
		-- No historical data maintained for map.tblEntityProductToSite. Simply flag the existing record as deleted if found, otherwise insert a new one.
		MERGE map.tblEntityProductToSite AS tgt
		USING  (SELECT [ProductToSiteKey], [ProductKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsRecordDeleted], [RecordUpdatedDate], [CombinedUpdatedDate] 
				FROM staging.tblEntityProductToSite WHERE ProductKey IS NOT NULL AND SiteSKey IS NOT NULL AND IgnoreRecord = 0) AS src
		ON tgt.ProductKey = src.ProductKey AND tgt.SiteSKey = src.SiteSKey
		WHEN NOT MATCHED THEN
			INSERT ([ProductToSiteKey], [ProductKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [EndedBy], [EndedDate])
			VALUES (src.[ProductToSiteKey], src.[ProductKey], src.[SiteSKey], src.[AssignedFromSiteSKey], src.[CreatedBy], src.[CreatedDate],
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[UpdatedBy] ELSE NULL END,
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[CombinedUpdatedDate] ELSE NULL END)
		WHEN MATCHED AND (tgt.EndedDate IS NULL) AND (src.IsRecordDeleted = 1) THEN
			UPDATE SET tgt.[EndedBy] = src.[UpdatedBy], 
			tgt.[EndedDate] = src.[CombinedUpdatedDate];

		
		--EntityTransactionAliasToSite
		-- No historical data maintained for map.tblEntityProductToSite. Simply flag the existing record as deleted if found, otherwise insert a new one.
		MERGE map.tblEntityTransactionAliasToSite AS tgt
		USING  (SELECT [TransactionAliasToSiteKey], [TransactionAliasKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsRecordDeleted], [RecordUpdatedDate], [CombinedUpdatedDate] 
				FROM staging.tblEntityTransactionAliasToSite WHERE TransactionAliasKey IS NOT NULL AND SiteSKey IS NOT NULL AND IgnoreRecord = 0) AS src
		ON tgt.TransactionAliasKey = src.TransactionAliasKey AND tgt.SiteSKey = src.SiteSKey
		WHEN NOT MATCHED THEN
			INSERT ([TransactionAliasToSiteKey], [TransactionAliasKey], [SiteSKey], [AssignedFromSiteSKey], [CreatedBy], [CreatedDate], [EndedBy], [EndedDate])
			VALUES (src.[TransactionAliasToSiteKey], src.[TransactionAliasKey], src.[SiteSKey], src.[AssignedFromSiteSKey], src.[CreatedBy], src.[CreatedDate],
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[UpdatedBy] ELSE NULL END,
				CASE WHEN (ISNULL(src.IsRecordDeleted, 0) = 1) THEN src.[CombinedUpdatedDate] ELSE NULL END)
		WHEN MATCHED AND (tgt.EndedDate IS NULL) AND (src.IsRecordDeleted = 1) THEN
			UPDATE SET tgt.[EndedBy] = src.[UpdatedBy], 
			tgt.[EndedDate] = src.[CombinedUpdatedDate];
									
					
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
						+ 'Procedure Name: [staging].[usp_LoadEntityToSiteMappings]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END