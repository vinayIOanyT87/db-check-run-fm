CREATE PROCEDURE [dbo].[usp_FuelCardLimitEnumerate]
	@SiteGuid UNIQUEIDENTIFIER,
	@ID NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@ID IS NOT NULL)
		BEGIN
			-- If the ID is provided, retrieve all fuel card limits 
			-- assigned to or owned by the site which partially match the id
			SELECT 
				tblFuelCardLimit.FuelCardLimitGuid,
				tblFuelCardLimit.ID,
				tblFuelCardLimit.SiteGuid
			FROM tblFuelCardLimit 
			WHERE EXISTS (SELECT * FROM map.tblEntityFuelCardLimitToSite 
				WHERE map.tblEntityFuelCardLimitToSite.SiteGuid = @SiteGuid
					AND map.tblEntityFuelCardLimitToSite.FuelCardLimitGuid = tblFuelCardLimit.FuelCardLimitGuid)
			AND tblFuelCardLimit.ID LIKE ('%' + @ID + '%')
			ORDER BY tblFuelCardLimit.ID
		END
		ELSE
		BEGIN
			-- Retrieve all fuel card limits 
			-- assigned to or owned by the site which partially match the id
			SELECT 
				tblFuelCardLimit.FuelCardLimitGuid,
				tblFuelCardLimit.ID,
				tblFuelCardLimit.SiteGuid
			FROM tblFuelCardLimit 
			WHERE EXISTS (SELECT * FROM map.tblEntityFuelCardLimitToSite 
				WHERE map.tblEntityFuelCardLimitToSite.SiteGuid = @SiteGuid
				AND map.tblEntityFuelCardLimitToSite.FuelCardLimitGuid = tblFuelCardLimit.FuelCardLimitGuid)
			ORDER BY tblFuelCardLimit.ID
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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)                 
						+ 'Procedure Name: usp_FuelCardLimitEnumerate' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	