/*
	DROP PROCEDURE [erv].[usp_ClearGlobalSpecificQueueRecords]

	[erv].[usp_ClearGlobalSpecificQueueRecords] '8A970C48-1B04-4DFB-83FD-01D734C84199'
*/

CREATE PROCEDURE [erv].[usp_ClearGlobalSpecificQueueRecords]
(
	@GSQueueGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_ClearGlobalSpecificQueueRecords] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete from the erv.tblGlobalSpecificChangesQueue table a given queue record and all queue records that were effectively also
	-- processed by virtue of processing that queue record
	-- Notes:
	-- 1. @GSQueueGuid: Guid of the queue record to be deleted
	-- 2. The presence of multiple queue entries for the same entity record indicates that the record was modified multiple times.
	--    When processing the first available queue entry for an entity record, the latest available state of that entity record would be read.
	--    This effectively means that all the entity record modifications flagged by the queue entries currently available for that record would be 
	--    processed together, and all those queue records can be marked as being processed, and be cleared together once the first queue record is 
	--    processed.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @batchProcessingMarker uniqueidentifier
		SELECT @batchProcessingMarker = BatchProcessingMarker FROM erv.tblGlobalSpecificChangesQueue
		WHERE GSQueueGuid = @GSQueueGuid

		DELETE a FROM erv.tblGlobalSpecificChangesQueue a
		WHERE EXISTS
		(
			SELECT * FROM erv.tblGlobalSpecificChangesQueue b
			WHERE b.GSQueueGuid = a.GSQueueGuid
			AND b.BatchProcessingMarker = @batchProcessingMarker 
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
						+ 'Procedure Name: [erv].usp_ClearGlobalSpecificQueueRecords' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
