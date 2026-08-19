CREATE PROCEDURE [dbo].[usp_GasboyDepartmentGet]
	@IdentityGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@DepartmentID BIGINT = NULL,
	@DepartmentName NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@IdentityGuid IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record by its primary key
			SELECT 
				[dbo].[tblGasboyDepartment].[GasboyDepartmentGuid],
				[dbo].[tblGasboyDepartment].[SiteGuid],
				[dbo].[tblGasboyDepartment].[DepartmentID],
				[dbo].[tblGasboyDepartment].[DepartmentCode],
				[dbo].[tblGasboyDepartment].[DepartmentName],
				[dbo].[tblGasboyDepartment].[GroupRuleName],
				[dbo].[tblGasboyDepartment].[PriceListName],
				[dbo].[tblGasboyDepartment].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyDepartment].[UsePINCodeFlag],
				[dbo].[tblGasboyDepartment].[PINCode],
				[dbo].[tblGasboyDepartment].[AuthPINFrom],
				[dbo].[tblGasboyDepartment].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyDepartment].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyDepartment].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyDepartment].[CreatedBy],
				[dbo].[tblGasboyDepartment].[CreatedDate],
				[dbo].[tblGasboyDepartment].[UpdatedBy],
				[dbo].[tblGasboyDepartment].[UpdatedDate]
			FROM [dbo].[tblGasboyDepartment] 
			WHERE [dbo].[tblGasboyDepartment].GasboyDepartmentGuid = @IdentityGuid
		END
		ELSE IF (@DepartmentID IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblGasboyDepartment].[GasboyDepartmentGuid],
				[dbo].[tblGasboyDepartment].[SiteGuid],
				[dbo].[tblGasboyDepartment].[DepartmentID],
				[dbo].[tblGasboyDepartment].[DepartmentCode],
				[dbo].[tblGasboyDepartment].[DepartmentName],
				[dbo].[tblGasboyDepartment].[GroupRuleName],
				[dbo].[tblGasboyDepartment].[PriceListName],
				[dbo].[tblGasboyDepartment].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyDepartment].[UsePINCodeFlag],
				[dbo].[tblGasboyDepartment].[PINCode],
				[dbo].[tblGasboyDepartment].[AuthPINFrom],
				[dbo].[tblGasboyDepartment].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyDepartment].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyDepartment].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyDepartment].[CreatedBy],
				[dbo].[tblGasboyDepartment].[CreatedDate],
				[dbo].[tblGasboyDepartment].[UpdatedBy],
				[dbo].[tblGasboyDepartment].[UpdatedDate]
			FROM [dbo].[tblGasboyDepartment] 
				INNER JOIN [map].[tblEntityGasboyDepartmentToSite]
						ON [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentGuid] = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
			WHERE [map].[tblEntityGasboyDepartmentToSite].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblGasboyDepartment].[DepartmentID] = @DepartmentID
		END
		ELSE 
		BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblGasboyDepartment].[GasboyDepartmentGuid],
				[dbo].[tblGasboyDepartment].[SiteGuid],
				[dbo].[tblGasboyDepartment].[DepartmentID],
				[dbo].[tblGasboyDepartment].[DepartmentCode],
				[dbo].[tblGasboyDepartment].[DepartmentName],
				[dbo].[tblGasboyDepartment].[GroupRuleName],
				[dbo].[tblGasboyDepartment].[PriceListName],
				[dbo].[tblGasboyDepartment].[LookupGasboyRecordStatusIndex],
				[dbo].[tblGasboyDepartment].[UsePINCodeFlag],
				[dbo].[tblGasboyDepartment].[PINCode],
				[dbo].[tblGasboyDepartment].[AuthPINFrom],
				[dbo].[tblGasboyDepartment].[PromptForVehiclePlateFlag],
				[dbo].[tblGasboyDepartment].[LookupGasboyVehiclePlateCheckTypeIndex],
				[dbo].[tblGasboyDepartment].[AlwaysPromptForAdditionalValidationFlag],
				[dbo].[tblGasboyDepartment].[CreatedBy],
				[dbo].[tblGasboyDepartment].[CreatedDate],
				[dbo].[tblGasboyDepartment].[UpdatedBy],
				[dbo].[tblGasboyDepartment].[UpdatedDate]
			FROM [dbo].[tblGasboyDepartment] 
				INNER JOIN [map].[tblEntityGasboyDepartmentToSite]
						ON [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentGuid] = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
			WHERE [map].[tblEntityGasboyDepartmentToSite].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblGasboyDepartment].[DepartmentName] = @DepartmentName
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
						+ 'Procedure Name: usp_GasboyDepartmentGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	