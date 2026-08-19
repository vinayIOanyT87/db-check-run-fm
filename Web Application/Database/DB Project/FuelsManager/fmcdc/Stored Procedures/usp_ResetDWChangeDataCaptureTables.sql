/*
	DROP PROCEDURE [fmcdc].[usp_ResetDWChangeDataCaptureTables]

	EXEC [fmcdc].[usp_ResetDWChangeDataCaptureTables]
	
*/
CREATE PROCEDURE [fmcdc].[usp_ResetDWChangeDataCaptureTables]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [fmcdc].[usp_ResetDWChangeDataCaptureTables]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete the rows of the Data Warehouse Data Capture tables that have already been processed and captured in both the OLAP database
	--          and the Archive database.
	-- Notes:	
	-- 1. Only the tables covered by the Date Warehouse are reset by this operation.
	-- 2. This query assumes that the operation to capture the last RowVersion processed for each entity has already been executed, 
	--    and the results captured in table fmcdc.tblLastProcessedRowVersion.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		DECLARE @lastRowVersion bigint
		
		--DataWarehouse-only tables
		-- Site
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Site')
	
		DELETE fmcdc.tblSites
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- AutoDistributionCode
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('AutoDistributionCode')
	
		DELETE fmcdc.tblAutoDistributionReasonCodes
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)
		
		-- Product
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Product')
	
		DELETE fmcdc.tblProducts	
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)
		
		-- Company
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Company')
	
		DELETE fmcdc.tblCompanies	
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	
	
		-- Equipment
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Equipment')
	
		DELETE fmcdc.tblEquipment
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- EquipmentType
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EquipmentType')
	
		DELETE fmcdc.tblEquipmentTypes
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- Personnel
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Personnel')
	
		DELETE fmcdc.tblPersonnel
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- TransactionAlias
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionAlias')
	
		DELETE fmcdc.tblTransactionAliases
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	
				
		-- ApplicationString
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('ApplicationString')
	
		DELETE fmcdc.tblApplicationString
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)				

		-- LoadArm
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('LoadArm')
	
		DELETE fmcdc.tblLoadArms
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- Station
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Station')
	
		DELETE fmcdc.tblStations
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)

		-- Tank
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('Tank')
	
		DELETE fmcdc.tblTanks
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)
	
		-- MapSiteToSite
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('MapSiteToSite')
	
		DELETE fmcdc.tblSiteToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)			

		-- CompanyToUserGroup
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('CompanyToUserGroup')
	
		DELETE fmcdc.tblCompanyCompanyToUserGroup
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		-- UserToUserGroup
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('UserToUserGroup')
	
		DELETE fmcdc.tblUserToGroup
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		-- EntityEquipmentToSite
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityEquipmentToSite')
	
		DELETE fmcdc.tblEntityEquipmentToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		--EntityProductToSite
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityProductToSite')
	
		DELETE fmcdc.tblEntityProductToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

        --EntityUserToSite				
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityUserToSite')
	
		DELETE fmcdc.tblEntityUserToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		--tblEntityCompanyToSite				
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityCompanyToSite')
	
		DELETE fmcdc.tblEntityCompanyToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		--tblEntityPersonnelToSite				
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityPersonnelToSite')
	
		DELETE fmcdc.tblEntityPersonnelToSite
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		--tblEntityTransactionAliasToSite				
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('EntityTransactionAliasToSite')
	
		DELETE fmcdc.tblEntityTransactionAliasToSite 
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		-- User
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('User')
	
		DELETE fmcdc.tblUsers
		WHERE @lastRowVersion IS NOT NULL
		AND _RowVersion <= CONVERT(timestamp, @lastRowVersion)	

		-- TransactionHeader
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionHeader')
	
		DELETE fmcdc.tblTransactions
		WHERE @lastRowVersion IS NOT NULL
		AND InitialCDCRowVersion <= @lastRowVersion

		-- TransactionLineItem
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionLineItem')
	
		DELETE fmcdc.tblTransactionLineItems
		WHERE @lastRowVersion IS NOT NULL
		AND InitialCDCRowVersion <= @lastRowVersion	
		
		-- TransactionSubLineItem
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionSubLineItem')
	
		DELETE fmcdc.tblTransactionSubLineItems
		WHERE @lastRowVersion IS NOT NULL
		AND InitialCDCRowVersion <= @lastRowVersion

		-- TransactionUserData
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionUserData')
	
		DELETE fmcdc.tblTransactionUserData
		WHERE @lastRowVersion IS NOT NULL
		AND InitialCDCRowVersion <= @lastRowVersion

		-- TransactionLineItemUserData
		SELECT @lastRowVersion = [fmcdc].[udf_GetLastRowVersionProcessed]('TransactionLineItemUserData')
	
		DELETE fmcdc.tblTransactionLineItemUserData
		WHERE @lastRowVersion IS NOT NULL
		AND InitialCDCRowVersion <= @lastRowVersion
					
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
						+ 'Procedure Name: [fmcdc].[usp_ResetDWChangeDataCaptureTables]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO