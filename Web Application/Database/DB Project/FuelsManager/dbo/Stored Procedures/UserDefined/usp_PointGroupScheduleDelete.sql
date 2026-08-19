CREATE PROCEDURE [dbo].[usp_PointGroupScheduleDelete]
	@PointGroupGuid uniqueidentifier,
	@UserGuid uniqueidentifier,
	@SiteGuid  uniqueidentifier
AS	
------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_PointGroupScheduleDelete]
	-- Author: Francisco Martin
	-- Version/Date: 1.0
	-- Purpose: Retrieve a Point Group by Guid
	-- Notes:
	-- 1. @PointGroupGuid: @PointGroupGuid is the Guid of the Point Group to retrieve
	-- 2. @UserGuid
	-- 3. @SiteGuid
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DELETE pg
		FROM [dbo].[tblPointGroupSchedule] pg
		WHERE pg.PointGroupGuid = @PointGroupGuid
		AND pg.UserGuid = @UserGuid
		AND pg.SiteGuid = @SiteGuid

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
						+ 'Procedure Name: [dbo].[usp_PointGroupScheduleDelete]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH  
