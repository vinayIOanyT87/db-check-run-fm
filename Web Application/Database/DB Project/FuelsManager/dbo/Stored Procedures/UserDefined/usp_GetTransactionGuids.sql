
CREATE PROCEDURE [dbo].[usp_GetTransactionGuids]
(
	@TransIDs dbo.TransIDListType READONLY
)
AS
BEGIN
	BEGIN TRY
		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_GetTransactionGuids
		-- Author: Ryan Hill
		-- Purpose: Given a list of TransIDs, return the corresponding Primary Key values associated with the Transaction Header for each TransID.
		-- The procedure will only return results for records where a match is found in the database, if no result is returned for a particular TransID
		-- that means that there was no match in the tblTransactions table.
		------------------------------------------------------------------------------------------------------

		SELECT InputTransIDs.TransID, 
			tblTransactions.TransactionGuid, 
			tblTransactionNotes.TransactionNoteGuid, 
			tblTransactionUserData.TransactionUserDataGuid,
			tblTransactionSignature.TransactionSignatureGuid
		FROM @TransIDs InputTransIDs 
		INNER JOIN tblTransactions ON InputTransIDs.TransID = tblTransactions.TransID
		LEFT JOIN tblTransactionNotes ON tblTransactions.TransactionGuid = tblTransactionNotes.TransactionGuid
		LEFT JOIN tblTransactionUserData ON tblTransactions.TransactionGuid = tblTransactionUserData.TransactionGuid
		LEFT JOIN tblTransactionSignature ON tblTransactions.TransactionGuid = tblTransactionSignature.TransactionGuid
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
						+ 'Procedure Name: usp_GetTransactionGuids' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

