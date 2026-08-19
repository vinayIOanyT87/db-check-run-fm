CREATE PROCEDURE [dbo].[usp_TransactionLinksGetLineItemsWithParentAssociations]
(
	@LineItemGuids dbo.GuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- Using the line item guids provided, pick out the line item guids which actually have parent associated transactions (a record in tblTransactionLinks matching on LinkedTransactionLineItemGuid)
		SELECT lineItemGuids.[Guid]
		FROM @LineItemGuids lineItemGuids
		WHERE EXISTS (SELECT * FROM tblTransactionLinks WHERE tbltransactionLinks.LinkedTransactionLineItemGuid = lineItemGuids.[Guid])
		
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
						+ 'Procedure Name: usp_TransactionLinksGetLineItemsWithParentAssociations' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
