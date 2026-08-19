CREATE PROCEDURE [dbo].[usp_EnumerateFuelCardLimitMappingsByFuelCardGuid]
	@FuelCardGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		SELECT fcl.*
		FROM dbo.tblFuelCardLimit fcl LEFT OUTER JOIN
			 map.tblFuelCardLimitToFuelCard fclmap ON fcl.FuelCardLimitGuid = fclmap.FuelCardLimitGuid
		WHERE fclmap.FuelCardGuid = @FuelCardGuid
			  AND fcl.FuelCardLimitGuid IN (SELECT FuelCardLimitGuid 
											FROM map.tblEntityFuelCardLimitToSite
											WHERE map.tblEntityFuelCardLimitToSite.SiteGuid = @SiteGuid)
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
						+ 'Procedure Name: usp_EnumerateFuelCardLimitMappingsByFuelCardGuid' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,16,1); 
	END CATCH

END
