/*
	DROP PROCEDURE [Staging].[usp_SetTransactionLineItemEmptyIDFieldsToNull]

	EXEC [staging].[usp_SetTransactionLineItemEmptyIdFieldsToNull]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionLineItemEmptyIdFieldsToNull]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_SetTransactionLineItemEmptyIdFieldsToNull]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Set to NULL, the empty EntityIds of staging TransactionLineItem, so that they are treated just like a Null/missing value.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		-- TransactionLineItem Equipment ID references	
		UPDATE staging.tblTransactionLineItems
		SET DestinationCompartmentID = NULL
		WHERE LEN(TRIM(DestinationCompartmentID)) = 0
		AND IgnoreRecord = 0

		UPDATE staging.tblTransactionLineItems
		SET DestinationCompanyEquipmentID = NULL
		WHERE LEN(TRIM(DestinationCompanyEquipmentID)) = 0
		AND IgnoreRecord = 0


		UPDATE staging.tblTransactionLineItems
		SET SourceCompartmentID = NULL
		WHERE LEN(TRIM(SourceCompartmentID)) = 0
		AND IgnoreRecord = 0
											
		UPDATE staging.tblTransactionLineItems
		SET SourceEquipmentID = NULL
		WHERE LEN(TRIM(SourceEquipmentID)) = 0
		AND IgnoreRecord = 0
		


		-- TransactionLineItem Product references			
		UPDATE staging.tblTransactionLineItems
		SET ProductId = NULL
		WHERE LEN(TRIM(ProductId)) = 0
		AND IgnoreRecord = 0



		-- TransactionLineItem Station references			
		UPDATE staging.tblTransactionLineItems
		SET LoadingLocationID = NULL
		WHERE LEN(TRIM(LoadingLocationID)) = 0
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
						+ 'Procedure Name: [staging].[usp_SetTransactionLineItemEmptyIdFieldsToNull]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END