
CREATE PROCEDURE [map].[usp_GetCompanyToCarrierCustomerShipToBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyToCarrierCustomerShipToBySite] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Company-To-UserGroup mappings for a given Site/Sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		SELECT mapAuthCar.*, 
		c1.ID AS AssignedToID, c1.Name AS AssignedToName, c1.Address1 AS AssignedToAddress, c1.City AS AssignedToCity, c1.State AS AssignedToState, 
		c2.ID AS AssignedID, c2.LockedOut AS LockedOut, c2.Name AS AssignedName, c2.Address1 AS AssignedAddress, c2.City AS AssignedCity, c2.State AS AssignedState 
		FROM map.tblCompanyAuthorizedCarrierToCompany mapAuthCar 
		INNER JOIN tblCompanies c1 
		ON c1.CompanyGuid = mapAuthCar.AssignedToCompanyGuid 
		INNER JOIN tblCompanies c2 
		ON c2.CompanyGuid = mapAuthCar.CompanyGuid 
		INNER JOIN erv.udf_GetCompanyRecordVersions(@TargetSiteGuid) d 
		ON d.CompanyGuid = mapAuthCar.AssignedToCompanyGuid	

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
						+ 'Procedure Name: [map].usp_GetCompanyToCarrierCustomerShipToBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     



