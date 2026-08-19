
/*

	EXEC [map].[usp_GetProdToSupplierProdComMapByGuid] '1376098C-B669-46FD-B083-7616D33CAEA6', '00000000-0000-0000-0000-000000000001'
	EXEC [map].[usp_GetProdToSupplierProdComMapByGuid] NULL, '00000000-0000-0000-0000-000000000001'


*/


CREATE PROCEDURE [map].[usp_GetProdToSupplierProdComMapByGuid]
(
	@ProductToSupplierProductCompanyGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProdToSupplierProdComMapByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the map.tblProductToSupplierProductCompany mappings for a given ProductToSupplierProductCompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @ProductToSupplierProductCompanyGuid: ProductToSupplierProductCompanyGuid for which to fetch the mapping record. If NULL then all records are returned.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the ProductMapClass.SelectSQL() inline SQL, for the case where Type = PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP.
	-- 4. This query is both Company and Product Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode as PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, d.ID AS AdditiveProfileID, e.TankID AS TankID , 
		f.ID AS AssignedToID, f.Name AS AssignedToName, f.Address1 AS AssignedToAddress, f.City AS AssignedToCity, 
		f.State AS AssignedToState  
		FROM map.tblProductToSupplierProductCompany a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c  ON c.ProductGuid = b.ProductGuid  
		LEFT OUTER JOIN tblAdditiveProfiles d ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT OUTER JOIN tblTanks e ON e.TankGuid = a.TankGuid  
		INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) g	ON g.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies f ON f.CompanyGuid = g.CompanyGuid		
		WHERE (a.ProductToSupplierProductCompanyGuid = @ProductToSupplierProductCompanyGuid OR @ProductToSupplierProductCompanyGuid IS NULL)


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
						+ 'Procedure Name: [map].usp_GetProdToSupplierProdComMapByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END