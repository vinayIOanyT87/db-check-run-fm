/*
	DROP PROCEDURE [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]

	EXEC [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]

*/
CREATE PROCEDURE [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]
(
	@recordKeyStart bigint,
	@recordKeyEnd bigint
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete the FMSync tracking records associated with Transaction records that have been archived.
	-- Notes:
	-- Notes:
	-- 1. @recordKeyStart: Start RecordKey of the batch. IF set to NUll, the deletion executes without batching.
	-- 2. @recordKeyEnd: End RecordKey of the batch
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		--Transaction
		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblExportResultDetails a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_ExportResultDetailGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResultDetails]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblExportResultDetails a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_ExportResultDetailGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResultDetails]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END

		
		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblExportResults a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_ExportResultGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResults]'	
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblExportResults a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_ExportResultGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResults]'	
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionLineItems a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionLineItemGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItems]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionLineItems a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionLineItemGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionLineItemUserData a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionLineItemUserDataGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItemUserData]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionLineItemUserData a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionLineItemUserDataGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItemUserData]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionLinks a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionLinkGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLinks]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionLinks a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionLinkGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLinks]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		
	
		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionNotes a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionNoteGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionNotes]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionNotes a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionNoteGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionNotes]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		
	
		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionPIDX a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionPIDXGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionPIDX]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionPIDX a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionPIDXGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionPIDX]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactions a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactions a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND b.RecordIndex >= @recordKeyStart 
			AND ((b.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionSignature a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionSignatureGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSignature]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionSignature a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionSignatureGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSignature]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionSubLineItems a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionSubLineItemGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSubLineItems]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionSubLineItems a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionSubLineItemGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSubLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionTransportLineItems a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionTransportLineItemGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionTransportLineItems]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionTransportLineItems a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionTransportLineItemGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionTransportLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionUserData a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionUserDataGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionUserData]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionUserData a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionUserDataGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionUserData]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END
		

		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM track.tblTransactionWeightReadings a
			INNER JOIN archive.tblTransactionLastProcessedRecords b
			ON b.RecordGuid = a.PK_TransactionWeightReadingGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionWeightReadings]'
			AND b.IsProcessed = 1
		END
		ELSE
		BEGIN
			DELETE a
			FROM track.tblTransactionWeightReadings a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordGuid = a.PK_TransactionWeightReadingGuid
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionWeightReadings]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END

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
						+ 'Procedure Name: [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO




