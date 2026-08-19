

/*
	EXEC [map].[usp_EquipmentToSiteDelete] '09AE67FB-E988-4836-B444-68957B2ED33F', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [map].[usp_EquipmentToSiteDelete] '09AE67FB-E988-4836-B444-68957B2ED33F', NULL
	EXEC [map].[usp_EquipmentToSiteDelete] '09AE67FB-E988-4836-B444-68957B2ED33F', NULL, 1

*/
CREATE PROCEDURE [map].[usp_EquipmentToSiteDelete]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@DeleteBaseMapping bit = 0,
	@ExtendToCompartments bit = 1
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_EquipmentToSiteDelete]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes an EquipmentToSite mapping entry.
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the Equipment record for which the mapping is to be deleted. This can be either the Master Record Guid or the actual record guid.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping is to be deleted. 
	--    If the @AssignedToSiteGuid parameter is null, then all the Equipment to Site mappings for the entity record are deleted.
	-- 3. @DeleteBaseMapping: 0: Do not delete the base mapping for the entity record. 1: Delete the base mapping for the entity record.
	-- 4. @ExtendToCompartments: 0:Delete the mapping only on the target Equipment; 1: Extend the site assignment mapping deletion to the Compartments of the target Equipment as well. 
	-- 5. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 6. This operation also deletes all the other EquipmentToSite assignments that have been made possible by the given assignment (Cascading entity assignment deletion).
	-- 7. For each EntityToSite assignment deleted by this operation, the associated record version, if it exists, is also deleted.
	-- 8. The base mapping is the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself.
	--	  It is only deleted if the @DeleteBaseMapping parameter is set to 1 and the AssignedToSiteGuid is NULL
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @EntityMasterRecordGuid uniqueidentifier
		SELECT @EntityMasterRecordGuid = _MasterRecordGuid FROM tblEquipment
		WHERE EquipmentGuid = @EntityRecordGuid

		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			MappingGuid uniqueidentifier,
			AssignedFromSiteGuid uniqueidentifier,
			AssignedToSiteGuid uniqueidentifier,
			HierarchyLevel integer
		)

		DECLARE @tblTargetMasterRecordGuids TABLE
		(
			MasterRecordGuid uniqueidentifier
		)

		--Get the assignment hierarchy that was built off the assignment that is to be deleted, i.e. the assignments that were subsequently created from the assignment that is to be deleted.
		INSERT INTO @tblEntityToSiteHierarchy
		(MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, HierarchyLevel)
		SELECT MappingGuid, AssignedFromSiteGuid, SiteGuid, HierarchyLevel FROM [erv].[udf_GetEquipmentToSiteHierarchyByAssignment] (@EntityMasterRecordGuid, NULL, @AssignedToSiteGuid)

		INSERT INTO @tblTargetMasterRecordGuids
		(MasterRecordGuid)
		VALUES (@EntityMasterRecordGuid)

		-- Get the Compartments of the target Equipment and their assignment hierarchy
		DECLARE @CompartmentMasterRecordGuid uniqueidentifier
		DECLARE CompartmentCursor CURSOR FOR 
		  SELECT _MasterRecordGuid FROM dbo.tblEquipment
		  WHERE ParentEquipmentGuid = @EntityMasterRecordGuid
		  AND @ExtendToCompartments = 1
		OPEN CompartmentCursor 

			FETCH NEXT FROM CompartmentCursor INTO @CompartmentMasterRecordGuid
 
			WHILE @@FETCH_STATUS = 0  
			BEGIN 
				INSERT INTO @tblTargetMasterRecordGuids
				(MasterRecordGuid)
				VALUES (@CompartmentMasterRecordGuid)

				INSERT INTO @tblEntityToSiteHierarchy
				(MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, HierarchyLevel)
				SELECT MappingGuid, AssignedFromSiteGuid, SiteGuid, HierarchyLevel FROM [erv].[udf_GetEquipmentToSiteHierarchyByAssignment] (@CompartmentMasterRecordGuid, NULL, @AssignedToSiteGuid)

				FETCH NEXT FROM CompartmentCursor INTO @CompartmentMasterRecordGuid
			END 
		CLOSE CompartmentCursor 
		DEALLOCATE CompartmentCursor


		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --DeleteEquipmentAssignmentMappings
            SET @BeginTran = 1   
		END  
		BEGIN TRY
			--Delete all the child record versions from the assignment hierarchy
			--Delete the Tag and License External attributes of the child record versions
			DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			INNER JOIN @tblTargetMasterRecordGuids d
			ON d.MasterRecordGuid = b._MasterRecordGuid
			WHERE a.EquipmentGuid <> b._MasterRecordGuid

			--Delete the Test and Inspection External attributes of the child record versions
			DELETE a FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			INNER JOIN @tblTargetMasterRecordGuids d
			ON d.MasterRecordGuid = b._MasterRecordGuid
			WHERE a.EquipmentGuid <> b._MasterRecordGuid

			/*
			--Delete the Process Variable External attributes of the child record versions
			DELETE a FROM [dbo].[tblProcessVariableEquipment] a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			INNER JOIN @tblTargetMasterRecordGuids d
			ON d.MasterRecordGuid = b._MasterRecordGuid
			WHERE a.EquipmentGuid <> b._MasterRecordGuid
			*/

			--Delete the child record versions
			DELETE a FROM dbo.tblEquipment a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.AssignedToSiteGuid = a.SiteGuid
			INNER JOIN @tblTargetMasterRecordGuids c
			ON c.MasterRecordGuid = a._MasterRecordGuid
			WHERE a.EquipmentGuid <> a._MasterRecordGuid

			--Delete the assignment hierarchy
			DELETE a FROM map.tblEntityEquipmentToSite a
			INNER JOIN @tblEntityToSiteHierarchy b 
			ON b.MappingGuid = a.EquipmentToSiteGuid

			--Delete the base mapping
			DELETE a FROM map.tblEntityEquipmentToSite a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN @tblTargetMasterRecordGuids c
			ON c.MasterRecordGuid = a.EquipmentGuid
			WHERE b.EquipmentGuid = b._MasterRecordGuid
			AND @AssignedToSiteGuid IS NULL
			AND @DeleteBaseMapping = 1

			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION --DeleteEquipmentAssignmentMappings
		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --DeleteEquipmentAssignmentMappings
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
						+ 'Procedure Name: map.usp_EquipmentToSiteDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END