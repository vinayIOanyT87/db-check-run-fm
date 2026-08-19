CREATE PROCEDURE [dbo].[usp_GetMovementNodeModuleType]
(
	@PointGuid UNIQUEIDENTIFIER
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetMovementNodeModuleType] 
	-- Author: Warren Gray
	-- Version/Date: 1.0
	-- Purpose: Get Movement Module Type for Point
	-- Notes:
	-- 1. @PointGuid: Guid of the Point to retrieve
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT 
		CASE
		WHEN '26DE3166-5417-415C-9801-BB2E363D2447' = mpt.ModuleGuid THEN 1
		WHEN 'F769E8AF-1F5F-4EC7-A2E5-58759EF79186' = mpt.ModuleGuid THEN 2
		WHEN 'DB8313DD-E9BD-4BCF-8584-B3B6B33E827E' = mpt.ModuleGuid THEN 3
		ELSE 0
		END AS MovementNodeModuleType
		FROM [dbo].[tblPoint] p
		LEFT JOIN [map].[tblModuleToPointTemplate] mpt ON mpt.PointTemplateGuid = p.PointTemplateGuid
		WHERE p.PointGuid = @PointGuid AND mpt.ModuleGuid IN ('DB8313DD-E9BD-4BCF-8584-B3B6B33E827E','F769E8AF-1F5F-4EC7-A2E5-58759EF79186','26DE3166-5417-415C-9801-BB2E363D2447')

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
						+ 'Procedure Name: [dbo].[usp_GetMovementNodeModuleType]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END