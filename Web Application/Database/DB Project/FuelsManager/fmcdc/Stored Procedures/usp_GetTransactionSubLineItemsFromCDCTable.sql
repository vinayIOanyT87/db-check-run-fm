/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable]

	EXEC [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable] 10000, 0

	EXEC [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable] 10000, 1
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable]
(
	@batchExtractionSize  int,
	@isLastBatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves a batch of fmcdc.tblTransactionSubLineItems records.
	-- Notes:
	-- 1. @batchExtractionSize: Number of records to be fetched.
	-- 2. @isLastBatch: 0: Intermediate extraction batch - fetch a regular batch of record.
	--					1: Last extraction batch - fetch all remaining unprocessed records

	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @startSKey int
		DECLARE @endSKey int
		DECLARE @batchNum int
		DECLARE @batchNumLineItem int
		DECLARE @batchNumSubLineItem int
		DECLARE @runningBatchSize int
		DECLARE @fillupBatchSize int

		UPDATE fmcdc.tblTransactionSubLineItems
		SET InitialCDCRowVersion = CONVERT(BigInt, _RowVersion)
		WHERE InitialCDCRowVersion IS NULL

		SET @batchNumLineItem = (SELECT MAX(BatchProcessNumber) FROM fmcdc.tblTransactionLineItems WHERE IsProcessed IS NULL)
		SET @batchNumSubLineItem = (SELECT MAX(BatchProcessNumber) FROM fmcdc.tblTransactionSubLineItems)

		SELECT @batchNum = ISNULL(@batchNumSubLineItem, 0) + 1
		IF ((@batchNumLineItem IS NOT NULL) AND (ISNULL(@batchNumLineItem, 0) > ISNULL(@batchNumSubLineItem, 0)))
		BEGIN
			SELECT @batchNum = @batchNumLineItem
		END

		--Retrieve the SubLineItems records that match the parent records selected in the current batch
		IF (@isLastBatch = 0)
		BEGIN
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionSubLineItems a
			INNER JOIN fmcdc.tblTransactionLineItems b
			ON b.TransactionLineItemGuid = a.TransactionLineItemGuid
			WHERE @batchNumLineItem IS NOT NULL 
			AND b.IsProcessed IS NULL
			AND b.BatchProcessNumber = @batchNumLineItem
			AND a.IsProcessed IS NULL
			AND a.BatchProcessNumber IS NULL
		END

		SET @runningBatchSize = (SELECT COUNT(*) FROM fmcdc.tblTransactionSubLineItems WHERE BatchProcessNumber = @batchNum)
		SET @fillupBatchSize = ISNULL(@batchExtractionSize, 0) - ISNULL(@runningBatchSize, 0)
		
		
		IF ((@isLastBatch = 1) OR (ISNULL(@batchExtractionSize, 0) = 0))
		BEGIN
			UPDATE fmcdc.tblTransactionSubLineItems
			SET BatchProcessNumber = @batchNum
			WHERE IsProcessed IS NULL
			AND BatchProcessNumber IS NULL
		END
		ELSE IF (ISNULL(@fillupBatchSize, 0) > 0)
		BEGIN	
			--Fill up the batch with records that do not have a parent LineItem fmcdc record or get all remaining records if this is the last batch		
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionSubLineItems a
			INNER JOIN
			(
				SELECT TOP(@fillupBatchSize) a.TransactionSubLineItemsSKey FROM fmcdc.tblTransactionSubLineItems a
				WHERE a.IsProcessed IS NULL
				AND a.BatchProcessNumber IS NULL
				AND NOT EXISTS
				(
					SELECT * FROM fmcdc.tblTransactionLineItems b
					WHERE b.TransactionLineItemGuid = a.TransactionLineItemGuid
				)
				ORDER BY a.TransactionSubLineItemsSKey		
			) x
			ON x.TransactionSubLineItemsSKey = a.TransactionSubLineItemsSKey
			WHERE a.BatchProcessNumber IS NULL
		END

		IF (@isLastBatch <> 1)
		BEGIN
			--Add to the batch, records that were not initially selected, but that share the same TransactionSubLineItemGuid as those selected, i.e. other versions of the same records.
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionSubLineItems a
			INNER JOIN
			(
				SELECT b.TransactionSubLineItemsSKey, b.TransactionSubLineItemGuid FROM fmcdc.tblTransactionSubLineItems b
				INNER JOIN 
				(
					SELECT c.TransactionSubLineItemsSKey, c.TransactionSubLineItemGuid FROM fmcdc.tblTransactionSubLineItems c
					WHERE c.IsProcessed IS NULL
					AND c.BatchProcessNumber = @batchNum
				) k
				ON k.TransactionSubLineItemGuid = b.TransactionSubLineItemGuid
				WHERE b.IsProcessed IS NULL
				AND b.BatchProcessNumber IS NULL
			) x
			ON x.TransactionSubLineItemsSKey = a.TransactionSubLineItemsSKey
			WHERE a.BatchProcessNumber IS NULL
		END


		SELECT *, InitialCDCRowVersion RowVersionInt FROM fmcdc.tblTransactionSubLineItems
		WHERE IsProcessed IS NULL
		AND BatchProcessNumber = @batchNum
		ORDER BY TransactionSubLineItemsSKey
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionSubLineItemsFromCDCTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO
