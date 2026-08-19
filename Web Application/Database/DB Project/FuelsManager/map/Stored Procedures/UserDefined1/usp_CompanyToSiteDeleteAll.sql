/*
	DROP PROCEDURE [map].[usp_CompanyToSiteDeleteAll]

	EXEC [map].[usp_CompanyToSiteDeleteAll] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'

*/
CREATE PROCEDURE [map].[usp_CompanyToSiteDeleteAll]
(
	@AssignedFromSiteGroupGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_CompanyToSiteDeleteAll]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Cascade deletes all the Company-to-site mappings and the associated record versions for all the Company-to-site mappings between two sites.
	-- Notes:
	-- 1. @AssignedFromSiteGroupGuid: Guid of the AssignedFrom sitegroup for which the Entity-to-site assignments are to be deleted.
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the Entity-to-site assignments are to be deleted.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @runningMappingLevel int

		IF (@AssignedFromSiteGroupGuid = @AssignedToSiteGuid)  -- this is not to be used to delete base mappings
		BEGIN
			RETURN
		END

		DECLARE @tblEntityToSiteMappings TABLE
		(
			MasterRecGuid uniqueidentifier
			, AssignedFromSiteGuid uniqueidentifier
			, AssignedToSiteGuid uniqueidentifier
			, MappingLevel int
			, Processed bit
		);		

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION
            SET @BeginTran = 1   
		END  		

		--Retrieve all the direct entity-to-site assignment mappings from the Target sitegroup to any of the site/groups to which the target sitegroup is no longer a parent
		INSERT INTO @tblEntityToSiteMappings
		(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
		SELECT a.CompanyGuid, a.AssignedFromSiteGuid, a.SiteGuid, 0, 0 FROM map.tblEntityCompanyToSite a
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGroupGuid
		AND a.SiteGuid = @AssignedToSiteGuid

		--Also extract all the subsequent entity-to-site mappings that derive from the direct mappings above
		SET @runningMappingLevel = 0
		WHILE ((SELECT COUNT(*) FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel) > 0)
		BEGIN
			SET @runningMappingLevel = @runningMappingLevel + 1
			INSERT INTO @tblEntityToSiteMappings
			(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
			SELECT a.CompanyGuid, a.AssignedFromSiteGuid, a.SiteGuid, @runningMappingLevel, 0 FROM map.tblEntityCompanyToSite a
			INNER JOIN @tblEntityToSiteMappings b
			ON b.MasterRecGuid = a.CompanyGuid
			WHERE b.MappingLevel = 0
			AND a.AssignedFromSiteGuid IN 
			(
				SELECT AssignedToSiteGuid FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel-1
			)									
		END


		--For each affected entity-to-site mapping, delete the corresponding child record version
		--Delete the external attributes of the parent record version
	
		--Equipments. The relationship between Company and Equipment is maintained fully on the Equipment side, and references the Company using the Company MasterRecordGuid, 
		--and as such, there are no record versioning changes to be applied to the Company-Equipment relationships during a CompanyToSite mapping deletion.			
		

		--AuthorizedShipTo
		DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.CompanyGuid <> b._MasterRecordGuid
		
		--Drivers 
		DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.CompanyGuid <> b._MasterRecordGuid
				
		--UnavailableInventories
		DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.AssignedToCompanyGuid <> b._MasterRecordGuid

		--ShipToAuthorizedProducts
		DELETE a FROM [map].[tblProductToCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.AssignedToCompanyGuid <> b._MasterRecordGuid

		--AuthorizedCarriers
		DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.AssignedToCompanyGuid <> b._MasterRecordGuid

		--SupplierAuthorizedProducts
		DELETE a FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.AssignedToCompanyGuid <> b._MasterRecordGuid

		--AccessSchedule
		DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.CompanyGuid <> b._MasterRecordGuid

		--CertificatesAndPermits
		DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.CompanyGuid <> b._MasterRecordGuid

		--UserGroups
		-- Company-UserGroup mappings use the Company MasterRecordGuid (because the mapping table has a SiteGuid field)
		DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
		INNER JOIN dbo.tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = a.SiteGuid

		--Company Roles. Company Roles are created/cloned and deleted independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.		
		--However the independent CompanyToRole maintenance process is not aware of cascading assignments and the need for cascading deletions.
		--The query below handles the cascading deletion needs of map.tblCompanyToRole.
		DELETE a FROM [map].[tblCompanyToRole] a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.AssignedToSiteGuid = a.SiteGuid
		AND b.MasterRecGuid = a.CompanyGuid
		INNER JOIN dbo.tblCompanies c
		ON c._MasterRecordGuid = a.CompanyGuid
		AND c.SiteGuid = a.SiteGuid
		WHERE c.CompanyGuid <> c._MasterRecordGuid

		--Delete the child record versions
		DELETE a FROM tblCompanies a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a._MasterRecordGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.CompanyGuid <> a._MasterRecordGuid

		--Delete the entity-to-site mappings affected by the site-to-site mapping deletion
		DELETE a 
		FROM map.tblEntityCompanyToSite a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a.CompanyGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.AssignedFromSiteGuid <> a.SiteGuid

		-- Delete the allocation line items
		DELETE a FROM dbo.tblAllocationLineItems ali
		INNER JOIN dbo.tblAllocations a
		ON ali.AllocationGuid = a.AllocationGuid
		WHERE a.SiteGuid = @AssignedToSiteGuid 

		-- Delete the allocations
		DELETE a FROM dbo.tblAllocations a
		WHERE a.SiteGuid = @AssignedToSiteGuid 

		-- Delete the company PIDX profile mapping
		DELETE pptc FROM map.tblPIDXProfileToCompany pptc
		WHERE pptc.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Personnel To Ship To To Bill To
		DELETE cptstbt FROM map.tblCompanyPersonnelToShipToBillTo cptstbt
		WHERE cptstbt.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Ship To To Bill To
		DELETE csttbt FROM map.tblCompanyShipToToBillTo csttbt
		WHERE csttbt.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Bill To To Shipper
		DELETE cbtts FROM map.tblCompanyBillToToShipper cbtts
		WHERE cbtts.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Shipper to Owner
		DELETE csto FROM map.tblCompanyShipperToOwner csto
		WHERE csto.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Load Owner to Manager
		DELETE clotom FROM map.tblCompanyLoadOwnerToManager clotom
		WHERE clotom.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Personnel To Supplier to Owner
		DELETE cptso FROM map.tblCompanyPersonnelToSupplierOwner cptso
		WHERE cptso.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Supplier to Owner
		DELETE csto FROM map.tblCompanySupplierToOwner csto
		WHERE csto.SiteGuid = @AssignedToSiteGuid

		-- Delete Company Map Offload Oener to Manager
		DELETE colotm FROM map.tblCompanyOffLoadOwnerToManager colotm
		WHERE colotm.SiteGuid = @AssignedToSiteGuid

		-- Update any Equipment that references Company if DeleteBaseMapping
		UPDATE e SET CompanyGuid = null, CompanyEquipmentID = ''
		FROM tblEquipment e
		WHERE e.SiteGuid = @AssignedToSiteGuid


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
						+ 'Procedure Name: map.usp_CompanyToSiteDeleteAll' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
