



CREATE PROCEDURE [map].[usp_GetProdToSupplierProdComMapBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProdToSupplierProdComMapBySite] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the map.tblProductToSupplierProductCompany mappings for a given Site/Sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode as PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, d.ID AS AdditiveProfileID, e.TankID AS TankID , 
		f.ID AS AssignedToID, f.Name AS AssignedToName, f.Address1 AS AssignedToAddress, f.City AS AssignedToCity, 
		f.State AS AssignedToState  
		FROM map.tblProductToSupplierProductCompany a
		INNER JOIN (SELECT * FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid)) b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c  ON c.ProductGuid = b.ProductGuid  
		LEFT OUTER JOIN tblAdditiveProfiles d ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT OUTER JOIN tblTanks e ON e.TankGuid = a.TankGuid  
		INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) g	ON g.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies f ON f.CompanyGuid = g.CompanyGuid		
		ORDER BY a.AssignedToCompanyGuid

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
						+ 'Procedure Name: [map].usp_GetProdToSupplierProdComMapBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END

