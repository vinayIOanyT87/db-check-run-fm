/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionHeadersFromCDCTable]

	EXEC [fmcdc].[usp_GetTransactionHeadersFromCDCTable] 10000, 0

	EXEC [fmcdc].[usp_GetTransactionHeadersFromCDCTable] 10000, 1

	EXEC [fmcdc].[usp_GetTransactionHeadersFromCDCTable] 0, 0
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionHeadersFromCDCTable]
(
	@batchExtractionSize  int,
	@isLastBatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [fmcdc].[usp_GetTransactionHeadersFromCDCTable]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves a batch of fmcdc.tblTransactions records.
	-- Notes:
	-- 1. @batchExtractionSize: Number of records to be fetched.
	-- 2. @isLastBatch: 0: Intermediate extraction batch - fetch a regular batch of record.
	--					1: Last extraction batch - fetch all remaining unprocessed records

	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @batchNum int

		UPDATE fmcdc.tblTransactions
		SET InitialCDCRowVersion = CONVERT(BigInt, _RowVersion)
		WHERE InitialCDCRowVersion IS NULL

		SET @batchNum = (SELECT MAX(BatchProcessNumber) FROM fmcdc.tblTransactions)
		SELECT @batchNum = ISNULL(@batchNum, 0) + 1

		--Retrieve the next batch of records
		IF ((@isLastBatch = 1) OR (ISNULL(@batchExtractionSize, 0) = 0))
		BEGIN
			UPDATE fmcdc.tblTransactions
			SET BatchProcessNumber = @batchNum
			WHERE IsProcessed IS NULL
			AND BatchProcessNumber IS NULL
		END
		ELSE
		BEGIN
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactions a
			INNER JOIN
			(
				SELECT TOP(@batchExtractionSize) a.TransactionsSKey FROM fmcdc.tblTransactions a
				WHERE a.IsProcessed IS NULL
				AND a.BatchProcessNumber IS NULL
				ORDER BY a.TransactionsSKey			
			) x
			ON x.TransactionsSKey = a.TransactionsSKey
			WHERE a.BatchProcessNumber IS NULL

			--Add to the batch, records that were not initially selected, but that share the same TransactionGuid as those selected, i.e. other versions of the same records.
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactions a
			INNER JOIN
			(
				SELECT b.TransactionsSKey, b.TransactionGuid FROM fmcdc.tblTransactions b
				INNER JOIN 
				(
					SELECT c.TransactionsSKey, c.TransactionGuid FROM fmcdc.tblTransactions c
					WHERE c.IsProcessed IS NULL
					AND c.BatchProcessNumber = @batchNum
				) k
				ON k.TransactionGuid = b.TransactionGuid
				WHERE b.IsProcessed IS NULL
				AND b.BatchProcessNumber IS NULL
			) x
			ON x.TransactionsSKey = a.TransactionsSKey
			WHERE a.BatchProcessNumber IS NULL
		END


		SELECT *, InitialCDCRowVersion RowVersionInt FROM fmcdc.tblTransactions
		WHERE IsProcessed IS NULL
		AND BatchProcessNumber = @batchNum
		ORDER BY TransactionsSKey
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionHeadersFromCDCTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO