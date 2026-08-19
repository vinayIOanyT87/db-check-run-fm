
CREATE PROCEDURE [dbo].[usp_TransactionDeleteRemainingByTransVersion]
(
	@TransactionGuidsAndTransVersions TransactionGuidAndTransVersionListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY

		-- This procedure deletes line items, sub line items, transport line items, and any child records.
		-- The logic uses the TransVersion to detect which records were deleted.
		-- The TransVersion is a number incremented for each transaction when the transaction is modified.
		DELETE FROM tblTransactionTransportLineItems 
		WHERE EXISTS (SELECT * FROM @TransactionGuidsAndTransVersions transactionGuidsAndTransVersions
			WHERE transactionGuidsAndTransVersions.TransactionGuid = tblTransactionTransportLineItems.TransactionGuid 
			AND transactionGuidsAndTransVersions.TransVersion <> tblTransactionTransportLineItems.TransVersion)

		-- Delete any old transaction line items and the associated child records.
		DECLARE @LineItemGuids TABLE (TransactionLineItemGuid UNIQUEIDENTIFIER) 

		INSERT INTO @LineItemGuids 
		SELECT TransactionLineItemGuid 
		FROM tblTransactionLineItems 
		INNER JOIN @TransactionGuidsAndTransVersions transactionGuidsAndTransVersions ON tblTransactionLineItems.TransactionGuid = transactionGuidsAndTransVersions.TransactionGuid 
			AND tblTransactionLineItems.TransVersion <> transactionGuidsAndTransVersions.TransVersion

		DELETE FROM tblTransactionLinks WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM @LineItemGuids) 

		DELETE FROM tblTransactionLineItemUserData WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM @LineItemGuids) 

		DECLARE @SubLineItemGuids TABLE (TransactionSubLineItemGuid UNIQUEIDENTIFIER) 

		INSERT INTO @SubLineItemGuids
		SELECT TransactionSubLineItemGuid
		FROM tblTransactionSubLineItems WITH (NOLOCK)
		WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM @LineItemGuids) 
			OR EXISTS (SELECT * FROM @TransactionGuidsAndTransVersions transactionGuidsAndTransVersions
				WHERE transactionGuidsAndTransVersions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid 
				AND transactionGuidsAndTransVersions.TransVersion <> tblTransactionSubLineItems.TransVersion)

		-- Delete any sub line items that belong to deleted line items or were individually deleted
		DELETE FROM tblTransactionSubLineItems WHERE TransactionSubLineItemGuid in (select TransactionSubLineItemGuid from @SubLineItemGuids)

		DELETE FROM tblTransactionLineItems WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM @LineItemGuids) 	
		
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
						+ 'Procedure Name: usp_TransactionDeleteRemainingByTransVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 