
CREATE PROCEDURE [dbo].[usp_GetTransactionLineItemGuids]
(
	@TransactionGuidAndLineItemSequences dbo.TransactionGuidAndLineItemSequenceListType READONLY
)
AS
BEGIN
	BEGIN TRY
		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_GetTransactionLineItemGuids
		-- Author: Ryan Hill
		-- Purpose: Given a list of TransactionGuids and line item SequenceIDs, attempt to match them up with existing records in the database and return 
		-- the primary key values of the line item and user data records
		-- The procedure will only return results for records where a match is found in the database, if no result is returned 
		-- that means that there was no match in the tblTransactionLineItems table.
		------------------------------------------------------------------------------------------------------

		SELECT InputTransactionGuidAndLineItemSequences.TransactionGuid, 
			InputTransactionGuidAndLineItemSequences.SequenceID, 
			tblTransactionLineItems.TransactionLineItemGuid,
			tblTransactionLineItemUserData.TransactionLineItemUserDataGuid
		FROM @TransactionGuidAndLineItemSequences InputTransactionGuidAndLineItemSequences 
		INNER JOIN tblTransactionLineItems ON InputTransactionGuidAndLineItemSequences.TransactionGuid = tblTransactionLineItems.TransactionGuid 
			AND InputTransactionGuidAndLineItemSequences.SequenceID = tblTransactionLineItems.SequenceID
		LEFT JOIN tblTransactionLineItemUserData ON tblTransactionLineItems.TransactionLineItemGuid = tblTransactionLineItemUserData.TransactionLineItemGuid

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
						+ 'Procedure Name: usp_GetTransactionLineItemGuids' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
