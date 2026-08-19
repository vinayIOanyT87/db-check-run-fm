CREATE PROCEDURE [dbo].[usp_GasboyDeviceUpdate]
	@GasboyDeviceGuid  UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@GasboyDepartmentGuid UNIQUEIDENTIFIER = NULL,
	@DeviceCode BIGINT = NULL,
	@DeviceName NVARCHAR(50) = NULL,
	@CardNumber NVARCHAR(50) = NULL,
	@GroupRuleName NVARCHAR(50) = NULL,
	@LookupGasboyDeviceTypeIndex INT = NULL,
	@LookupGasboyRecordStatusIndex INT = NULL,
	@LookupGasboyHardwareTypeIndex INT = NULL,
	@LookupGasboyAuthorizationTypeIndex INT = NULL,
	@LookupGasboyEmployeeTypeIndex INT = NULL,
	@LookupGasboyTwoStageDriverValidationTypeIndex INT = NULL,
	@UsePINCodeFlag BIT = 0,
	@PINCode VARBINARY(256) = NULL,
	@AuthPINFrom TINYINT = 2,
	@VehiclePlate NVARCHAR(50) = NULL,
	@PromptForVehiclePlateFlag BIT = NULL,
	@LookupGasboyVehiclePlateCheckTypeIndex INT = NULL,
	@AlwaysPromptForAdditionalValidationFlag TINYINT = NULL,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @UpdatedDate DATETIMEOFFSET(7);
		SET @UpdatedDate = SYSDATETIMEOFFSET();

		UPDATE [dbo].[tblGasboyDevice]
			SET	SiteGuid = @SiteGuid,
				GasboyDepartmentGuid = @GasboyDepartmentGuid,
				DeviceCode = @DeviceCode,
				DeviceName = @DeviceName,
				CardNumber = @CardNumber,
				GroupRuleName = @GroupRuleName,
				LookupGasboyDeviceTypeIndex = @LookupGasboyDeviceTypeIndex,
				LookupGasboyRecordStatusIndex = @LookupGasboyRecordStatusIndex,
				LookupGasboyHardwareTypeIndex = @LookupGasboyHardwareTypeIndex,
				LookupGasboyAuthTypeIndex = @LookupGasboyAuthorizationTypeIndex,
				LookupGasboyEmployeeTypeIndex = @LookupGasboyEmployeeTypeIndex,
				LookupGasboyTwoStageDriverValidationTypeIndex = @LookupGasboyTwoStageDriverValidationTypeIndex,
				UsePINCodeFlag = @UsePINCodeFlag,
				--PINCode = @PINCode,
				AuthPINFrom = @AuthPINFrom,
				VehiclePlate = @VehiclePlate,
				PromptForVehiclePlateFlag = @PromptForVehiclePlateFlag,
				LookupGasboyVehiclePlateCheckTypeIndex = @LookupGasboyVehiclePlateCheckTypeIndex,
				AlwaysPromptForAdditionalValidationFlag = @AlwaysPromptForAdditionalValidationFlag,
				UpdatedBy = @UpdatedBy,
				UpdatedDate = @UpdatedDate
		WHERE GasboyDeviceGuid = @GasboyDeviceGuid 

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
						+ 'Procedure Name: usp_GasboyDeviceUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END