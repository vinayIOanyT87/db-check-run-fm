/*
	DROP PROCEDURE [erv].[usp_GetFirstGlobalSpecificQueueRecord]

	[erv].[usp_GetFirstGlobalSpecificQueueRecord]
*/

CREATE PROCEDURE [erv].[usp_GetFirstGlobalSpecificQueueRecord]
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetGlobalSpecificQueueRecord] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the first available queue record not already being processed, and marks all the queue records for the same entity record 
	--          as being processed.
	-- Notes:
	-- 1. The GlobalSpecific queue record information returned are: The GSQueueGuid, the EntityTypeId, and the EntityGuid.
	-- 2. The presence of multiple queue entries for the same entity record indicates that the record was modified multiple times. 
	--    When processing the first available queue entry for an entity record, the latest available state of that entity record would be read.
	--    This effectively means that all the entity record modifications flagged by the queue entries currently available for that record would be 
	--    processed together, and all those queue records can be marked as being processed.
	------------------------------------------------------------------------------------------------------
	
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @tblTargetRecords TABLE
		(
			RecordIndex int Identity (1, 1) NOT NULL,
			GSQueueGuid uniqueidentifier,
			EntityTypeId nvarchar (100) NOT NULL,
			EntityGuid	uniqueidentifier NOT NULL
		);

		DECLARE @gsGuid uniqueidentifier
		DECLARE @entityTypeId nvarchar(100)
		DECLARE @entityGuid uniqueidentifier
		SELECT TOP(1) @gsGuid = GSQueueGuid, @entityTypeId = EntityTypeId, @entityGuid = EntityGuid FROM erv.tblGlobalSpecificChangesQueue
		WHERE BatchProcessingMarker IS NULL
		ORDER BY _RowVersion

		INSERT INTO @tblTargetRecords
		(GSQueueGuid, EntityTypeId, EntityGuid)
		SELECT GSQueueGuid, EntityTypeId, EntityGuid FROM erv.tblGlobalSpecificChangesQueue
		WHERE EntityTypeId = @entityTypeId
		AND EntityGuid = @entityGuid
		AND BatchProcessingMarker IS NULL
		ORDER BY _RowVersion

		DECLARE @batchMarker uniqueidentifier
		SET @batchMarker = NEWID()

		UPDATE a
		SET a.BatchProcessingMarker = @batchMarker
		FROM  erv.tblGlobalSpecificChangesQueue a
		INNER JOIN @tblTargetRecords b
		ON b.GSQueueGuid = a.GSQueueGuid
		
		SELECT TOP(1) GSQueueGuid, EntityTypeId, EntityGuid FROM @tblTargetRecords
		ORDER BY RecordIndex

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
						+ 'Procedure Name: [erv].usp_GetFirstGlobalSpecificQueueRecord' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
