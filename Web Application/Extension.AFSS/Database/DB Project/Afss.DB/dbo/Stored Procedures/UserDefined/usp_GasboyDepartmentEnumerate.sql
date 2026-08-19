CREATE PROCEDURE [dbo].[usp_GasboyDepartmentEnumerate]
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@DepartmentName NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @DepartmentNamePattern NVARCHAR(50);
		SET @DepartmentNamePattern = @DepartmentName

		IF (LEFT(@DepartmentNamePattern, 1) <> '%')
		BEGIN
			SET @DepartmentNamePattern = '%' + @DepartmentNamePattern;
		END

		IF (RIGHT(@DepartmentNamePattern, 1) <> '%')
		BEGIN
			SET @DepartmentNamePattern = @DepartmentNamePattern + '%';
		END

		-- Retrieve the Gasboy Department record(s) that contain the specified DepartmentName
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
				AND (@DepartmentNamePattern IS NULL OR (@DepartmentNamePattern IS NOT NULL AND [dbo].[tblGasboyDepartment].[DepartmentName] LIKE @DepartmentNamePattern))

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
						+ 'Procedure Name: usp_GasboyDepartmentEnumerate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	