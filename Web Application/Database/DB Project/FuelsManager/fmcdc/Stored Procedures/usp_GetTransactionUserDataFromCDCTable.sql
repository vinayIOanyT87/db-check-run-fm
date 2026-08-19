/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionUserDataFromCDCTable]

	EXEC [fmcdc].[usp_GetTransactionUserDataFromCDCTable] 10000, 0

	EXEC [fmcdc].[usp_GetTransactionUserDataFromCDCTable] 10000, 1
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionUserDataFromCDCTable]
(
	@batchExtractionSize  int,
	@isLastBatch bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [fmcdc].[usp_GetTransactionUserDataFromCDCTable]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves a batch of fmcdc.tblTransactionUserData records.
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
		DECLARE @batchNumHeader int
		DECLARE @batchNumUserData int
		DECLARE @runningBatchSize int
		DECLARE @fillupBatchSize int

		UPDATE fmcdc.tblTransactionUserData
		SET InitialCDCRowVersion = CONVERT(BigInt, _RowVersion)
		WHERE InitialCDCRowVersion IS NULL

		SET @batchNumHeader = (SELECT MAX(BatchProcessNumber) FROM fmcdc.tblTransactions WHERE IsProcessed IS NULL)
		SET @batchNumUserData = (SELECT MAX(BatchProcessNumber) FROM fmcdc.tblTransactionUserData)

		SELECT @batchNum = ISNULL(@batchNumUserData, 0) + 1
		IF ((@batchNumHeader IS NOT NULL) AND (ISNULL(@batchNumHeader, 0) > ISNULL(@batchNumUserData, 0)))
		BEGIN
			SELECT @batchNum = @batchNumHeader
		END

		--Retrieve the UserData records that match the parent records selected in the current batch
		IF (@isLastBatch = 0)
		BEGIN
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionUserData a
			INNER JOIN fmcdc.tblTransactions b
			ON b.TransactionGuid = a.TransactionGuid
			WHERE @batchNumHeader IS NOT NULL 
			AND b.IsProcessed IS NULL
			AND b.BatchProcessNumber = @batchNumHeader
			AND a.IsProcessed IS NULL
			AND a.BatchProcessNumber IS NULL
		END

		SET @runningBatchSize = (SELECT COUNT(*) FROM fmcdc.tblTransactionUserData WHERE BatchProcessNumber = @batchNum)
		SET @fillupBatchSize = ISNULL(@batchExtractionSize, 0) - ISNULL(@runningBatchSize, 0)
		
		
		IF ((@isLastBatch = 1) OR (ISNULL(@batchExtractionSize, 0) = 0))
		BEGIN
			UPDATE fmcdc.tblTransactionUserData
			SET BatchProcessNumber = @batchNum
			WHERE IsProcessed IS NULL
			AND BatchProcessNumber IS NULL
		END
		ELSE IF (ISNULL(@fillupBatchSize, 0) > 0)
		BEGIN	
			--Fill up the batch with records that do not have a parent header fmcdc record or get all remaining records if this is the last batch		
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionUserData a
			INNER JOIN
			(
				SELECT TOP(@fillupBatchSize) a.TransactionUserDataSKey FROM fmcdc.tblTransactionUserData a
				WHERE a.IsProcessed IS NULL
				AND a.BatchProcessNumber IS NULL
				AND NOT EXISTS
				(
					SELECT * FROM fmcdc.tblTransactions b
					WHERE b.TransactionGuid = a.TransactionGuid
				)
				ORDER BY a.TransactionUserDataSKey		
			) x
			ON x.TransactionUserDataSKey = a.TransactionUserDataSKey
			WHERE a.BatchProcessNumber IS NULL
		END

		IF (@isLastBatch <> 1)
		BEGIN
			--Add to the batch, records that were not initially selected, but that share the same TransactionUserDataGuid as those selected, i.e. other versions of the same records.
			UPDATE a
			SET a.BatchProcessNumber = @batchNum
			FROM fmcdc.tblTransactionUserData a
			INNER JOIN
			(
				SELECT b.TransactionUserDataSKey, b.TransactionUserDataGuid FROM fmcdc.tblTransactionUserData b
				INNER JOIN 
				(
					SELECT c.TransactionUserDataSKey, c.TransactionUserDataGuid FROM fmcdc.tblTransactionUserData c
					WHERE c.IsProcessed IS NULL
					AND c.BatchProcessNumber = @batchNum
				) k
				ON k.TransactionUserDataGuid = b.TransactionUserDataGuid
				WHERE b.IsProcessed IS NULL
				AND b.BatchProcessNumber IS NULL
			) x
			ON x.TransactionUserDataSKey = a.TransactionUserDataSKey
			WHERE a.BatchProcessNumber IS NULL
		END


		SELECT *, InitialCDCRowVersion RowVersionInt FROM fmcdc.tblTransactionUserData
		WHERE IsProcessed IS NULL
		AND BatchProcessNumber = @batchNum
		ORDER BY TransactionUserDataSKey
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionUserDataFromCDCTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO
