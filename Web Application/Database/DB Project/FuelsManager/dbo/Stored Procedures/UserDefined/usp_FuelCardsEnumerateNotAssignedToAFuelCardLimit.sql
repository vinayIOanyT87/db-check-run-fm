CREATE PROCEDURE [dbo].[usp_FuelCardsEnumerateNotAssignedToAFuelCardLimit]
	@SiteGuid UNIQUEIDENTIFIER,
	@FuelCardLimitGuid UNIQUEIDENTIFIER,
	@ID NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- Retrieve fuel cards that aren't mapped to a fuel card limit, unless they are mapped to the Fuel Card Limit matching the provided @FuelCardLimitGuid
		-- We retrieve fuel cards mapped to the provided limit so that we can possibly display them on the fuel card assignment screen if they are unassigned and then the user wants to assign them again
		IF (@ID IS NOT NULL)
		BEGIN
			-- If the ID is provided, limit the results to those which contain the provided value.
			SELECT 
				tblFuelCards.FuelCardGuid,
				tblFuelCards.ID,
				tblFuelCards.SiteGuid,
				managerCompanies.ID AS ManagerID,
				billToCompanies.ID AS BillToID
			FROM tblFuelCards
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) managerCompanies
			ON managerCompanies._MasterRecordGuid = tblFuelCards.ManagerCompanyGuid	
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) billToCompanies
			ON billToCompanies._MasterRecordGuid = tblFuelCards.BillToCompanyGuid	
			WHERE EXISTS (SELECT * FROM map.tblEntityFuelCardToSite 
				WHERE map.tblEntityFuelCardToSite.SiteGuid = @SiteGuid
					AND map.tblEntityFuelCardToSite.FuelCardGuid = tblFuelCards.FuelCardGuid)
			AND NOT EXISTS (SELECT * FROM map.tblFuelCardLimitToFuelCard WHERE map.tblFuelCardLimitToFuelCard.FuelCardGuid = tblFuelCards.FuelCardGuid 
				AND (@FuelCardLimitGuid IS NULL OR map.tblFuelCardLimitToFuelCard.FuelCardLimitGuid <> @FuelCardLimitGuid))
			AND (tblFuelCards.ID LIKE ('%' + @ID + '%') OR managerCompanies.ID LIKE ('%' + @ID + '%') OR billToCompanies.ID LIKE ('%' + @ID + '%'))
			ORDER BY tblFuelCards.ID
		END
		ELSE
		BEGIN
			SELECT 
				tblFuelCards.FuelCardGuid,
				tblFuelCards.ID,
				tblFuelCards.SiteGuid,
				managerCompanies.ID AS ManagerID,
				billToCompanies.ID AS BillToID
			FROM tblFuelCards 
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) managerCompanies
			ON managerCompanies._MasterRecordGuid = tblFuelCards.ManagerCompanyGuid	
			LEFT JOIN (SELECT tblCompanies.ID, tblCompanies._MasterRecordGuid FROM tblCompanies 
				INNER JOIN erv.udf_GetCompanyRecordVersions(@SiteGuid) companyRecordVersions ON tblCompanies.CompanyGuid = companyRecordVersions.CompanyGuid) billToCompanies
			ON billToCompanies._MasterRecordGuid = tblFuelCards.BillToCompanyGuid	
			WHERE EXISTS (SELECT * FROM map.tblEntityFuelCardToSite 
				WHERE map.tblEntityFuelCardToSite.SiteGuid = @SiteGuid
					AND map.tblEntityFuelCardToSite.FuelCardGuid = tblFuelCards.FuelCardGuid)
			AND NOT EXISTS (SELECT * FROM map.tblFuelCardLimitToFuelCard WHERE map.tblFuelCardLimitToFuelCard.FuelCardGuid = tblFuelCards.FuelCardGuid 
				AND (@FuelCardLimitGuid IS NULL OR map.tblFuelCardLimitToFuelCard.FuelCardLimitGuid <> @FuelCardLimitGuid))
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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)                 
						+ 'Procedure Name: usp_FuelCardsEnumerateNotAssignedToAFuelCardLimit' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	