/*
	DROP PROCEDURE [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel]

	DECLARE @tblTargetPersonnel erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetPersonnel
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Personnel', 'BCAD83C8-CBCD-4A4A-8BBD-2CA14AD0E7A9', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Personnel', 'B98881AA-540D-4127-92F8-E4CC75586D0A', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Personnel', '6F5FEF48-72B2-4AE0-A1F0-C74296D78487', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Personnel', '31062FCF-ADDC-428B-860D-D185862E1E8E', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Personnel', 'EE7C5B83-39D7-4956-BFBF-45869B1B06C7', '80B08634-D356-4569-B9A2-CD36DF955BD0', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Personnel', 'F16C052E-2549-4B00-81EC-1AD7818F6A49', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	EXEC [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel] @tblTargetPersonnel

	DECLARE @tblTargetPersonnel erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetPersonnel
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Personnel', 'EEDD179A-3C23-4E95-8845-8CC93762E289', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Personnel', '2B8513FD-1041-409F-89E8-9554B4A3CA0F', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'),
	('Personnel', '2238851C-3D9A-4D1B-A12F-8651172B264A', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Personnel', '91E94F2E-0032-4320-9CAF-406544768D26', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'),
	('Personnel', '8A73B209-E500-4777-B1F7-AA7E4A9B6221', '80B08634-D356-4569-B9A2-CD36DF955BD0', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Personnel', '459CF841-B562-4B04-AE4D-9DADE4F606EA', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')
	EXEC [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel] @tblTargetPersonnel

*/
CREATE PROCEDURE [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel]
(
	@tblTargetPersonnel erv.utt_EntityRecordVersions READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete, Add, or Update the Personnel-To-Company mappings to the support the deletions of a set of Personnel child record versions as a result of FLC configuration changes.	
	-- Notes:
	-- 1. @tblTargetPersonnel: Table containing the Personnel record versions whose deletion impact on the Personnel-to-Company mappings need to be addressed.
	-- 2. This procedure addresses the Shared Mappings needs of the Personnel-to-Company mappings when a Personnel record version is deleted.
	-- 3. This procedure is to be executed before the actual deletion of the Personnel child record versions.
	-- 4. This procedure assumes that the Personnel will still be mapped to the target site/sitegroup after the deletion, even though the Personnel child record version will be deleted, i.e.
	--    this procedure is not to be used in the case of Personnel-to-Site mapping deletions.
	-- 5. It handles the Personnel-to-Company mappings managed by the tblCompanyPersonnelAssignedToCompany table
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		
		DECLARE @mappingCount int
		DECLARE @level int

		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()	
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION 
            SET @BeginTran = 1   
		END  
		BEGIN TRY
		
			------------------------------------------------------------map.tblCompanyPersonnelAssignedToCompany--------------------------------------------------------------------------------
			--For each Personnel that is mapped to a Company record version owned by the same site/sitegroup as the PPersonnel owner site or by a lower site/sitegroup, retrieve the details of the Company record version that is a parent to the Company record version that is tied in the mapping.
			INSERT INTO erv.tblTempPersonnelToCompanyForParentCompany
			(PersonnelGuid, PersonnelMasterRecordGuid, TargetSiteGuid, CompanyGuid, CompanyMasterRecordGuid, CompanyParentSiteGuid, ParentCompanyGuid, PersonnelParentSiteGuid, IsMasterRecordCompany, CompanyOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.CompanyGuid, c._MasterRecordGuid, e.SiteGuid, e.CompanyGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
			FROM @tblTargetPersonnel a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.PersonnelGuid = a.EntityGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = b.CompanyGuid -- This covers both Companies that are owned by the same site/sitegroup as the target Personnel, and those Companies that are owned by a lower site/sitegroup.
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN tblCompanies e
			ON e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid  -- Companies that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
			INNER JOIN tblPersonnel f
			ON f.PersonnelGuid = a.EntityGuid
			INNER JOIN map.tblEntityPersonnelToSite g
			ON g.PersonnelGuid = f._MasterRecordGuid
			AND g.SiteGuid = f.SiteGuid
			WHERE f.PersonnelGuid <> f._MasterRecordGuid  --Operation limited to Personnel child record versions
			AND f.SiteGuid = a.SiteGuid
			AND a.EntityTypeId = 'Personnel'


			--Retrieve the first available parent Personnel record version applicable for all the Personnel records captured in erv.tblTempPersonnelToCompanyForParentPersonnel, starting from the PersonnelParentSiteGuid.
			--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
			DECLARE @callingRef2Guid uniqueidentifier
			SET @callingRef2Guid = NEWID()

			INSERT INTO erv.tblTempEntityMappingHierarchy
			(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
			SELECT a.PersonnelMasterRecordGuid, b.PersonnelGuid, a.PersonnelParentSiteGuid, 0, @callingRef2Guid
			FROM erv.tblTempPersonnelToCompanyForParentCompany a
			LEFT OUTER JOIN tblPersonnel b
			ON b._MasterRecordGuid = a.PersonnelMasterRecordGuid
			AND b.SiteGuid = a.PersonnelParentSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid

			SET @level = 0

			WHILE ((SELECT COUNT(*) FROM erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid AND EntityGuid IS NULL) > 0)
			BEGIN
				SET @level = @level - 1
				IF (@level < -20)
				BEGIN
					RAISERROR('Maximum iteration of mapping hierarchy reached.',16,1);   --safeguard against infinite looping
					RETURN;
				END
				UPDATE a 
				SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.PersonnelGuid
				FROM erv.tblTempEntityMappingHierarchy a
				INNER JOIN map.tblEntityPersonnelToSite b
				ON b.PersonnelGuid = a.EntityMasterGuid
				AND b.SiteGuid = a.AssignedFromSiteGuid
				LEFT OUTER JOIN tblPersonnel c
				ON c._MasterRecordGuid = b.PersonnelGuid
				AND c.SiteGuid = b.SiteGuid
				WHERE a._CallingReferenceGuid = @callingRef2Guid
				AND a.EntityGuid IS NULL
			END							

			-- Retrieve the first available Personnel record applicable for the Personnel Parent Sitegroup. Note: Unlike with the Parent Company record, the PersonnelGuidForParentCompany does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
			UPDATE a 
			SET a.PersonnelGuidForParentCompany = b.EntityGuid
			FROM erv.tblTempPersonnelToCompanyForParentCompany a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.PersonnelMasterRecordGuid
			AND b.AssignedToSiteGuid = a.PersonnelParentSiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef2Guid

			DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
		
			--Mark all Companies that have a master record at either the target (AssignedTo) sitegroup of the Personnel or lower, as a MasterRecordCompany
			UPDATE a 
			SET a.IsMasterRecordCompany = 1
			FROM erv.tblTempPersonnelToCompanyForParentCompany a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.PersonnelMasterRecordGuid		
			WHERE a.CompanyMasterRecordGuid = a.ParentCompanyGuid
			AND a.CompanyParentSiteGuid = b.AssignedToSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef1Guid
			
			-- Retrieve the Forward Control Mode of the Company field that is used to control the map.tblCompanyPersonnelAssignedToCompany from the Company side
			UPDATE a 
			SET a.DriversFCM = b.ForwardControlMode
			FROM erv.tblTempPersonnelToCompanyForParentCompany a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.SiteGroupGuid = CompanyParentSiteGuid
			INNER JOIN erv.tblEntitySegmentTemplate c
			ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
			WHERE c.EntityTypeId = 'Company'
			AND b.TargetField = 'Drivers'
			AND a.CompanyOwnsRecordAtAssignedFromSitegroup = 1
			AND a._CallingReferenceGuid = @callingRef1Guid

			UPDATE erv.tblTempPersonnelToCompanyForParentCompany
			SET DriversFCM = 'ParentSpecific'
			WHERE _CallingReferenceGuid = @callingRef1Guid
			AND DriversFCM IS NULL
			AND IsMasterRecordCompany <> 1
			AND CompanyOwnsRecordAtAssignedFromSitegroup = 1

			--CapturePersonnel mappings with Companies which do not have a record that is owned by their AssignedFrom sitegroup
			INSERT INTO erv.tblTempPersonnelToCompanyForParentCompany
			(PersonnelGuid, PersonnelMasterRecordGuid, TargetSiteGuid, CompanyGuid, CompanyMasterRecordGuid, IsMasterRecordCompany, CompanyOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.CompanyGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetPersonnel a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.PersonnelGuid = a.EntityGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = b.CompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = a.SiteGuid
			WHERE c.SiteGuid <> a.SiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM tblCompanies e
				WHERE e._MasterRecordGuid = c._MasterRecordGuid
				AND e.SiteGuid = d.AssignedFromSiteGuid
			)
			AND NOT EXISTS 
			(
				SELECT * FROM erv.tblTempPersonnelToCompanyForParentCompany f
				WHERE f.PersonnelGuid = a.EntityGuid
				AND f.CompanyGuid = b.CompanyGuid
				AND f._CallingReferenceGuid = @callingRef1Guid
			)

			--Capture Personnel mappings with Companies which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
			INSERT INTO erv.tblTempPersonnelToCompanyForParentCompany
			(PersonnelGuid, PersonnelMasterRecordGuid, TargetSiteGuid, CompanyGuid, CompanyMasterRecordGuid, IsMasterRecordCompany, CompanyOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.CompanyGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetPersonnel a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.PersonnelGuid = a.EntityGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = b.CompanyGuid
			WHERE c.SiteGuid <> a.SiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM map.tblEntityCompanyToSite D
				WHERE d.CompanyGuid = c._MasterRecordGuid
				AND d.SiteGuid = a.SiteGuid
			)
			AND NOT EXISTS 
			(
				SELECT * FROM erv.tblTempPersonnelToCompanyForParentCompany e
				WHERE e.PersonnelGuid = a.EntityGuid
				AND e.CompanyGuid = b.CompanyGuid
				AND e._CallingReferenceGuid = @callingRef1Guid
			)


			--Delete all the mappings owned by the target Personnel child record versions if the FMC of the Company.Drivers is 'ParentSpecific', i.e. if the corresponding Company record in the mapping is not allowed to have its own version of the Personnel-to-Company mappings.
			DELETE a 
			FROM map.tblCompanyPersonnelAssignedToCompany a
			INNER JOIN erv.tblTempPersonnelToCompanyForParentCompany b
			ON b.PersonnelGuid = a.PersonnelGuid
			AND b.CompanyGuid = a.CompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.DriversFCM = 'ParentSpecific') OR (CompanyOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordCompany <> 1

		
			-- If a Company in the mappings owned by the target Personnel child record versions has a Parent Company record which itself has a mapping with the Parent Personnel record, then clone that parent mapping for the child Company record version associated with the target Personnel child record version.
			INSERT INTO map.tblCompanyPersonnelAssignedToCompany 
			(PersonnelGuid, CompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.PersonnelGuid, b.CompanyGuid, b.TargetSiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM map.tblCompanyPersonnelAssignedToCompany a 
			INNER JOIN erv.tblTempPersonnelToCompanyForParentCompany b
			ON b.ParentCompanyGuid = a.CompanyGuid
			AND b.PersonnelGuidForParentCompany = a.PersonnelGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.CompanyOwnsRecordAtAssignedFromSitegroup = 1
			AND b.DriversFCM = 'ParentSpecific'
			AND b.IsMasterRecordCompany <> 1
			AND b.PersonnelGuidForParentCompany IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblCompanyPersonnelAssignedToCompany c
				WHERE c.PersonnelGuid = a.PersonnelGuid
				AND c.CompanyGuid = b.CompanyGuid
			)

			-- If the corresponding Company record in the target Personnel child record version mapping is allowed to have its own version of the Personnel-to-Company mappings, then do not delete that mapping, but simply modify it to point to the Parent Personnel Guid, instead of the target Personnel child record version (that is marked for deletion).
			UPDATE a 
			SET a.PersonnelGuid = b.PersonnelGuidForParentCompany, a.UpdatedDate = GETDATE()
			FROM map.tblCompanyPersonnelAssignedToCompany a
			INNER JOIN erv.tblTempPersonnelToCompanyForParentCompany b
			ON b.CompanyGuid = a.CompanyGuid
			AND b.PersonnelGuid = a.PersonnelGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.CompanyOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.DriversFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordCompany = 1)
			)
			AND b.PersonnelGuidForParentCompany IS NOT NULL

			
			DELETE erv.tblTempPersonnelToCompanyForParentCompany WHERE _CallingReferenceGuid = @callingRef1Guid

			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION 

		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION 
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
						+ 'Procedure Name: [erv].usp_SetPersonnelToCompanyMappingsForDeletedPersonnel' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END