CREATE PROCEDURE [dbo].[usp_PointTemplateGetDuplicate]
(
	@ID nvarchar(60),  
	@SiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_PointTemplateGetDuplicate] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0
	-- Purpose: Retrieve a Point Group Duplicate CustomPointTemplateGuid
	-- Notes:
	-- 1. @ID: ID of the point group
	-- 2. @SiteGuid: Target owner site of the record version that needs to be retrieved.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT pg.PointTemplateGuid
 		FROM tblPointTemplate pg
		WHERE pg.ID = @ID
		AND pg.SiteGuid = @SiteGuid
		AND pg.Standard = 0

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
						+ 'Procedure Name: [dbo].[usp_PointTemplateGetDuplicate]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END

GO


