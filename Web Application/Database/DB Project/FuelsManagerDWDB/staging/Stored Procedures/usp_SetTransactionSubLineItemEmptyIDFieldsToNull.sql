
/*
	DROP PROCEDURE [Staging].[usp_SetTransactionSubLineItemEmptyIdFieldsToNull]

	EXEC [staging].[usp_SetTransactionSubLineItemEmptyIdFieldsToNull]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionSubLineItemEmptyIdFieldsToNull]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_SetTransactionSubLineItemEmptyIdFieldsToNull]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Set to NULL, the empty EntityIds of staging TransactionSubLineItem, so that they are treated just like a Null/missing value.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		-- TransactionSubLineItem Product ID references	
		UPDATE staging.tblTransactionSubLineItems
		SET ProductId = NULL
		WHERE LEN(TRIM(ProductId)) = 0
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
						+ 'Procedure Name: [staging].[usp_SetTransactionSubLineItemEmptyIdFieldsToNull]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
