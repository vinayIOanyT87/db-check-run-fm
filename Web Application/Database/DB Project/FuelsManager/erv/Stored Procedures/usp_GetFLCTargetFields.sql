

/*
	EXEC [erv].[usp_GetFLCTargetFields] 'Equipment'
*/
CREATE PROCEDURE [erv].[usp_GetFLCTargetFields]
(
	@EntityTypeId nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetFLCTargetFields] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the Field Level Configuration Target Fields for a given entity type
	-- Notes:
	-- 1. 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		
		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		EXEC [erv].[usp_GetFieldLevelConfigMatrix] @EntityTypeId, NULL, NULL, NULL, NULL, NULL, @callingRefGuid, 0
	
		SELECT DISTINCT TargetField FROM erv.tblTempFieldLevelConfigMatrix	
		WHERE _CallingReferenceGuid = @callingRefGuid		
		ORDER BY TargetField

		DELETE erv.tblTempFieldLevelConfigMatrix
		WHERE _CallingReferenceGuid = @callingRefGuid

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
						+ 'Procedure Name: dbo.usp_GetFLCTargetFields' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    	
END