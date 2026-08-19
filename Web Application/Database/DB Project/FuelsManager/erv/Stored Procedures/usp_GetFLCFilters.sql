

/*
	EXEC [erv].[usp_GetFLCFilters] NULL
	EXEC [erv].[usp_GetFLCFilters] 'Product'
*/
CREATE PROCEDURE [erv].[usp_GetFLCFilters]
(
	@EntityTypeId nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetFLCFilters] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the Field Level Configuration filters for a given entity
	-- Notes:
	-- 1. 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		SELECT EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, FilterDisplayName 
		FROM erv.tblEntitySegmentTemplate
		WHERE ((EntityTypeId = @EntityTypeId) OR (@EntityTypeId IS NULL))
		AND FilterFieldName IS NOT NULL
		ORDER BY FilterDisplayName
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
						+ 'Procedure Name: dbo.usp_GetFLCFilters' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    	
END