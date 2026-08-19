/*
	DROP PROCEDURE [staging].[usp_CaptureInventoryYears]

	EXEC [staging].[usp_CaptureInventoryYears]
	
*/
CREATE PROCEDURE [staging].[usp_CaptureInventoryYears]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_CaptureInventoryYear]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Capture the distinct inventory years processed in the current batch of transactions.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		INSERT INTO staging.tblProcessedInventoryYears
		(InventoryYear)
		SELECT DISTINCT Year(a.InventoryDate) FROM staging.tblTransactions a
		WHERE NOT EXISTS
		(
			SELECT * FROM staging.tblProcessedInventoryYears b
			WHERE b.InventoryYear = Year(a.InventoryDate)
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
						+ 'Procedure Name: [staging].[usp_CaptureInventoryYears]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END