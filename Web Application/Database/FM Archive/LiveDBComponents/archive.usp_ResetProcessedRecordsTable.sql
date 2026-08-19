/*
	DROP PROCEDURE [archive].[usp_ResetProcessedRecordsTable]

	EXEC [archive].[usp_ResetProcessedRecordsTable]

*/
CREATE PROCEDURE [archive].[usp_ResetProcessedRecordsTable]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_ResetProcessedRecordsTable]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete all the entries from the archive.tblProcessedRecords table.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		TRUNCATE TABLE archive.tblAlarmAndEventLogLastProcessedRecords

		TRUNCATE TABLE archive.tblAuditLogLastProcessedRecords

		TRUNCATE TABLE archive.tblTransactionLastProcessedRecords
					
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
						+ 'Procedure Name: [archive].[usp_ResetProcessedRecordsTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


