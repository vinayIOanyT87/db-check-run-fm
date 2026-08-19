/*
	DROP PROCEDURE [archive].[usp_PurgeProcessedTransactionRecords]

	EXEC [archive].[usp_PurgeProcessedTransactionRecords] 1000, 2000

*/
CREATE PROCEDURE [archive].[usp_PurgeProcessedTransactionRecords]
(
	@recordKeyStart bigint,
	@recordKeyEnd bigint
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_PurgeProcessedTransactionRecords]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete a batch of Transaction records that are referenced in the archive.tblTransactionLastProcessedRecords table.
	-- Notes:
	-- 1. @recordKeyStart: Start RecordKey of the batch. IF set to NUll, the deletion executes without batching.
	-- 2. @recordKeyEnd: End RecordKey of the batch
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @bini varbinary(128)
		SET @bini = 0x
		DECLARE  @context_info varbinary(128)
		SELECT  @context_info = cast('TransactionArchiving' + space(128) as binary(128))
		SET CONTEXT_INFO @context_info

		--Transaction Tables
		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblExportResultDetails] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResultDetails]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblExportResultDetails] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResultDetails]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))			
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblExportResultDetails]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblExportResults] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResults]'	
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblExportResults] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx			
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblExportResults]'		
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblExportResults]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionSubLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSubLineItems]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionSubLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx			
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSubLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionSubLineItems]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLineItemUserData] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItemUserData]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLineItemUserData] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItemUserData]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionLineItemUserData]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItems]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx			
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionLineItems]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLinks] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLinks]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionLinks] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionLinks]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionLinks]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionNotes] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionNotes]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionNotes] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionNotes]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionNotes]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionPIDX] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionPIDX]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionPIDX] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionPIDX]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionPIDX]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionSignature] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSignature]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionSignature] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionSignature]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionSignature]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))



		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionTransportLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionTransportLineItems]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionTransportLineItems] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionTransportLineItems]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionTransportLineItems]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionUserData] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionUserData]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionUserData] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionUserData]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionUserData]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionWeightReadings] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionWeightReadings]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactionWeightReadings] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] c
			ON c.RecordGuid = b.ParentRecordGuid
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactionWeightReadings]'
			AND c.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND c.RecordIndex >= @recordKeyStart 
			AND ((c.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactionWeightReadings]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))


		IF (@recordKeyStart IS NULL)
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactions] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'
		END
		ELSE
		BEGIN
			DELETE a
			FROM [dbo].[tblTransactions] a
			INNER JOIN [archive].[tblTransactionLastProcessedRecords] b
			ON b.RecordIndex = a._ClusterIdx
			WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'
			AND b.RecordIndex >= @recordKeyStart 
			AND ((b.RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))
		END

		UPDATE [archive].[tblTransactionLastProcessedRecords]
		SET IsProcessed = 1
		WHERE SourceArchiveTable = '[dbo].[tblTransactions]'
		AND ((RecordIndex >= @recordKeyStart) OR (@recordKeyStart IS NULL))
		AND ((@recordKeyStart IS NULL) OR (RecordIndex < @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))

		SET Context_Info @bini --reset context_info

	END TRY
	BEGIN CATCH  
		SET Context_Info @bini --reset context_info
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
						+ 'Procedure Name: [archive].[usp_PurgeProcessedTransactionRecords]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


