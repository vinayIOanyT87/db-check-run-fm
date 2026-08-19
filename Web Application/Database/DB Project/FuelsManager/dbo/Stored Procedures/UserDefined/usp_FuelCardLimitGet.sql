CREATE PROCEDURE [dbo].[usp_FuelCardLimitGet]
	@FuelCardLimitGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@ID NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF(@FuelCardLimitGuid IS NOT NULL)
		BEGIN
			-- Retrieve the Fuel Card Limit record by its primary key
			SELECT 
				FuelCardLimitGuid,
				ID,
				SiteGuid,
				CreatedBy,
				CreatedDate,
				UpdatedBy,
				UpdatedDate
			FROM tblFuelCardLimit
			WHERE FuelCardLimitGuid = @FuelCardLimitGuid
		END
		ELSE 
		BEGIN
			-- Retrieve the Fuel Card Limit record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				FuelCardLimitGuid,
				ID,
				SiteGuid,
				CreatedBy,
				CreatedDate,
				UpdatedBy,
				UpdatedDate
			FROM tblFuelCardLimit
			WHERE EXISTS (SELECT * FROM map.tblEntityFuelCardLimitToSite 
				WHERE map.tblEntityFuelCardLimitToSite.SiteGuid = @SiteGuid AND map.tblEntityFuelCardLimitToSite.FuelCardLimitGuid = tblFuelCardLimit.FuelCardLimitGuid)
			AND ID = @ID
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
						+ 'Procedure Name: usp_FuelCardLimitGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	