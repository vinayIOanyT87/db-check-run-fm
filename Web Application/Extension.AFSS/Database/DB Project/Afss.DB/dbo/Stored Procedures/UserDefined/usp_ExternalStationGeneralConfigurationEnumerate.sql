CREATE PROCEDURE [dbo].[usp_ExternalStationGeneralConfigurationEnumerate]
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		SELECT 
			tblExternalStationGeneralConfiguration.ExternalStationGeneralConfigurationGuid, 
			tblExternalStationGeneralConfiguration.SiteGuid, 
			tblExternalStationGeneralConfiguration.RetailSaleTransactionAliasGuid,
			tblExternalStationGeneralConfiguration.DownloadTransactionsIntervalMinutes,
			tblExternalStationGeneralConfiguration.DownloadEventsIntervalMinutes,
			tblExternalStationGeneralConfiguration.CreatedBy,
			tblExternalStationGeneralConfiguration.CreatedDate,
			tblExternalStationGeneralConfiguration.UpdatedBy,
			tblExternalStationGeneralConfiguration.UpdatedDate,
			tblTransactionAliases.AliasName AS RetailSaleTransactionAliasName
		FROM tblExternalStationGeneralConfiguration
		-- When retrieving transaction aliases, keep record versioning in mind. We store the product MasterRecordGuid in the general configuration table.
		-- Since we're only getting the name of the alias here, it's OK to join on tblTransactionAliases.TransactionAliasGuid. Name can't change between master and child record versions. 
		-- But if you want to retrieve any other fields, you'd need to use erv.udf_GetTransactionAliasRecordVersions
		LEFT JOIN tblTransactionAliases ON tblExternalStationGeneralConfiguration.RetailSaleTransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid

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
						+ 'Procedure Name: usp_ExternalStationGeneralConfigurationEnumerate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	