CREATE PROCEDURE [dbo].[usp_GasboyDepartmentInsert]
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
	@CreatedUpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		INSERT INTO tblGasboyDepartment
		(
			GasboyDepartmentGuid,
			SiteGuid,
			DepartmentID,
			DepartmentCode,
			DepartmentName,
			GroupRuleName,
			PriceListName,
			LookupGasboyRecordStatusIndex,
			UsePINCodeFlag,
			PINCode,
			AuthPINFrom,
			PromptForVehiclePlateFlag,
			LookupGasboyVehiclePlateCheckTypeIndex,
			AlwaysPromptForAdditionalValidationFlag,
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		VALUES
		(
			@IdentityGuid, 
			@SiteGuid,
			@DepartmentID,
			@DepartmentCode, 
			@DepartmentName,
			@GroupRuleName, 
			@PriceListName, 
			@LookupGasboyRecordStatusIndex, 
			@UsePINCodeFlag, 
			@PINCode,
			@AuthPINFrom,
			@PromptForVehiclePlateFlag,
			@LookupGasboyVehiclePlateCheckTypeIndex,
			@AlwaysPromptForAdditionalValidationFlag,
			@CreatedUpdatedBy,
			SYSDATETIMEOFFSET(),
			@CreatedUpdatedBy,
			SYSDATETIMEOFFSET()
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
						+ 'Procedure Name: usp_GasboyDepartmentInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
