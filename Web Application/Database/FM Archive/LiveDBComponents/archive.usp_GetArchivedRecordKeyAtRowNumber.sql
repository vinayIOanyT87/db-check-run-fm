/*
       DROP PROCEDURE [archive].[usp_GetArchivedRecordKeyAtRowNumber]

       EXEC [archive].[usp_GetArchivedRecordKeyAtRowNumber] '[dbo].[tblAuditLog]', 100000
       
*/
CREATE PROCEDURE [archive].[usp_GetArchivedRecordKeyAtRowNumber]
(
	@sourceArchiveTable nvarchar(100),
	@rowNumber int
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored procedure: [fmcdc].[usp_GetArchivedRecordKeyAtRowNumber]
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
       -- Purpose: Retrieves the RecordKey (_ClusterIdx) of a given source table record that is 
       --          located at a given row number of the archive.tblLastProcessedRecords table ordered by _ClusterIdx.
       -- Notes:
       -- 1. @sourceArchiveTable: Name of source table for which to retrieve the key value from the archived record list
	   -- 2. @rowNumber: Row number at which to retrieve the AuditLogKey
       ------------------------------------------------------------------------------------------------------
       SET NOCOUNT ON;
       BEGIN TRY
			  IF (@sourceArchiveTable = '[dbo].[tblAuditLog]')
			  BEGIN
				  SELECT CONVERT(BigInt, x.RecordIndex) FROM
				  (
						 SELECT ROW_NUMBER() OVER (ORDER BY RecordIndex ASC) As RowNum, RecordIndex FROM archive.tblAuditLogLastProcessedRecords
				  ) x
				  WHERE x.RowNum =@rowNumber
			  END
			  ELSE IF (@sourceArchiveTable = '[dbo].[tblAlarmAndEventLog]')
			  BEGIN
				  SELECT CONVERT(BigInt, x.RecordIndex) FROM
				  (
						 SELECT ROW_NUMBER() OVER (ORDER BY RecordIndex ASC) As RowNum, RecordIndex FROM archive.tblAlarmAndEventLogLastProcessedRecords
				  ) x
				  WHERE x.RowNum =@rowNumber
			  END
			  ELSE IF (@sourceArchiveTable = '[dbo].[tblTransactions]')
			  BEGIN
				  SELECT CONVERT(BigInt, x.RecordIndex) FROM
				  (
						 SELECT ROW_NUMBER() OVER (ORDER BY RecordIndex ASC) As RowNum, RecordIndex FROM archive.tblTransactionLastProcessedRecords
						 WHERE SourceArchiveTable = @sourceArchiveTable
				  ) x
				  WHERE x.RowNum =@rowNumber
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
                                         + 'Procedure Name: [fmcdc].[usp_GetArchivedRecordKeyAtRowNumber]' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,16,1);      
       END CATCH    
       
END



GO
