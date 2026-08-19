/*
	DROP PROCEDURE [erv].[usp_CreatePersonnelChildRecordVersionBySegment]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	EXEC [erv].[usp_CreatePersonnelChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001', @dt, 'HB'
	--EXEC [erv].[usp_CreatePersonnelChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

*/

CREATE PROCEDURE [erv].[usp_CreatePersonnelChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreatePersonnelChildRecordVersionBySegment] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Creates a new Personnel record version for each of the existing entity assignments of a given Personnel segment from a given SiteGroup.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.
	-- 2. @SourceSiteGroupGuid: SiteGroup parent from which the record version are to be created. This would correspond to the AssignedFrom Sitegroup.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		--Verify if RecordVersioning is turned ON for the source sitegroup before creating new child record versions from it.	
		DECLARE @tblVersionSpecificFields TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);
		INSERT @tblVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid

		IF ((SELECT COUNT(*) FROM @tblVersionSpecificFields) = 0)
		BEGIN
			RETURN
		END


		--Capture the Site/SiteGroup, MasterRecordGuid, and the parent record versions for the entity assignments from which new record versions need to be created/cloned.
		DECLARE @tblTargetEntitySite TABLE
		(
			SiteGuid uniqueidentifier,
			MasterRecordGuid uniqueidentifier,
			ParentEntityGuid uniqueidentifier,
			PersonnelGuid uniqueidentifier  -- The child record version PersonnelGuid is not initially available since the process will be creating the new Personnel child record versions, but it is populated and used further down the process when handling the external attributes.
		)

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Personnel')
		BEGIN
			INSERT INTO @tblTargetEntitySite
			(SiteGuid, MasterRecordGuid, ParentEntityGuid)
			SELECT b.SiteGuid, b.PersonnelGuid, a.PersonnelGuid
			FROM tblPersonnel a
			INNER JOIN map.tblEntityPersonnelToSite b
			ON b.PersonnelGuid = a._MasterRecordGuid
			AND b.AssignedFromSiteGuid = a.SiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about creating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no need to create a child record version in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND NOT EXISTS
			(SELECT * FROM tblPersonnel c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.SiteGuid = b.SiteGuid)
			AND b.SiteGuid <> b.AssignedFromSiteGuid
		END
				

		--Create the child record versions by cloning the internal fields of the parent record version
		INSERT INTO tblPersonnel
		(PersonnelGuid,_MasterRecordGuid,SiteGuid,PersonID,CardNumber,UserGuid,FirstName,MiddleName,LastName,Title,Department,SupervisorPersonnelGuid,Address1,Address2,City,State,Zip,Country,Phone1,Phone2,AssignmentDate,SupervisionDate,SSAN,BirthDate,PayRate,LaborRate1,LaborRate2,LaborRate3,LaborRate4,Status,Email,ResponsibleOfficer,Shift,CompanyGuid,PINNumber,PINRequired,LockedOut,LockedOutReason,LockedOutDate,LastActivityDate,CardedIn,ShortCardNumber,AssignedEquipmentGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,OnFileSignature,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,HiddenDate,InhibitInactivityLockout)		
		SELECT NewID(),a._MasterRecordGuid,b.SiteGuid,a.PersonID,a.CardNumber,a.UserGuid,a.FirstName,a.MiddleName,a.LastName,a.Title,a.Department,a.SupervisorPersonnelGuid,a.Address1,a.Address2,a.City,a.State,a.Zip,a.Country,a.Phone1,a.Phone2,a.AssignmentDate,a.SupervisionDate,a.SSAN,a.BirthDate,a.PayRate,a.LaborRate1,a.LaborRate2,a.LaborRate3,a.LaborRate4,a.Status,a.Email,a.ResponsibleOfficer,a.Shift,a.CompanyGuid,a.PINNumber,a.PINRequired,a.LockedOut,a.LockedOutReason,a.LockedOutDate,a.LastActivityDate,a.CardedIn,a.ShortCardNumber,a.AssignedEquipmentGuid,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy,a.OnFileSignature,a.UserData1,a.UserData2,a.UserData3,a.UserData4,a.UserData5,a.UserData6,a.UserData7,a.UserData8,a.UserData9,a.UserData10,a.UserData11,a.UserData12,a.UserData13,a.UserData14,a.UserData15,a.UserData16,a.UserData17,a.UserData18,a.UserData19,a.UserData20,a.UserData21,a.UserData22,a.UserData23,a.UserData24,a.HiddenDate,a.InhibitInactivityLockout
		FROM tblPersonnel a
		INNER JOIN @tblTargetEntitySite b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.ParentEntityGuid = a.PersonnelGuid

		--Clone the external attributes of the parent record version
		
		--Retrieve the first available Personnel record version applicable for all Personnel mappings to @SourceSiteGroupGuid
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it just updates the AssignedFromSiteGuid and the EntityGuid of the initial mapping record to reflect the actual parent record.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.PersonnelGuid, b.PersonnelGuid, a.SiteGuid, 0, @callingRef1Guid
		FROM map.tblEntityPersonnelToSite a
		LEFT OUTER JOIN tblPersonnel b
		ON b._MasterRecordGuid = a.PersonnelGuid
		AND b.SiteGuid = a.SiteGuid
		WHERE a.SiteGuid = @SourceSiteGroupGuid

		DECLARE @level int
		SET @level = 0

		WHILE ((SELECT COUNT(*) FROM erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef1Guid AND EntityGuid IS NULL) > 0)
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
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND a.EntityGuid IS NULL
		END				


		--Schedule Personnel Access		
		INSERT INTO tblSchedulePersonnelAccess 
		(SchedulePersonnelAccessGuid,PersonnelGuid,LookupDayOfWeekIndex,Enabled,OpeningTime,ClosingTime,EndOfDayEnabled,EndOfDayTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NEWID(),c.PersonnelGuid,a.LookupDayOfWeekIndex,a.Enabled,a.OpeningTime,a.ClosingTime,a.EndOfDayEnabled,a.EndOfDayTime,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM tblSchedulePersonnelAccess a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM tblSchedulePersonnelAccess d
			WHERE d.PersonnelGuid = c.PersonnelGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
		)

		-- Personnel Role
		INSERT INTO [map].[tblPersonnelToRole]
		(PersonnelToRoleGuid,PersonnelGuid,LookupPersonnelRoleIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),c.PersonnelGuid,a.LookupPersonnelRoleIndex,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblPersonnelToRole] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblPersonnelToRole] d
			WHERE d.PersonnelGuid = c.PersonnelGuid
			AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
		)


		--Personnel Qualification
		UPDATE a 
		SET a.PersonnelGuid = e.PersonnelGuid
		FROM [map].[tblQualificationPersonQualificationToPerson] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.PersonnelGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel f
		ON f.PersonnelGuid = a.PersonnelGuid
		WHERE e._MasterRecordGuid <> e.PersonnelGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblQualificationPersonQualificationToPerson]
		(QualificationPersonQualificationToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,c.PersonnelGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonQualificationToPerson] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d 
			WHERE d.PersonnelGuid = c.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
		)

		--Personnel License
		UPDATE a 
		SET a.PersonnelGuid = e.PersonnelGuid
		FROM [map].[tblQualificationPersonLicenseToPerson] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.PersonnelGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel f
		ON f.PersonnelGuid = a.PersonnelGuid
		WHERE e._MasterRecordGuid <> e.PersonnelGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblQualificationPersonLicenseToPerson]
		(QualificationPersonLicenseToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,c.PersonnelGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonLicenseToPerson] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d 
			WHERE d.PersonnelGuid = c.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
		)

		--Personnel Training
		UPDATE a 
		SET a.PersonnelGuid = e.PersonnelGuid
		FROM [map].[tblQualificationPersonTrainingToPerson] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.PersonnelGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel f
		ON f.PersonnelGuid = a.PersonnelGuid
		WHERE e._MasterRecordGuid <> e.PersonnelGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblQualificationPersonTrainingToPerson]
		(QualificationPersonTrainingToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,c.PersonnelGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonTrainingToPerson] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d 
			WHERE d.PersonnelGuid = c.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
		)


		--Carrier
		-- For all the CompanyPersonnelAssignedToCompany mappings that reference a Parent Company record version instead of the actual Company child record version, because Record Versioning 
		-- was previously OFF for Company for that site, update the Company field of the mapping to point to the newly created Company child record versions.
		UPDATE a 
		SET a.PersonnelGuid = e.PersonnelGuid
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.PersonnelGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblCompanies d
		ON d.CompanyGuid = a.CompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblPersonnel f
		ON f.PersonnelGuid = a.PersonnelGuid
		WHERE e._MasterRecordGuid <> e.PersonnelGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the tblCompanyPersonnelAssignedToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Company owned by a sitegroup/site lower than the SourceSiteGroup. Company is also an External Client of Personnel, which allows a Company at a lower site/sitegroup 
		--      to establish a relationship with a Personnel assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Personnel
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Company (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Personnel-to-Company relationships, i.e Personnel-to-Company relationships that did not exist prior to turning Personnel Record Versioning ON.
		-- Note: Mappings against a Company not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
		(PersonnelGuid, CompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.PersonnelGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Company is eventually mapped to the site.
		b.SiteGuid, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.CompanyGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] f 
			WHERE f.PersonnelGuid = c.PersonnelGuid
			AND f.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)
		)



		DELETE erv.tblTempEntityMappingHierarchy
		WHERE _CallingReferenceGuid = @callingRef1Guid

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
						+ 'Procedure Name: [erv].usp_CreatePersonnelChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
