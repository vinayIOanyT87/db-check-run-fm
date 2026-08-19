/*
       DROP PROCEDURE [archive].[usp_MarkArchivedBatchAsComplete]

       EXEC [archive].[usp_MarkArchivedBatchAsComplete] 'TransactionScope', 1000, 2000
       
*/
CREATE PROCEDURE [archive].[usp_MarkArchivedBatchAsComplete]
(
	@archiveScope nvarchar(100),
	@recordKeyStart bigint,
	@recordKeyEnd bigint
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored procedure: [fmcdc].[usp_MarkArchivedBatchAsComplete]
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
       -- Purpose: Set the IsProcessed flag for a batch of records in the relevant Archive Processed Records table for a given archive scope.
       -- Notes:
       -- 1. @archiveScope: Archive Scope to mark as processed/deleted
	   -- 2. @recordKeyStart: Start RecordKey of the batch. If NULL, then the IsProcessed flag for the whole recordset is set.
	   -- 2. @recordKeyEnd: End RecordKey of the batch
       ------------------------------------------------------------------------------------------------------
       SET NOCOUNT ON;
       BEGIN TRY

			IF (@archiveScope = 'AuditLogScope')
			BEGIN
				UPDATE archive.tblAuditLogLastProcessedRecords
				SET IsProcessed = 1
				WHERE ((RecordIndex BETWEEN @recordKeyStart AND @recordKeyEnd) OR (@recordKeyStart IS NULL))
			END
			ELSE IF (@archiveScope = 'AlarmAndEventLogScope')
			BEGIN
				UPDATE archive.tblAlarmAndEventLogLastProcessedRecords
				SET IsProcessed = 1
				WHERE ((RecordIndex BETWEEN @recordKeyStart AND @recordKeyEnd) OR (@recordKeyStart IS NULL))
			END
			ELSE IF (@archiveScope = 'TransactionScope')
			BEGIN
				IF (@recordKeyStart IS NULL)
				BEGIN
					UPDATE archive.tblTransactionLastProcessedRecords
					SET IsProcessed = 1
				END
				ELSE
				BEGIN
					UPDATE a
					SET a.IsProcessed = 1
					FROM archive.tblTransactionLastProcessedRecords a
					INNER JOIN archive.tblTransactionLastProcessedRecords b
					ON b.RecordGuid = a.ParentRecordGuid
					WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'
					AND b.RecordIndex BETWEEN @recordKeyStart AND @recordKeyEnd
				END
			END
                                  
       END TRY
       BEGIN CATCH        
              DECLARE       @_ErrMessage NVARCHAR(2048)      
                           , @_ErrNumber INT           
                           , @_ErrProcName NVARCHAR(126)           
                           , @_ErrLineNumber INT;            
              SET @_ErrMessage = ERROR_MESSAGE();        
              SET @_ErrNumber = ERROR_NUMBER();        
              SET @_ErrProcName= ERROR_PROCEDURE();        
              SET @_ErrLineNumber = ERROR_LINE();            
              SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
                                         + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
                                         + 'Procedure Name: [fmcdc].[usp_MarkArchivedBatchAsComplete]' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,16,1);      
       END CATCH    
       
END



GO
