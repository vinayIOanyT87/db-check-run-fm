CREATE PROCEDURE [dbo].[usp_TransactionLinksDeleteList]
(
	@TransactionLinks dbo.TransactionLinksDeleteType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		
		DELETE FROM tblTransactionLinks 
		WHERE EXISTS (SELECT * FROM @TransactionLinks links WHERE tblTransactionLinks.LinkedTransID = links.LinkedTransID 
				AND tblTransactionLinks.OriginalTransID = links.OriginalTransID 
				AND tblTransactionLinks.TransactionLineItemGuid = links.TransactionLineItemGuid 
				AND tblTransactionLinks.LinkedTransactionLineItemGuid = links.LinkedTransactionLineItemGuid)

		-- If only the OriginalTransID is provided then delete all links that link to the TransID.
		DELETE FROM tblTransactionLinks
		WHERE LinkedTransID IN (SELECT links.LinkedTransID FROM @TransactionLinks links 
			WHERE links.LinkedTransID IS NOT NULL AND links.TransactionLineItemGuid IS NULL AND links.LinkedTransactionLineItemGuid IS NULL AND links.OriginalTransID IS NULL)

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
						+ 'Procedure Name: usp_TransactionLinksDeleteList' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 