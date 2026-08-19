
/*

	EXEC [map].[usp_GetProductToCompanyMapByProdGuid] '38B24944-C577-4193-B9FC-554205CB39D7', '00000000-0000-0000-0000-000000000001'
	EXEC [map].[usp_GetProductToCompanyMapByProdGuid] '80b08634-d356-4569-b9a2-cd36df955bd0', 'f4761a16-ab2f-41ee-b6fa-d17658df2602'


*/

CREATE PROCEDURE [map].[usp_GetProductToCompanyMapByProdGuid]
(
	@ProductGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProductToCompanyMapByProdGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Product-To-Company mappings for a given ProductGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @ProductGuid: ProductGuid for which to fetch the mapping records.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the ProductMapClass.EnumerateByAssignedGuidAndTypeSQL() inline SQL, for the case where Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP.
	-- 4. This query is both Company and Product Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		DECLARE @isProductRecVerOn bit
		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @assignedFrom uniqueidentifier
		DECLARE @firstAvailableOwnerSite uniqueidentifier
		
		SELECT @masterRecGuid = a._MasterRecordGuid, @assignedFrom = b.AssignedFromSiteGuid
		FROM tblProducts a 
		INNER JOIN map.tblEntityProductToSite b ON b.ProductGuid = a._MasterRecordGuid
		WHERE a.ProductGuid = @ProductGuid AND b.SiteGuid = @TargetSiteGuid

		SELECT @firstAvailableOwnerSite = a.SiteGuid
		FROM tblProducts a
		WHERE ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', @ProductGuid, @TargetSiteGuid)

		EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Product', @masterRecGuid, @assignedFrom, @isProductRecVerOn OUTPUT


		DECLARE @tblTargetCompanies TABLE
		(
			CompanyGuid uniqueidentifier,
			MasterRecordGuid uniqueidentifier
		)
		
		
		IF (@isProductRecVerOn = 0)
		BEGIN		
			-- Account for the case where Product Record Versioning has been turned off, therefore the children Prod-Com mappings have been deleted, and the Products at the AssignedTo sites/sitegroups are referencing their Company mappings using the Company Parent Record Guid at the Product Owner Site (even though the Company child record versions might exist at the target site), just as before Company Record Versioning was turned on.
			-- If a Product-To-Company mapping does not exist for a Company record version of the target site against any of the version of the target Product, then reference the Company using its Parent Company Guid at the Product Site Owner, instead of the Company Guid at the target site.
			INSERT INTO @tblTargetCompanies
			(CompanyGuid, MasterRecordGuid)
			SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', a.CompanyGuid, @firstAvailableOwnerSite), a.CompanyGuid FROM map.tblEntityCompanyToSite a 
			WHERE a.SiteGuid = @TargetSiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) b
				INNER JOIN map.tblProductToCompany c
				ON c.AssignedToCompanyGuid = b.CompanyGuid
				INNER JOIN tblProducts d
				ON d.ProductGuid = c.ProductGuid
				WHERE b.MasterRecordGuid = a.CompanyGuid
				AND d._MasterRecordGuid = @masterRecGuid
			)
		END

		INSERT INTO @tblTargetCompanies
		(CompanyGuid, MasterRecordGuid)
		SELECT a.CompanyGuid, a.MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) a
		WHERE NOT EXISTS
		(
			SELECT * FROM @tblTargetCompanies b
			WHERE b.MasterRecordGuid = a.MasterRecordGuid
		)


		SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode AS PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, d.ID AS AdditiveProfileID, e.TankID AS TankID , 
		f.ID AS AssignedToID, f.Name AS AssignedToName, f.Address1 AS AssignedToAddress, f.City AS AssignedToCity, 
		f.State AS AssignedToState  
		FROM map.tblProductToCompany a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c  ON c.ProductGuid = b.ProductGuid  
		LEFT OUTER JOIN tblAdditiveProfiles d ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT OUTER JOIN tblTanks e ON e.TankGuid = a.TankGuid  
		INNER JOIN @tblTargetCompanies g ON g.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies f ON f.CompanyGuid = g.CompanyGuid
		WHERE a.ProductGuid = @ProductGuid
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
						+ 'Procedure Name: [map].usp_GetProductToCompanyMapByProdGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END