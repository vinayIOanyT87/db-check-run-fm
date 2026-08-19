CREATE PROCEDURE [dbo].[usp_GasboyFleetGet]
	@IdentityGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@FleetID BIGINT = NULL,
	@FleetName NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@IdentityGuid IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record by its primary key
			SELECT 
				[dbo].[tblGasboyFleet].[GasboyFleetGuid],
				[dbo].[tblGasboyFleet].[SiteGuid],
				[dbo].[tblGasboyFleet].[FleetID],
				[dbo].[tblGasboyFleet].[FleetCode],
				[dbo].[tblGasboyFleet].[FleetName],
				[dbo].[tblGasboyFleet].[GroupRuleName],
				[dbo].[tblGasboyFleet].[PriceListName],
				[dbo].[tblGasboyFleet].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyFleet].[UsePINCodeFlag],
				[dbo].[tblGasboyFleet].[PINCode],
				[dbo].[tblGasboyFleet].[AuthPINFrom],
				[dbo].[tblGasboyFleet].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyFleet].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyFleet].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyFleet].[CreatedBy],
				[dbo].[tblGasboyFleet].[CreatedDate],
				[dbo].[tblGasboyFleet].[UpdatedBy],
				[dbo].[tblGasboyFleet].[UpdatedDate]
			FROM [dbo].[tblGasboyFleet] 
			WHERE [dbo].[tblGasboyFleet].GasboyFleetGuid = @IdentityGuid
		END
		ELSE IF (@FleetID IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblGasboyFleet].[GasboyFleetGuid],
				[dbo].[tblGasboyFleet].[SiteGuid],
				[dbo].[tblGasboyFleet].[FleetID],
				[dbo].[tblGasboyFleet].[FleetCode],
				[dbo].[tblGasboyFleet].[FleetName],
				[dbo].[tblGasboyFleet].[GroupRuleName],
				[dbo].[tblGasboyFleet].[PriceListName],
				[dbo].[tblGasboyFleet].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyFleet].[UsePINCodeFlag],
				[dbo].[tblGasboyFleet].[PINCode],
				[dbo].[tblGasboyFleet].[AuthPINFrom],
				[dbo].[tblGasboyFleet].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyFleet].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyFleet].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyFleet].[CreatedBy],
				[dbo].[tblGasboyFleet].[CreatedDate],
				[dbo].[tblGasboyFleet].[UpdatedBy],
				[dbo].[tblGasboyFleet].[UpdatedDate]
			FROM [dbo].[tblGasboyFleet] 
				INNER JOIN [map].[tblEntityGasboyFleetToSite]
						ON [map].[tblEntityGasboyFleetToSite].[GasboyFleetGuid] = [dbo].[tblGasboyFleet].[GasboyFleetGuid]
			WHERE [map].[tblEntityGasboyFleetToSite].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblGasboyFleet].[FleetID] = @FleetID
		END
		ELSE
		BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblGasboyFleet].[GasboyFleetGuid],
				[dbo].[tblGasboyFleet].[SiteGuid],
				[dbo].[tblGasboyFleet].[FleetID],
				[dbo].[tblGasboyFleet].[FleetCode],
				[dbo].[tblGasboyFleet].[FleetName],
				[dbo].[tblGasboyFleet].[GroupRuleName],
				[dbo].[tblGasboyFleet].[PriceListName],
				[dbo].[tblGasboyFleet].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyFleet].[UsePINCodeFlag],
				[dbo].[tblGasboyFleet].[PINCode],
				[dbo].[tblGasboyFleet].[AuthPINFrom],
				[dbo].[tblGasboyFleet].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyFleet].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyFleet].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyFleet].[CreatedBy],
				[dbo].[tblGasboyFleet].[CreatedDate],
				[dbo].[tblGasboyFleet].[UpdatedBy],
				[dbo].[tblGasboyFleet].[UpdatedDate]
			FROM [dbo].[tblGasboyFleet] 
				INNER JOIN [map].[tblEntityGasboyFleetToSite]
						ON [map].[tblEntityGasboyFleetToSite].[GasboyFleetGuid] = [dbo].[tblGasboyFleet].[GasboyFleetGuid]
			WHERE [map].[tblEntityGasboyFleetToSite].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblGasboyFleet].[FleetName] = @FleetName
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
						+ 'Procedure Name: usp_GasboyFleetGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	