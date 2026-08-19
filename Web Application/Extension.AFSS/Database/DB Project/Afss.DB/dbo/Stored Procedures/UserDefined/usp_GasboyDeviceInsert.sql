CREATE PROCEDURE [dbo].[usp_GasboyDeviceInsert]
	@GasboyDeviceGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@GasboyDepartmentGuid UNIQUEIDENTIFIER = NULL,
	@DeviceCode BIGINT = NULL,
	@DeviceName NVARCHAR(50) = NULL,
	@CardNumber NVARCHAR(50) = NULL,
	@GroupRuleName NVARCHAR(50) = NULL,
	@LookupGasboyDeviceTypeIndex INT = NULL,
	@LookupGasboyRecordStatusIndex INT = NULL,
	@LookupGasboyHardwareTypeIndex INT = NULL,
	@LookupGasboyAuthorizationTypeIndex  INT = NULL,
	@LookupGasboyEmployeeTypeIndex INT = NULL,
	@LookupGasboyTwoStageDriverValidationTypeIndex INT = NULL,
	@UsePINCodeFlag BIT = 0,
	@PINCode VARBINARY(256) = NULL,
	@AuthPINFrom TINYINT = 2,
	@VehiclePlate NVARCHAR(50) = NULL,
	@PromptForVehiclePlateFlag BIT = NULL,
	@LookupGasboyVehiclePlateCheckTypeIndex INT = NULL,
	@AlwaysPromptForAdditionalValidationFlag TINYINT = NULL,
	@CreatedUpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		DECLARE @GeneratedDeviceID BIGINT
		
		--Only code ground cards eith device ID greater than or equal to 900000001
		--Ground cards start with 69	
		if(SUBSTRING(@CardNumber	,1,2) = '69')
			SET @GeneratedDeviceID = ISNULL(( SELECT MAX(DeviceCode)+1 FROM dbo.tblGasboyDevice where DeviceID between 900000001 and 999999999),  900000001);
		else
			SET @GeneratedDeviceID = ISNULL(( SELECT MAX(DeviceCode)+1 FROM dbo.tblGasboyDevice where DeviceID between 700000001 and 799999999),  700000001);

		SET @DeviceCode = @GeneratedDeviceID;

		INSERT INTO tblGasboyDevice
		(
			GasboyDeviceGuid,
			SiteGuid,
			GasboyDepartmentGuid,
			DeviceCode,
			DeviceName,
			CardNumber,
			GroupRuleName,
			LookupGasboyDeviceTypeIndex,
			LookupGasboyRecordStatusIndex,
			LookupGasboyHardwareTypeIndex,
			LookupGasboyAuthTypeIndex,
			LookupGasboyEmployeeTypeIndex,
			LookupGasboyTwoStageDriverValidationTypeIndex,
			UsePINCodeFlag,
			PINCode,
			AuthPINFrom,
			VehiclePlate,
			PromptForVehiclePlateFlag,
			LookupGasboyVehiclePlateCheckTypeIndex,
			AlwaysPromptForAdditionalValidationFlag,
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate,
			DeviceID
		)
		VALUES
		(
			@GasboyDeviceGuid,
			@SiteGuid,
			@GasboyDepartmentGuid,
			@DeviceCode,
			@DeviceName,
			@CardNumber,
			@GroupRuleName,
			@LookupGasboyDeviceTypeIndex,
			@LookupGasboyRecordStatusIndex,
			@LookupGasboyHardwareTypeIndex,
			@LookupGasboyAuthorizationTypeIndex,
			@LookupGasboyEmployeeTypeIndex,
			@LookupGasboyTwoStageDriverValidationTypeIndex,
			@UsePINCodeFlag,
			@PINCode,
			@AuthPINFrom,
			@VehiclePlate,
			@PromptForVehiclePlateFlag,
			@LookupGasboyVehiclePlateCheckTypeIndex,
			@AlwaysPromptForAdditionalValidationFlag,
			@CreatedUpdatedBy,
			SYSDATETIMEOFFSET(),
			@CreatedUpdatedBy,
			SYSDATETIMEOFFSET(),
			@GeneratedDeviceID
		)

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
						+ 'Procedure Name: usp_GasboyDeviceInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
