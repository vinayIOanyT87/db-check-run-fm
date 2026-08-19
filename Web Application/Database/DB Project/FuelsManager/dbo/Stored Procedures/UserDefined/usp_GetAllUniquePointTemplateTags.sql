
CREATE PROCEDURE [dbo].[usp_GetAllUniquePointTemplateTagNames]
(
	@TargetSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetAllUniquePointTemplateTagNames] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0.000 / 2016-12-12 
	-- Purpose: Retrieve the the list of unique point template tags in the system
	-- Notes:
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT DISTINCT ptt.ID
		FROM [dbo].[tblPointTemplateTag] ptt 
		JOIN map.tblEntityPointTemplateToSite pts
		ON pts.SiteGuid = @TargetSiteGuid
		AND pts.PointTemplateGuid = ptt.PointTemplateGuid
		ORDER BY ptt.ID

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
						+ 'Procedure Name: [dbo].usp_GetAllUniquePointTemplateTags' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END



