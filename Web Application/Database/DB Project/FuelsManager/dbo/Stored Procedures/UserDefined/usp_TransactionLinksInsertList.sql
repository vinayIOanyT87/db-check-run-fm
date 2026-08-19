CREATE PROCEDURE [dbo].[usp_TransactionLinksInsertList]
(
	@TransactionLinks dbo.TransactionLinksType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY

		INSERT INTO tblTransactionLinks 			
		(
			SiteGuid, 
			OriginalTransID, 
			LinkedTransID, 
			[Level], 
			TransactionLineItemGuid, 
			LinkedTransactionLineItemGuid, 
			CreatedDate, 
			CreatedBy, 
			UpdatedDate,
			UpdatedBy			
		)
		SELECT
			SiteGuid, 
			OriginalTransID, 
			LinkedTransID, 
			[Level], 
			TransactionLineItemGuid, 
			LinkedTransactionLineItemGuid,
			SYSDATETIMEOFFSET(), --CreatedDate
			CreatedUpdatedBy,
			SYSDATETIMEOFFSET(), --UpdatedDate
			CreatedUpdatedBy
		FROM @TransactionLinks

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
						+ 'Procedure Name: dbo.usp_TransactionLinksInsertList' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 