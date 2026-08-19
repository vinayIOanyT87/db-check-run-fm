
CREATE PROCEDURE [dbo].[usp_GetTransactionSubLineItemGuids]
(
	@TransactionGuidAndSubLineItemSequences dbo.TransactionGuidAndSubLineItemSequenceListType READONLY
)
AS
BEGIN
	BEGIN TRY
		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_GetTransactionSubLineItemGuids
		-- Author: Ryan Hill
		-- Purpose: Given a list of TransactionLineItemGuids and line item SequenceIDs, attempt to match them up with existing records in the database and return 
		-- the TransactionSubLineItemGuid of the sub line item record
		-- The procedure will only return results for records where a match is found in the database, if no result is returned 
		-- that means that there was no match in the tblTransactionSubLineItems table.
		------------------------------------------------------------------------------------------------------

		SELECT InputTransactionGuidAndSubLineItemSequences.TransactionLineItemGuid, 
			InputTransactionGuidAndSubLineItemSequences.SequenceID,
			tblTransactionSubLineItems.TransactionSubLineItemGuid
		FROM @TransactionGuidAndSubLineItemSequences InputTransactionGuidAndSubLineItemSequences 
		INNER JOIN tblTransactionSubLineItems ON InputTransactionGuidAndSubLineItemSequences.TransactionLineItemGuid = tblTransactionSubLineItems.TransactionLineItemGuid 
			AND InputTransactionGuidAndSubLineItemSequences.SequenceID = tblTransactionSubLineItems.SequenceID
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
						+ 'Procedure Name: usp_GetTransactionSubLineItemGuids' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
