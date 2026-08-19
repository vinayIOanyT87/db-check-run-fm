

/*
	DROP PROCEDURE [erv].[usp_GetProcessSettings]


	EXEC [erv].[usp_GetProcessSettings]

*/

/*
------------------------------------------------------------------------------------------------------
-- Function: [erv].[usp_GetProcessSettings] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Procedure to return the ERV Process Settings
-- Notes:
------------------------------------------------------------------------------------------------------
*/
CREATE PROCEDURE [erv].[usp_GetProcessSettings]
AS
BEGIN
	BEGIN TRY	

		SELECT InhibitGlobalFieldsProcessing, UpdatedBy, UpdatedDate
		FROM erv.tblProcessSettings
		WHERE ProcessSettingsKey = (SELECT MAX(ProcessSettingsKey) FROM tblProcessSettings) 

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
						+ 'Procedure Name: [erv].usp_GetProcessSettings' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 