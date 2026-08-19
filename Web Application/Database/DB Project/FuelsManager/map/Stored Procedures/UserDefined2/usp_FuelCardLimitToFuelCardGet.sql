CREATE PROCEDURE [map].[usp_FuelCardLimitToFuelCardGet]
	@FuelCardLimitGuid UNIQUEIDENTIFIER,
	@FuelCardGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY		
		IF (@FuelCardGuid IS NOT NULL)
		BEGIN
			-- Retrieve fuel card -> fuel card limit mappings by the FuelCardLimitGuid and FuelCardGuid, which should identify only one record
			SELECT 
				FuelCardLimitToFuelCardGuid,
				FuelCardLimitGuid,
				FuelCardGuid
			FROM map.tblFuelCardLimitToFuelCard 
			WHERE FuelCardLimitGuid = @FuelCardLimitGuid 
				AND FuelCardGuid = @FuelCardGuid
		END
		ELSE
		BEGIN
			-- Retrieve all fuel card -> fuel card limit mappings for a particular Fuel Card Limit
			SELECT 
				map.tblFuelCardLimitToFuelCard.FuelCardLimitToFuelCardGuid,
				map.tblFuelCardLimitToFuelCard.FuelCardLimitGuid,
				map.tblFuelCardLimitToFuelCard.FuelCardGuid,
				tblFuelCards.ID,
				managerCompanies.ID AS ManagerID,
				billToCompanies.ID AS BillToID
			FROM map.tblFuelCardLimitToFuelCard 
			INNER JOIN tblFuelCards ON map.tblFuelCardLimitToFuelCard.FuelCardGuid = tblFuelCards.FuelCardGuid 
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) managerCompanies
			ON managerCompanies._MasterRecordGuid = tblFuelCards.ManagerCompanyGuid
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) billToCompanies
			ON billToCompanies._MasterRecordGuid = tblFuelCards.ManagerCompanyGuid
			WHERE FuelCardLimitGuid = @FuelCardLimitGuid
			AND EXISTS (SELECT * FROM map.tblEntityFuelCardToSite WHERE map.tblEntityFuelCardToSite.FuelCardGuid = tblFuelCards.FuelCardGuid AND map.tblEntityFuelCardToSite.SiteGuid = @SiteGuid)
			ORDER BY tblFuelCards.ID
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
						+ 'Procedure Name: map.usp_FuelCardLimitToFuelCardGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	