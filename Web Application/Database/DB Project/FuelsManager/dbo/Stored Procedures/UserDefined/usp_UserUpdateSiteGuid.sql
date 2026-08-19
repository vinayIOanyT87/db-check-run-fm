/*
	EXEC [dbo].[usp_UserUpdateSiteGuid] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87'

*/
CREATE PROCEDURE [dbo].[usp_UserUpdateSiteGuid]
(
	@UserGuid uniqueidentifier,
    @SiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_UserUpdateSiteGuid]
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Updates the active directory user owner (SiteGuid) Guid to a new owner.
	-- Notes:
	-- 1. @UserGuid: The active directory user GUID used to update the user.
	-- 1. @SiteGuid: The new owner (SiteGuid) Guid for the user.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

        UPDATE tblUsers SET SiteGuid = @SiteGuid
        WHERE UserGuid = @UserGuid
		
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
						+ 'Procedure Name: [dbo].[usp_UserUpdateSiteGuid]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
