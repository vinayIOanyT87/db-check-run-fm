/*
	DROP PROCEDURE [Staging].[usp_SetTransactionHeaderEmptyIdFieldsToNull]

	EXEC [staging].[usp_SetTransactionHeaderEmptyIdFieldsToNull]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionHeaderEmptyIdFieldsToNull]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_usp_SetTransactionHeaderEmptyIdFieldsToNull]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Set to NULL, the empty EntityIds of staging TransactionHeader, so that they are treated just like a Null/missing value.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		
		-- TransactionHeader AutoDistributionReasonCode ID references
		UPDATE staging.tblTransactions
		SET ReasonCode = NULL
		WHERE LEN(TRIM(ReasonCode)) = 0
		AND IgnoreRecord = 0
				


		-- TransactionHeader Commpany ID references			
		UPDATE staging.tblTransactions
		SET BillToId = NULL
		WHERE LEN(TRIM(BillToId)) = 0
		AND IgnoreRecord = 0

		UPDATE staging.tblTransactions
		SET CarrierID = NULL
		WHERE LEN(TRIM(CarrierID)) = 0
		AND IgnoreRecord = 0
			
		
		UPDATE staging.tblTransactions
		SET ManagerID = NULL
		WHERE LEN(TRIM(ManagerID)) = 0
		AND IgnoreRecord = 0

		
		UPDATE staging.tblTransactions
		SET OwnerID = NULL
		WHERE LEN(TRIM(OwnerID)) = 0
		AND IgnoreRecord = 0

			
		UPDATE staging.tblTransactions
		SET ShipperID = NULL
		WHERE LEN(TRIM(ShipperID)) = 0
		AND IgnoreRecord = 0
		

		UPDATE staging.tblTransactions
		SET ShipToID = NULL
		WHERE LEN(TRIM(ShipToID)) = 0
		AND IgnoreRecord = 0


		UPDATE staging.tblTransactions
		SET SupplierID = NULL
		WHERE LEN(TRIM(SupplierID)) = 0
		AND IgnoreRecord = 0

	

		-- TransactionHeader Equipment ID references		
		UPDATE staging.tblTransactions
		SET DestinationCompanyEquipmentID1 = NULL
		WHERE LEN(TRIM(DestinationCompanyEquipmentID1)) = 0
		AND IgnoreRecord = 0


		UPDATE staging.tblTransactions
		SET DestinationCompanyEquipmentID2 = NULL
		WHERE LEN(TRIM(DestinationCompanyEquipmentID2)) = 0
		AND IgnoreRecord = 0


		UPDATE staging.tblTransactions
		SET DestinationCompanyEquipmentID3 = NULL
		WHERE LEN(TRIM(DestinationCompanyEquipmentID3)) = 0
		AND IgnoreRecord = 0

	
		UPDATE staging.tblTransactions
		SET SourceCompanyEquipmentID1 = NULL
		WHERE LEN(TRIM(SourceCompanyEquipmentID1)) = 0
		AND IgnoreRecord = 0

			
		UPDATE staging.tblTransactions
		SET SourceCompanyEquipmentID2 = NULL
		WHERE LEN(TRIM(SourceCompanyEquipmentID2)) = 0
		AND IgnoreRecord = 0

		
		UPDATE staging.tblTransactions
		SET SourceCompanyEquipmentID3 = NULL
		WHERE LEN(TRIM(SourceCompanyEquipmentID3)) = 0
		AND IgnoreRecord = 0



		-- TransactionHeader Personnel ID references		
		UPDATE staging.tblTransactions
		SET OperatorID = NULL
		WHERE LEN(TRIM(OperatorID)) = 0
		AND IgnoreRecord = 0



		-- TransactionHeader TransactionAlias references		
		UPDATE staging.tblTransactions
		SET TransactionAliasName = NULL
		WHERE LEN(TRIM(TransactionAliasName)) = 0
		AND IgnoreRecord = 0

					
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
						+ 'Procedure Name: [staging].[usp_SetTransactionHeaderEmptyIdFieldsToNull]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END