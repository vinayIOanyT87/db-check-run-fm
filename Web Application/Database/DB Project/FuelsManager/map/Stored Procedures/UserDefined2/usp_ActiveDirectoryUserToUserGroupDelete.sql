/*
	EXEC [map].[usp_ActiveDirectoryUserToUserGroupDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', null, null

*/
CREATE PROCEDURE [map].[usp_ActiveDirectoryUserToUserGroupDelete]
(
	@UserGuid uniqueidentifier,
	@SiteGuid uniqueidentifier,
	@UserGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_ActiveDirectoryUserToUserGroupDelete]
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes an active directory user from the map user to user group table.
	-- Notes:
	-- 1. @UserGuid: The active directory user GUID used to delete the mapping.
	-- 2. @SiteGuid: The site GUID used to delete the mapping.
	-- 3. @UserGroupGuid: The user group GUID used to delete the mapping.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF ((SELECT COUNT(*) FROM tblUsers WHERE UserGuid = @UserGuid AND ActiveDirectoryUser = 1) > 0)
		BEGIN
			IF (@SiteGuid IS NULL AND @UserGroupGuid IS NULL)
			BEGIN
				DELETE FROM map.tblUserToGroup WHERE UserGuid = @UserGuid
			END
			ELSE IF (@SiteGuid IS NOT NULL AND @UserGroupGuid IS NULL)
			BEGIN
				DELETE FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid
			END
			ELSE IF (@SiteGuid IS NULL AND @UserGroupGuid IS NOT NULL)
			BEGIN
				DELETE FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND GroupGuid = @UserGroupGuid
			END
			ELSE IF (@SiteGuid IS NOT NULL AND @UserGroupGuid IS NOT NULL)
			BEGIN
				DELETE FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND GroupGuid = @UserGroupGuid AND SiteGuid = @SiteGuid
			END
		END
		
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
						+ 'Procedure Name: map.usp_ActiveDirectoryUserToUserGroupDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
