/*
	DROP PROCEDURE [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]

	EXEC [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]

*/
CREATE PROCEDURE [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]
(
	@recordKeyStart bigint,
	@recordKeyEnd bigint
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete the FMSync tracking records associated with AuditLog records that have been archived.
	-- Notes:
	-- Notes:
	-- 1. @recordKeyStart: Start RecordKey of the batch. If set to NUll, the deletion executes without batching.
	-- 2. @recordKeyEnd: End RecordKey of the batch. 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		--AuditLog
		DELETE a
		FROM track.tblAuditLog a
		INNER JOIN archive.tblAuditLogLastProcessedRecords b
		on b.RecordGuid = a.PK_AuditLogGuid
		WHERE b.IsProcessed = 1
		AND ((b.RecordIndex BETWEEN @recordKeyStart AND @recordKeyEnd) OR (ISNULL(@recordKeyEnd, 0) = 0))

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
						+ 'Procedure Name: [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO




