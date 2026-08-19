/*
	DROP PROCEDURE [map].[usp_CompanyToSiteDelete]

	EXEC [map].[usp_CompanyToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'
	EXEC [map].[usp_CompanyToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', NULL
	EXEC [map].[usp_CompanyToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', NULL, 1

*/
CREATE PROCEDURE [map].[usp_CompanyToSiteDelete]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@DeleteBaseMapping bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_CompanyToSiteDelete]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes a CompanyToSite mapping entry.
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the Company record for which the mapping is to be deleted. This can be either the Master Record Guid or the actual record guid.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping is to be deleted. 
	--    If the @AssignedToSiteGuid parameter is null, then all the Company to Site mappings for the entity record are deleted.
	-- 3. @DeleteBaseMapping: 0: Do not delete the base mapping for the entity record. 1: Delete the base mapping for the entity record.
	-- 3. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 4. This operation also deletes all the other CompanyToSite assignments that have been made possible by the given assignment (Cascading entity assignment deletion).
	-- 5. For each EntityToSite assignment deleted by this operation, the associated record version, if it exists, is also deleted.
	-- 6. The base mapping is the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself.
	--	  It is only deleted if the @DeleteBaseMapping parameter is set to 1 and the AssignedToSiteGuid is NULL
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @EntityMasterRecordGuid uniqueidentifier
		SELECT @EntityMasterRecordGuid = _MasterRecordGuid FROM tblCompanies
		WHERE CompanyGuid = @EntityRecordGuid

		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			MappingGuid uniqueidentifier,
			AssignedFromSiteGuid uniqueidentifier,
			AssignedToSiteGuid uniqueidentifier,
			HierarchyLevel integer
		)

		--Get the assignment hierarchy that was built off the assignment that is to be deleted, i.e. the assignments that were subsequently created from the assignment that is to be deleted.
		INSERT INTO @tblEntityToSiteHierarchy
		(MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, HierarchyLevel)
		SELECT MappingGuid, AssignedFromSiteGuid, SiteGuid, HierarchyLevel FROM [erv].[udf_GetCompanyToSiteHierarchyByAssignment] (@EntityMasterRecordGuid, NULL, @AssignedToSiteGuid)

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --DeleteCompanyAssignmentMappings
            SET @BeginTran = 1   
		END  
		BEGIN TRY
			--Delete all the child record versions from the assignment hierarchy

		
			--Delete the AuthorizedShipTo mappings of the child record versions
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.CompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			--Delete the Drivers mappings of the child record versions
			DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.CompanyGuid <> b._MasterRecordGuid  OR @DeleteBaseMapping = 1)

			--Delete the UnavailableInventories mappings of the child record versions
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.AssignedToCompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			--Delete the ShipToAuthorizedProducts mappings of the child record versions
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.AssignedToCompanyGuid <> b._MasterRecordGuid  OR @DeleteBaseMapping = 1)

			--Delete the AuthorizedCarriers mappings of the child record versions
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.AssignedToCompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			--Delete the SupplierAuthorizedProducts mappings of the child record versions
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.AssignedToCompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			--Delete the AccessSchedule mappings of the child record versions
			DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.CompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			--Delete the CertificatesAndPermits mappings of the child record versions
			DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND (a.CompanyGuid <> b._MasterRecordGuid OR @DeleteBaseMapping = 1)

			-- Delete the Company to User Group mappings
			-- Company-UserGroup mappings use the Company MasterRecordGuid (because the mapping table has a SiteGuid field)
			DELETE cctug FROM map.tblCompanyCompanyToUserGroup cctug
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cctug.SiteGuid
			WHERE cctug.[CompanyGuid] = @EntityMasterRecordGuid

			--Delete the CompanyRoles mappings of the child record versions
			--Company Roles are created/cloned independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.		
			--However the independent CompanyToRole maintenance process is not aware of cascading assignments and the need for cascading deletions.
			--The query below handles the cascading deletion needs of map.tblCompanyToRole.
			DELETE a FROM [map].[tblCompanyToRole] a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.AssignedToSiteGuid = a.SiteGuid
			WHERE a.CompanyGuid = @EntityMasterRecordGuid
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			--Delete the child record versions
			DELETE a FROM dbo.tblCompanies a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.AssignedToSiteGuid = a.SiteGuid
			WHERE a._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.CompanyGuid <> a._MasterRecordGuid

			--Delete the assignment hierarchy
			DELETE a FROM map.tblEntityCompanyToSite a
			INNER JOIN @tblEntityToSiteHierarchy b 
			ON b.MappingGuid = a.CompanyToSiteGuid

			--Delete the base mapping
			DELETE a FROM map.tblEntityCompanyToSite a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			AND b.SiteGuid = a.SiteGuid
			WHERE a.CompanyGuid = @EntityMasterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND @AssignedToSiteGuid IS NULL
			AND @DeleteBaseMapping = 1

			-- Deletion of Allocatons Line Items and Allocations linked to all Company Hierarchy

			-- Delete the allocation line items for Shiip To To Bill To
			DELETE ali FROM dbo.tblAllocationLineItems ali
			INNER JOIN dbo.tblAllocations a
			ON ali.AllocationGuid = a.AllocationGuid
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = a.CompanyShipToToBillToGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE csttbt.CompanyGuid = @EntityMasterRecordGuid
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)


			-- Delete the allocations for Shiip To To Bill To
			DELETE a FROM dbo.tblAllocations a
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = a.CompanyShipToToBillToGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE csttbt.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the allocation line items for Bill To To Shipper
			DELETE ali FROM dbo.tblAllocationLineItems ali
			INNER JOIN dbo.tblAllocations a
			ON ali.AllocationGuid = a.AllocationGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = a.CompanyBillToToShipperGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the allocations for Bill To To Shipper
			DELETE a FROM dbo.tblAllocations a
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = a.CompanyBillToToShipperGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the allocation line items for Shipper To Owner
			DELETE ali FROM dbo.tblAllocationLineItems ali
			INNER JOIN dbo.tblAllocations a
			ON ali.AllocationGuid = a.AllocationGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = a.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the allocations for Shipper To Owner
			DELETE a FROM dbo.tblAllocations a
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = a.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the allocation line items for Load Owner To Manager
			DELETE ali FROM dbo.tblAllocationLineItems ali
			INNER JOIN dbo.tblAllocations a
			ON ali.AllocationGuid = a.AllocationGuid
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = a.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)


			-- Delete the allocations for Load Owner To Manager
			DELETE a FROM dbo.tblAllocations a
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = a.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = a.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid 
			AND (a.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Deletion of company PIDS profile mappint to all Company Hierarchy

			-- Delete the company PIDX profile mapping
			DELETE pptc FROM map.tblPIDXProfileToCompany pptc
			INNER JOIN map.tblCompanyPersonnelToShipToBillTo cptstbt
			ON cptstbt.CompanyPersonnelToShipToBillToGuid = pptc.CompanyPersonnelToShipToBillToGuid
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = pptc.SiteGuid
			WHERE csttbt.CompanyGuid = @EntityMasterRecordGuid
			AND (pptc.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			DELETE pptc FROM map.tblPIDXProfileToCompany pptc
			INNER JOIN map.tblCompanyPersonnelToShipToBillTo cptstbt
			ON cptstbt.CompanyPersonnelToShipToBillToGuid = pptc.CompanyPersonnelToShipToBillToGuid
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = pptc.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid
			AND (pptc.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			DELETE pptc FROM map.tblPIDXProfileToCompany pptc
			INNER JOIN map.tblCompanyPersonnelToShipToBillTo cptstbt
			ON cptstbt.CompanyPersonnelToShipToBillToGuid = pptc.CompanyPersonnelToShipToBillToGuid
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = pptc.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (pptc.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			DELETE pptc FROM map.tblPIDXProfileToCompany pptc
			INNER JOIN map.tblCompanyPersonnelToShipToBillTo cptstbt
			ON cptstbt.CompanyPersonnelToShipToBillToGuid = pptc.CompanyPersonnelToShipToBillToGuid
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = pptc.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (pptc.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Deletion of Company Maps linked to all Company Hierarchy

			-- Delete the company maps LoadIDs linked Load Onwer To Manager
			DELETE cptstbt FROM map.tblCompanyPersonnelToShipToBillTo cptstbt
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptstbt.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (cptstbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Ship To To Bill To linked Load Onwer To Manager
			DELETE csttbt FROM map.tblCompanyShipToToBillTo csttbt
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csttbt.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (csttbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Bill To To Shipper To linked Load Onwer To Manager
			DELETE cbtts FROM map.tblCompanyBillToToShipper cbtts
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cbtts.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (cbtts.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Shiipper To Owner linked Load Onwer To Manager
			DELETE csto FROM map.tblCompanyShipperToOwner csto
			INNER JOIN map.tblCompanyLoadOwnerToManager clotm
			ON clotm.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csto.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (csto.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Load Onwer To Manager
			DELETE clotm FROM map.tblCompanyLoadOwnerToManager clotm
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = clotm.SiteGuid
			WHERE clotm.CompanyGuid = @EntityMasterRecordGuid
			AND (clotm.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps LoadIDs linked Shipper To Onwer
			DELETE cptstbt FROM map.tblCompanyPersonnelToShipToBillTo cptstbt
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptstbt.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (cptstbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Ship To To Bill To linked Shipper To Onwer
			DELETE csttbt FROM map.tblCompanyShipToToBillTo csttbt
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csttbt.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (csttbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Bill To To Shipper To Shipper To Onwer
			DELETE cbtts FROM map.tblCompanyBillToToShipper cbtts
			INNER JOIN map.tblCompanyShipperToOwner csto
			ON csto.CompanyShipperToOwnerGuid = cbtts.CompanyShipperToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cbtts.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (cbtts.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Shiipper To Owner
			DELETE csto FROM map.tblCompanyShipperToOwner csto
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csto.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (csto.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps LoadIDs linked Bill To To Shipper
			DELETE cptstbt FROM map.tblCompanyPersonnelToShipToBillTo cptstbt
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptstbt.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid
			AND (cptstbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Ship To To Bill To linked Bill To To Shipper
			DELETE csttbt FROM map.tblCompanyShipToToBillTo csttbt
			INNER JOIN map.tblCompanyBillToToShipper cbtts
			ON cbtts.CompanyBillToToShipperGuid = csttbt.CompanyBillToToShipperGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csttbt.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid
			AND (csttbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Bill To To Shipper
			DELETE cbtts FROM map.tblCompanyBillToToShipper cbtts
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cbtts.SiteGuid
			WHERE cbtts.CompanyGuid = @EntityMasterRecordGuid
			AND (cbtts.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps LoadIDs linked Ship To To Bill To
			DELETE cptstbt FROM map.tblCompanyPersonnelToShipToBillTo cptstbt
			INNER JOIN map.tblCompanyShipToToBillTo csttbt
			ON csttbt.CompanyShipToToBillToGuid = cptstbt.CompanyShipToToBillToGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptstbt.SiteGuid
			WHERE csttbt.CompanyGuid = @EntityMasterRecordGuid
			AND (cptstbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Ship To To Bill
			DELETE csttbt FROM map.tblCompanyShipToToBillTo csttbt
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csttbt.SiteGuid
			WHERE csttbt.CompanyGuid = @EntityMasterRecordGuid
			AND (csttbt.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps LoadIDs linked OffLoad Onwer To Manager
			DELETE cptso FROM map.tblCompanyPersonnelToSupplierOwner cptso
			INNER JOIN map.tblCompanySupplierToOwner csto
			ON csto.CompanySupplierToOwnerGuid = cptso.CompanySupplierToOwnerGuid
			INNER JOIN map.tblCompanyOffLoadOwnerToManager colotm
			ON colotm.CompanyOffLoadOwnerToManagerGuid = csto.CompanyOffLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptso.SiteGuid
			WHERE colotm.CompanyGuid = @EntityMasterRecordGuid
			AND (cptso.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Supplier To Owner linked Off Load Onwer To Manager
			DELETE csto FROM map.tblCompanySupplierToOwner csto
			INNER JOIN map.tblCompanyOffLoadOwnerToManager colotm
			ON colotm.CompanyOffLoadOwnerToManagerGuid = csto.CompanyOffLoadOwnerToManagerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csto.SiteGuid
			WHERE colotm.CompanyGuid = @EntityMasterRecordGuid
			AND (csto.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Off Load Onwer To Manager
			DELETE colotm FROM map.tblCompanyOffLoadOwnerToManager colotm
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = colotm.SiteGuid
			WHERE colotm.CompanyGuid = @EntityMasterRecordGuid
			AND (colotm.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps LoadIDs linked Supplier Onwer
			DELETE cptso FROM map.tblCompanyPersonnelToSupplierOwner cptso
			INNER JOIN map.tblCompanySupplierToOwner csto
			ON csto.CompanySupplierToOwnerGuid = cptso.CompanySupplierToOwnerGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = cptso.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (cptso.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Delete the company maps Supplier To Owner
			DELETE csto FROM map.tblCompanySupplierToOwner csto
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = csto.SiteGuid
			WHERE csto.CompanyGuid = @EntityMasterRecordGuid
			AND (csto.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			-- Update any Equipment that references Company if DeleteBaseMapping
			UPDATE e SET CompanyGuid = null, CompanyEquipmentID = ''
			FROM tblEquipment e
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = e.SiteGuid
			WHERE CompanyGuid = @EntityMasterRecordGuid
			AND (e.SiteGuid <> (SELECT SiteGuid FROM tblCompanies WHERE CompanyGuid = @EntityMasterRecordGuid AND CompanyGuid = _MasterRecordGuid)  OR @DeleteBaseMapping = 1)

			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION --DeleteCompanyAssignmentMappings
		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --DeleteCompanyAssignmentMappings
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;
			SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		END CATCH
		
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
						+ 'Procedure Name: map.usp_CompanyToSiteDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
