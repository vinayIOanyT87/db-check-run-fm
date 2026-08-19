
/*

	EXEC [map].[usp_GetProdToSupplierProdComMapByComGuid] '38B24944-C577-4193-B9FC-554205CB39D7', '00000000-0000-0000-0000-000000000001'


*/

CREATE PROCEDURE [map].[usp_GetProdToSupplierProdComMapByComGuid]
(
	@CompanyGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier, @HideHiddenProducts BIT = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetProdToSupplierProdComMapByComGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the map.tblProductToSupplierProductCompany mappings for a given CompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @CompanyGuid: CompanyGuid for which to fetch the mapping records.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. @HideHiddenProducts: If true (1), only products with a NULL HiddenDate will be returned
	-- 4. This stored procedure replaces the ProductMapClass.ProductMapClass.EnumerateByAssignedToGuidAndTypeSQL() inline SQL, for the case where Type = PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP and where bInTransaction is false.
	-- 5. This query is both Company and Product Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		DECLARE @isCompanyRecVerOn bit
		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @assignedFrom uniqueidentifier
		DECLARE @firstAvailableOwnerSite uniqueidentifier
		
		SELECT @masterRecGuid = a._MasterRecordGuid, @assignedFrom = b.AssignedFromSiteGuid
		FROM tblCompanies a 
		INNER JOIN map.tblEntityCompanyToSite b ON b.CompanyGuid = a._MasterRecordGuid
		WHERE a.CompanyGuid = @CompanyGuid AND b.SiteGuid = @TargetSiteGuid

		SELECT @firstAvailableOwnerSite = a.SiteGuid
		FROM tblCompanies a
		WHERE CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', @CompanyGuid, @TargetSiteGuid)

		EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Company', @masterRecGuid, @assignedFrom, @isCompanyRecVerOn OUTPUT


		DECLARE @tblTargetProducts TABLE
		(
			ProductGuid uniqueidentifier,
			MasterRecordGuid uniqueidentifier
		)
		
		
		IF (@isCompanyRecVerOn = 0)
		BEGIN		
			-- Account for the case where Company Record Versioning has been turned off, therefore the children Prod-Com mappings have been deleted, and the Companies at the AssignedTo sites/sitegroups are referencing their Product mappings using the Product Parent Record Guid at the Company Owner Site (even though the Product child record versions might exist at the target site), just as before Product Record Versioning was turned on.
			-- If a Product-To-Company mapping does not exist for a Product record version of the target site against any of the version of the target Company, then reference the Product using its Parent Product Guid at the Company Site Owner, instead of the Product Guid at the target site.
			INSERT INTO @tblTargetProducts
			(ProductGuid, MasterRecordGuid)
			SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', a.ProductGuid, @firstAvailableOwnerSite), a.ProductGuid FROM map.tblEntityProductToSite a 
			WHERE a.SiteGuid = @TargetSiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b
				INNER JOIN map.tblProductToSupplierProductCompany c
				ON c.ProductGuid = b.ProductGuid
				INNER JOIN tblCompanies d
				ON d.CompanyGuid = c.AssignedToCompanyGuid
				WHERE b.MasterRecordGuid = a.ProductGuid
				AND d._MasterRecordGuid = @masterRecGuid
			)
		END

		INSERT INTO @tblTargetProducts
		(ProductGuid, MasterRecordGuid)
		SELECT a.ProductGuid, a.MasterRecordGuid FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
		WHERE NOT EXISTS
		(
			SELECT * FROM @tblTargetProducts b
			WHERE b.MasterRecordGuid = a.MasterRecordGuid
		)


		SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, c.Description AS AssignedDescription, 
		c.LookupProductTypeIndex AS AssignedProductType, c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
		c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, c.PIDXFamilyCode as PIDXFamilyCode, c.IsEthanol AS IsEthanol,
		c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, d.ID AS AdditiveProfileID, e.TankID AS TankID , 
		f.ID AS AssignedToID, f.Name AS AssignedToName, f.Address1 AS AssignedToAddress, f.City AS AssignedToCity, 
		f.State AS AssignedToState  
		FROM map.tblProductToSupplierProductCompany a
		INNER JOIN @tblTargetProducts b ON b.ProductGuid = a.ProductGuid  
		INNER JOIN tblProducts c  ON c.ProductGuid = b.ProductGuid  
		LEFT OUTER JOIN tblAdditiveProfiles d ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
		LEFT OUTER JOIN tblTanks e ON e.TankGuid = a.TankGuid  
		INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) g	ON g.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies f ON f.CompanyGuid = g.CompanyGuid		
		WHERE a.AssignedToCompanyGuid = @CompanyGuid
		AND (@HideHiddenProducts = 0 OR c.HiddenDate IS NULL) 
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
						+ 'Procedure Name: [map].usp_GetProdToSupplierProdComMapByComGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END