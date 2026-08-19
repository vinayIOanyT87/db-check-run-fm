
/*

	EXEC [map].[usp_GetProductToCompanyGroupMapByProdGuid] '0eac881d-8e71-4ed2-bfd2-ea1084019f3a', 'aeba18e3-e97b-479e-8b2d-0bcd69c1c421'
	EXEC [map].[usp_GetProductToCompanyGroupMapByProdGuid] '0eac881d-8e71-4ed2-bfd2-ea1084019f3a', 'f4761a16-ab2f-41ee-b6fa-d17658df2602'


*/

CREATE PROCEDURE [map].[usp_GetProductToCompanyGroupMapByProdGuid]
(
	@ProductGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProductToCompanyGroupMapByProdGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Product-To-CompanyGroup mappings for a given ProductGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @ProductGuid: ProductGuid for which to fetch the mapping records.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the ProductMapClass.EnumerateByAssignedGuidAndTypeSQL() inline SQL, for the case where Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP.
	-- 4. This query is Product Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY			
		
		SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode AS PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, e.ID AS AdditiveProfileID,  
		f.TankID AS TankID , g.ID AS AssignedToID  
		FROM map.tblProductToCompanyGroup a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c ON c.ProductGuid = a.ProductGuid  
		INNER JOIN map.tblEntityCompanyGroupToSite d ON d.ApplicationStringGuid = a.AssignedToApplicationStringGuid  
		LEFT JOIN tblAdditiveProfiles e  ON e.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT JOIN tblTanks f ON f.TankGuid = a.TankGuid  
		LEFT JOIN tblApplicationString g ON g.ApplicationStringGuid = a.AssignedToApplicationStringGuid 
		WHERE a.ProductGuid = @ProductGuid
		AND d.SiteGuid = @TargetSiteGuid  --Restrict mappings returned/displayed for a Product to only those CompanyGroups that have been assigned to the target site (just like with the Product-to-Company mappings).
		ORDER BY AssignedID

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
						+ 'Procedure Name: [map].usp_GetProductToCompanyGroupMapByProdGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     


