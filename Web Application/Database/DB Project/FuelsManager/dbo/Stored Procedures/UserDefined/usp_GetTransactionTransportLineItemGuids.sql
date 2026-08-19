
CREATE PROCEDURE [dbo].[usp_GetTransactionTransportLineItemGuids]
(
	@TransactionGuidAndTransportOrderNumbers dbo.TransactionGuidAndTransportOrderNumberListType READONLY
)
AS
BEGIN
	BEGIN TRY
		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_GetTransactionTransportLineItemGuids
		-- Author: Ryan Hill
		-- Purpose: Using the provided list of TransactionGuid and TransportOrderNumbers, retrieve the primary keys of any
		-- existing records that match
		------------------------------------------------------------------------------------------------------
		SELECT InputTransactionGuidAndTransportOrderNumbers.TransactionGuid,
			InputTransactionGuidAndTransportOrderNumbers.TransportOrderNumber,
			tblTransactionTransportLineItems.TransactionTransportLineItemGuid		
		FROM @TransactionGuidAndTransportOrderNumbers InputTransactionGuidAndTransportOrderNumbers 
		INNER JOIN tblTransactionTransportLineItems ON InputTransactionGuidAndTransportOrderNumbers.TransactionGuid = tblTransactionTransportLineItems.TransactionGuid 
			AND InputTransactionGuidAndTransportOrderNumbers.TransportOrderNumber = tblTransactionTransportLineItems.TransportOrderNumber
	
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
						+ 'Procedure Name: usp_GetTransactionTransportLineItemGuids' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

