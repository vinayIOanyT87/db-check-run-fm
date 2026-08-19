CREATE PROCEDURE [dbo].[usp_GasboyDeviceEnumerateByCardNumber]
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@CardNumber NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		IF (ISNULL(@CardNumber, '') = '')
		BEGIN
			RAISERROR('Invalid parameter: CardNumber cannot be NULL or empty', 18, 0)
			RETURN
		END

		DECLARE @CardNumberPattern NVARCHAR(50);
		SET @CardNumberPattern = @CardNumber

		IF (LEFT(@CardNumberPattern, 1) <> '%')
		BEGIN
			SET @CardNumberPattern = '%' + @CardNumberPattern;
		END

		IF (RIGHT(@CardNumberPattern, 1) <> '%')
		BEGIN
			SET @CardNumberPattern = @CardNumberPattern + '%';
		END

		-- Retrieve the Gasboy Device record(s) that contain the specified Card Number
		-- that is owned by the site provided.
		SELECT 
				[dbo].[tblGasboyDevice].[GasboyDeviceGuid],
				[dbo].[tblGasboyDevice].[SiteGuid],
				--[dbo].[tblGasboyFleet].[FleetID], --Fleets and departments are standard across FMD and are defined in GasboySpecialConstants
				--[dbo].[tblGasboyFleet].[FleetCode],
				--[dbo].[tblGasboyDepartment].[DepartmentID],
				--[dbo].[tblGasboyDepartment].[DepartmentCode],
				[dbo].[tblGasboyDevice].[GasboyDepartmentGuid],
				[dbo].[tblGasboyDevice].[DeviceID],
				[dbo].[tblGasboyDevice].[DeviceCode],
				[dbo].[tblGasboyDevice].[DeviceName],
				[dbo].[tblGasboyDevice].[CardNumber],
				[dbo].[tblGasboyDevice].[GroupRuleName],
				[dbo].[tblGasboyDevice].[LookupGasboyDeviceTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyHardwareTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyAuthTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyEmployeeTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyTwoStageDriverValidationTypeIndex],
				[dbo].[tblGasboyDevice].[UsePINCodeFlag],
				[dbo].[tblGasboyDevice].[PINCode],
				[dbo].[tblGasboyDevice].[AuthPINFrom],
				[dbo].[tblGasboyDevice].[VehiclePlate],
				[dbo].[tblGasboyDevice].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyDevice].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyDevice].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyDevice].[CreatedBy],
				[dbo].[tblGasboyDevice].[CreatedDate],
				[dbo].[tblGasboyDevice].[UpdatedBy],
				[dbo].[tblGasboyDevice].[UpdatedDate]
		FROM [dbo].[tblGasboyDevice] 
			--INNER JOIN [dbo].[tblGasboyDepartment]
			--	ON [dbo].[tblGasboyDevice].[GasboyDepartmentGuid] = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
			--INNER JOIN [map].[tblGasboyDepartmentToGasboyFleet]
			--	ON [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid] = [map].[tblGasboyDepartmentToGasboyFleet].[GasboyDepartmentGuid]
			--INNER JOIN [dbo].[tblGasboyFleet]
			--	ON [dbo].[tblGasboyFleet].[GasboyFleetGuid] = [map].[tblGasboyDepartmentToGasboyFleet].[GasboyFleetGuid]
		WHERE [dbo].[tblGasboyDevice].[SiteGuid] = @SiteGuid 
				AND [dbo].[tblGasboyDevice].[CardNumber] LIKE @CardNumberPattern
		-- Retrieve the Gasboy Device record(s) that contain the specified Card Number
		-- that is assigned to the site provided.
		UNION
		SELECT 
				[dbo].[tblGasboyDevice].[GasboyDeviceGuid],
				[dbo].[tblGasboyDevice].[SiteGuid],
				--[dbo].[tblGasboyFleet].[FleetID],
				--[dbo].[tblGasboyFleet].[FleetCode],
				--[dbo].[tblGasboyDepartment].[DepartmentID],
				--[dbo].[tblGasboyDepartment].[DepartmentCode],
				[dbo].[tblGasboyDevice].[GasboyDepartmentGuid],
				[dbo].[tblGasboyDevice].[DeviceID],
				[dbo].[tblGasboyDevice].[DeviceCode],
				[dbo].[tblGasboyDevice].[DeviceName],
				[dbo].[tblGasboyDevice].[CardNumber],
				[dbo].[tblGasboyDevice].[GroupRuleName],
				[dbo].[tblGasboyDevice].[LookupGasboyDeviceTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyHardwareTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyAuthTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyEmployeeTypeIndex],
				[dbo].[tblGasboyDevice].[LookupGasboyTwoStageDriverValidationTypeIndex],
				[dbo].[tblGasboyDevice].[UsePINCodeFlag],
				[dbo].[tblGasboyDevice].[PINCode],
				[dbo].[tblGasboyDevice].[AuthPINFrom],
				[dbo].[tblGasboyDevice].[VehiclePlate],
				[dbo].[tblGasboyDevice].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyDevice].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyDevice].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyDevice].[CreatedBy],
				[dbo].[tblGasboyDevice].[CreatedDate],
				[dbo].[tblGasboyDevice].[UpdatedBy],
				[dbo].[tblGasboyDevice].[UpdatedDate]
		FROM [dbo].[tblGasboyDevice] 
			INNER JOIN [map].[tblEntityGasboyDeviceToSite]
					ON [map].[tblEntityGasboyDeviceToSite].[OwnerSiteGuid] = [dbo].[tblGasboyDevice].[SiteGuid]
			--INNER JOIN [dbo].[tblGasboyDepartment]
			--	ON [dbo].[tblGasboyDevice].[GasboyDepartmentGuid] = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
			--INNER JOIN [map].[tblGasboyDepartmentToGasboyFleet]
			--	ON [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid] = [map].[tblGasboyDepartmentToGasboyFleet].[GasboyDepartmentGuid]
			--INNER JOIN [dbo].[tblGasboyFleet]
			--	ON [dbo].[tblGasboyFleet].[GasboyFleetGuid] = [map].[tblGasboyDepartmentToGasboyFleet].[GasboyFleetGuid]
		WHERE [map].[tblEntityGasboyDeviceToSite].[MapToSiteGuid] = @SiteGuid 
				AND [dbo].[tblGasboyDevice].[CardNumber] LIKE @CardNumberPattern

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
						+ 'Procedure Name: usp_GasboyDeviceEnumerateByCardNumber' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	