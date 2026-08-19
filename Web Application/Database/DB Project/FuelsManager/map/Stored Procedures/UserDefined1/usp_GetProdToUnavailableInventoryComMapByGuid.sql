
/*
	DROP PROCEDURE [map].[usp_GetProdToUnavailableInventoryComMapByGuid]

	EXEC [map].[usp_GetProdToUnavailableInventoryComMapByGuid] '38B24944-C577-4193-B9FC-554205CB39D7', '00000000-0000-0000-0000-000000000001'
	EXEC [map].[usp_GetProdToUnavailableInventoryComMapByGuid] NULL, '00000000-0000-0000-0000-000000000001'


*/


CREATE PROCEDURE [map].[usp_GetProdToUnavailableInventoryComMapByGuid]
(
	@ProductToUnavailableInventoryCompanyGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProdToUnavailableInventoryComMapByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the map.tblProductToUnavailableInventoryCompany mappings for a given ProductToUnavailableInventoryCompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @ProductToUnavailableInventoryCompanyGuid: ProductToUnavailableInventoryCompanyGuid for which to fetch the mapping record. If NULL then all records are returned.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the ProductMapClass.SelectSQL() inline SQL, for the case where Type = PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP.
	-- 4. This query is both Company and Product Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT a.*, c._MasterRecordGuid ProductMasterRecordGuid, c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode as PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, d.ID AS AdditiveProfileID, e.TankID AS TankID , 
		f.ID AS AssignedToID  
		FROM map.tblProductToUnavailableInventoryCompany a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c  ON c.ProductGuid = b.ProductGuid  
		LEFT OUTER JOIN tblAdditiveProfiles d ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT OUTER JOIN tblTanks e ON e.TankGuid = a.TankGuid  
		INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) g	ON g.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies f ON f.CompanyGuid = g.CompanyGuid		
		WHERE (a.ProductToUnavailableInventoryCompanyGuid = @ProductToUnavailableInventoryCompanyGuid OR @ProductToUnavailableInventoryCompanyGuid IS NULL)


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
						+ 'Procedure Name: [map].usp_GetProdToUnavailableInventoryComMapByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END