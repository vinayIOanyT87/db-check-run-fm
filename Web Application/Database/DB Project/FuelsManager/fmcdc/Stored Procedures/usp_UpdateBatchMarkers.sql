/*
	DROP PROCEDURE [fmcdc].[usp_UpdateBatchMarkers]

	EXEC [fmcdc].[usp_UpdateBatchMarkers] 1

	EXEC [fmcdc].[usp_UpdateBatchMarkers] 0
*/
CREATE PROCEDURE [fmcdc].[usp_UpdateBatchMarkers]
(
	@resetBatchMarkers bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [fmcdc].[usp_UpdateBatchMarkers]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Set the IsProcessed flag of the last batch that was ran for all applicable fmcdc tables
	-- Notes:
	-- 1. @resetBatchMarkers: 1 - Reset the IsProcessed flag as well as the BatchProcessNumber of all records to NULL
	------------------------------------------------------------------------------------------------------

	SET NOCOUNT ON;
	BEGIN TRY		

		DECLARE @lastExecutedBatch int

		SELECT @lastExecutedBatch = MAX(BatchProcessNumber) FROM fmcdc.tblTransactions
		UPDATE fmcdc.tblTransactions
		SET IsProcessed = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE 1 END),
		BatchProcessNumber = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE BatchProcessNumber END)
		WHERE 
		(
			(@resetBatchMarkers = 1)
			OR ((IsProcessed IS NULL) AND BatchProcessNumber = @lastExecutedBatch)
		)


		SELECT @lastExecutedBatch = MAX(BatchProcessNumber) FROM fmcdc.tblTransactionUserData
		UPDATE fmcdc.tblTransactionUserData
		SET IsProcessed = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE 1 END),
		BatchProcessNumber = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE BatchProcessNumber END)
		WHERE 
		(
			(@resetBatchMarkers = 1)
			OR ((IsProcessed IS NULL) AND BatchProcessNumber = @lastExecutedBatch)
		)


		SELECT @lastExecutedBatch = MAX(BatchProcessNumber) FROM fmcdc.tblTransactionLineItems
		UPDATE fmcdc.tblTransactionLineItems
		SET IsProcessed = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE 1 END),
		BatchProcessNumber = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE BatchProcessNumber END)
		WHERE 
		(
			(@resetBatchMarkers = 1)
			OR ((IsProcessed IS NULL) AND BatchProcessNumber = @lastExecutedBatch)
		)


		SELECT @lastExecutedBatch = MAX(BatchProcessNumber) FROM fmcdc.tblTransactionLineItemUserData
		UPDATE fmcdc.tblTransactionLineItemUserData
		SET IsProcessed = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE 1 END),
		BatchProcessNumber = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE BatchProcessNumber END)
		WHERE 
		(
			(@resetBatchMarkers = 1)
			OR ((IsProcessed IS NULL) AND BatchProcessNumber = @lastExecutedBatch)
		)


		SELECT @lastExecutedBatch = MAX(BatchProcessNumber) FROM fmcdc.tblTransactionSubLineItems
		UPDATE fmcdc.tblTransactionSubLineItems
		SET IsProcessed = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE 1 END),
		BatchProcessNumber = ( CASE WHEN (@resetBatchMarkers = 1) THEN NULL ELSE BatchProcessNumber END)
		WHERE 
		(
			(@resetBatchMarkers = 1)
			OR ((IsProcessed IS NULL) AND BatchProcessNumber = @lastExecutedBatch)
		)
					
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
						+ 'Procedure Name: [fmcdc].[usp_UpdateBatchMarkers]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END