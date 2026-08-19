CREATE PROCEDURE [dbo].[usp_GasboyDepartmentUpdate]
	@IdentityGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER, 
	@DepartmentID BIGINT,
	@DepartmentCode BIGINT, 
	@DepartmentName NVARCHAR(50) = NULL,
	@GroupRuleName NVARCHAR(50) = NULL, 
	@PriceListName NVARCHAR(50) = NULL, 
	@LookupGasboyRecordStatusIndex INT = 2, 
	@UsePINCodeFlag BIT = 0, 
	@PINCode VARBINARY(256) = NULL,
	@AuthPINFrom TINYINT = NULL,
	@PromptForVehiclePlateFlag BIT = 0,
	@LookupGasboyVehiclePlateCheckTypeIndex INT = NULL,
	@AlwaysPromptForAdditionalValidationFlag BIT = 0,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @UpdatedDate DATETIMEOFFSET(7);
		SET @UpdatedDate = SYSDATETIMEOFFSET();

		UPDATE [dbo].[tblGasboyDepartment]
			SET SiteGuid = @SiteGuid,
				DepartmentID = @DepartmentID,
				DepartmentCode = @DepartmentCode,
				DepartmentName = @DepartmentName,
				GroupRuleName = @GroupRuleName,
				PriceListName = @PriceListName,
				LookupGasboyRecordStatusIndex = @LookupGasboyRecordStatusIndex,
				UsePINCodeFlag = @UsePINCodeFlag,
				PINCode = @PINCode,
				AuthPINFrom = @AuthPINFrom,
				PromptForVehiclePlateFlag = @PromptForVehiclePlateFlag,
				LookupGasboyVehiclePlateCheckTypeIndex = @LookupGasboyVehiclePlateCheckTypeIndex,
				AlwaysPromptForAdditionalValidationFlag = @AlwaysPromptForAdditionalValidationFlag,
				UpdatedBy = @UpdatedBy,
				UpdatedDate = @UpdatedDate
		WHERE GasboyDepartmentGuid = @IdentityGuid

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
						+ 'Procedure Name: usp_GasboyDepartmentUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
