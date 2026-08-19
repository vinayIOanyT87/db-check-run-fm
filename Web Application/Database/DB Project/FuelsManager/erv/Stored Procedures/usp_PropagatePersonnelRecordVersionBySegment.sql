/*
	DROP PROCEDURE [erv].[usp_PropagatePersonnelRecordVersionBySegment]

	EXEC [erv].[usp_PropagatePersonnelRecordVersionBySegment] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagatePersonnelRecordVersionBySegment] '1eacc1d7-292d-4932-bc59-9c02740c6c19'

*/

CREATE PROCEDURE [erv].[usp_PropagatePersonnelRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagatePersonnelRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate all the Parent Specific fields of all the record versions in a Personnel segment from a given sitegroup down to all the sites/sitegroups that have a direct assignment from the given sitegroup.
	-- This Stored Procedure is to be used to enforce the effect of fields being changed from VersionSpecific to ParentSpecific as a result of Field Level Control configuration changes.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Entity Segment Template that needs to be processed.
	-- 3. @SourceSiteGroupGuid: Guid of the segment SiteGroup from which the ParentSpecific fields are to be propagated.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		--Retrieve the VersionSpecific fields for the entity record
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid

		IF (NOT EXISTS (SELECT * FROM @tblSourceVersionSpecificFields))
		BEGIN				
			/*
				All fields are ParentSpecific. This means that there will be no child record versions of the entity record for any site/sitegroup in the hierarchy below owner 
				sitegroup of the entity record, i.e. Record Versioning field data propagation does not apply.
			*/
			RETURN;
		END

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		--Capture the Site/SiteGroup, MasterRecordGuid, and PersonnelGuid of the child record versions that need to be updated.
		--This includes all the child record versions down the site hierarchy that have the same masterrecordguid as those owned by the SourceSiteGroup and which share the same filter value as the segment being processed, irrespective of where they were assigned from.
		IF (@entityTypeId = 'Personnel')
		BEGIN
			INSERT INTO erv.tblTempTargetEntitySite
			(SiteGuid, MasterRecordGuid, EntityGuid, ParentEntityGuid, _CallingReferenceGuid)
			SELECT a.SiteGuid, a._MasterRecordGuid, a.PersonnelGuid, d.PersonnelGuid, @callingRefGuid
			FROM [dbo].[tblPersonnel] a
			INNER JOIN map.tblEntityPersonnelToSite b
			ON b.PersonnelGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN tblPersonnel d
			ON d._MasterRecordGuid = b.PersonnelGuid
			AND d.SiteGuid = b.AssignedFromSiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about updating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no child record version to update in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND a.PersonnelGuid <> a._MasterRecordGuid
		END											
		
		IF (NOT EXISTS (SELECT * FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid))
		BEGIN							
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblPersonnel table, and set the flag according to whether the field is VersionSpecific or not.
		INSERT INTO erv.tblTempPersonnelRecordVersioningFlag
		(PersonnelGuid, _CallingReferenceGuid)
		SELECT DISTINCT MasterRecordGuid, @callingRefGuid FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForSegment] @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid, NULL, @callingRefGuid

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  		

		-- Update all the internal ParentSpecific fields for all applicable sites and sitegroups
		UPDATE a
			SET	a.[Address1] = (CASE e.[Address1_RVFlag] WHEN 1 THEN a.[Address1] ELSE b.[Address1] END),
			a.[Address2] = (CASE e.[Address2_RVFlag] WHEN 1 THEN a.[Address2] ELSE b.[Address2] END),
			a.[AssignedEquipmentGuid] = (CASE e.[AssignedEquipmentGuid_RVFlag] WHEN 1 THEN a.[AssignedEquipmentGuid] ELSE b.[AssignedEquipmentGuid] END),
			a.[AssignmentDate] = (CASE e.[AssignmentDate_RVFlag] WHEN 1 THEN a.[AssignmentDate] ELSE b.[AssignmentDate] END),
			a.[BirthDate] = (CASE e.[BirthDate_RVFlag] WHEN 1 THEN a.[BirthDate] ELSE b.[BirthDate] END),
			a.[CardedIn] = (CASE e.[CardedIn_RVFlag] WHEN 1 THEN a.[CardedIn] ELSE b.[CardedIn] END),
			a.[CardNumber] = (CASE e.[CardNumber_RVFlag] WHEN 1 THEN a.[CardNumber] ELSE b.[CardNumber] END),
			a.[City] = (CASE e.[City_RVFlag] WHEN 1 THEN a.[City] ELSE b.[City] END),
			a.[CompanyGuid] = (CASE e.[CompanyGuid_RVFlag] WHEN 1 THEN a.[CompanyGuid] ELSE b.[CompanyGuid] END),
			a.[Country] = (CASE e.[Country_RVFlag] WHEN 1 THEN a.[Country] ELSE b.[Country] END),
			a.[Department] = (CASE e.[Department_RVFlag] WHEN 1 THEN a.[Department] ELSE b.[Department] END),
			a.[Email] = (CASE e.[Email_RVFlag] WHEN 1 THEN a.[Email] ELSE b.[Email] END),
			a.[FirstName] = (CASE e.[FirstName_RVFlag] WHEN 1 THEN a.[FirstName] ELSE b.[FirstName] END),
			a.[HiddenDate] = (CASE e.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[InhibitInactivityLockout] = (CASE e.[InhibitInactivityLockout_RVFlag] WHEN 1 THEN a.[InhibitInactivityLockout] ELSE b.[InhibitInactivityLockout] END),
			a.[LaborRate1] = (CASE e.[LaborRate1_RVFlag] WHEN 1 THEN a.[LaborRate1] ELSE b.[LaborRate1] END),
			a.[LaborRate2] = (CASE e.[LaborRate2_RVFlag] WHEN 1 THEN a.[LaborRate2] ELSE b.[LaborRate2] END),
			a.[LaborRate3] = (CASE e.[LaborRate3_RVFlag] WHEN 1 THEN a.[LaborRate3] ELSE b.[LaborRate3] END),
			a.[LaborRate4] = (CASE e.[LaborRate4_RVFlag] WHEN 1 THEN a.[LaborRate4] ELSE b.[LaborRate4] END),
			a.[LastActivityDate] = (CASE e.[LastActivityDate_RVFlag] WHEN 1 THEN a.[LastActivityDate] ELSE b.[LastActivityDate] END),
			a.[LastName] = (CASE e.[LastName_RVFlag] WHEN 1 THEN a.[LastName] ELSE b.[LastName] END),
			a.[LockedOut] = (CASE e.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE e.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE e.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[MiddleName] = (CASE e.[MiddleName_RVFlag] WHEN 1 THEN a.[MiddleName] ELSE b.[MiddleName] END),
			a.[OnFileSignature] = (CASE e.[OnFileSignature_RVFlag] WHEN 1 THEN a.[OnFileSignature] ELSE b.[OnFileSignature] END),
			a.[PayRate] = (CASE e.[PayRate_RVFlag] WHEN 1 THEN a.[PayRate] ELSE b.[PayRate] END),
			a.[PersonID] = (CASE e.[PersonID_RVFlag] WHEN 1 THEN a.[PersonID] ELSE b.[PersonID] END),
			a.[Phone1] = (CASE e.[Phone1_RVFlag] WHEN 1 THEN a.[Phone1] ELSE b.[Phone1] END),
			a.[Phone2] = (CASE e.[Phone2_RVFlag] WHEN 1 THEN a.[Phone2] ELSE b.[Phone2] END),
			a.[PINNumber] = (CASE e.[PINNumber_RVFlag] WHEN 1 THEN a.[PINNumber] ELSE b.[PINNumber] END),
			a.[PINRequired] = (CASE e.[PINRequired_RVFlag] WHEN 1 THEN a.[PINRequired] ELSE b.[PINRequired] END),
			a.[ResponsibleOfficer] = (CASE e.[ResponsibleOfficer_RVFlag] WHEN 1 THEN a.[ResponsibleOfficer] ELSE b.[ResponsibleOfficer] END),
			a.[Shift] = (CASE e.[Shift_RVFlag] WHEN 1 THEN a.[Shift] ELSE b.[Shift] END),
			a.[ShortCardNumber] = (CASE e.[ShortCardNumber_RVFlag] WHEN 1 THEN a.[ShortCardNumber] ELSE b.[ShortCardNumber] END),
			a.[SSAN] = (CASE e.[SSAN_RVFlag] WHEN 1 THEN a.[SSAN] ELSE b.[SSAN] END),
			a.[State] = (CASE e.[State_RVFlag] WHEN 1 THEN a.[State] ELSE b.[State] END),
			a.[Status] = (CASE e.[Status_RVFlag] WHEN 1 THEN a.[Status] ELSE b.[Status] END),
			a.[SupervisionDate] = (CASE e.[SupervisionDate_RVFlag] WHEN 1 THEN a.[SupervisionDate] ELSE b.[SupervisionDate] END),
			a.[SupervisorPersonnelGuid] = (CASE e.[SupervisorPersonnelGuid_RVFlag] WHEN 1 THEN a.[SupervisorPersonnelGuid] ELSE b.[SupervisorPersonnelGuid] END),
			a.[Title] = (CASE e.[Title_RVFlag] WHEN 1 THEN a.[Title] ELSE b.[Title] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE e.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData10] = (CASE e.[UserData10_RVFlag] WHEN 1 THEN a.[UserData10] ELSE b.[UserData10] END),
			a.[UserData11] = (CASE e.[UserData11_RVFlag] WHEN 1 THEN a.[UserData11] ELSE b.[UserData11] END),
			a.[UserData12] = (CASE e.[UserData12_RVFlag] WHEN 1 THEN a.[UserData12] ELSE b.[UserData12] END),
			a.[UserData13] = (CASE e.[UserData13_RVFlag] WHEN 1 THEN a.[UserData13] ELSE b.[UserData13] END),
			a.[UserData14] = (CASE e.[UserData14_RVFlag] WHEN 1 THEN a.[UserData14] ELSE b.[UserData14] END),
			a.[UserData15] = (CASE e.[UserData15_RVFlag] WHEN 1 THEN a.[UserData15] ELSE b.[UserData15] END),
			a.[UserData16] = (CASE e.[UserData16_RVFlag] WHEN 1 THEN a.[UserData16] ELSE b.[UserData16] END),
			a.[UserData17] = (CASE e.[UserData17_RVFlag] WHEN 1 THEN a.[UserData17] ELSE b.[UserData17] END),
			a.[UserData18] = (CASE e.[UserData18_RVFlag] WHEN 1 THEN a.[UserData18] ELSE b.[UserData18] END),
			a.[UserData19] = (CASE e.[UserData19_RVFlag] WHEN 1 THEN a.[UserData19] ELSE b.[UserData19] END),
			a.[UserData2] = (CASE e.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData20] = (CASE e.[UserData20_RVFlag] WHEN 1 THEN a.[UserData20] ELSE b.[UserData20] END),
			a.[UserData21] = (CASE e.[UserData21_RVFlag] WHEN 1 THEN a.[UserData21] ELSE b.[UserData21] END),
			a.[UserData22] = (CASE e.[UserData22_RVFlag] WHEN 1 THEN a.[UserData22] ELSE b.[UserData22] END),
			a.[UserData23] = (CASE e.[UserData23_RVFlag] WHEN 1 THEN a.[UserData23] ELSE b.[UserData23] END),
			a.[UserData24] = (CASE e.[UserData24_RVFlag] WHEN 1 THEN a.[UserData24] ELSE b.[UserData24] END),
			a.[UserData3] = (CASE e.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE e.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE e.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE e.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE e.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE e.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[UserData9] = (CASE e.[UserData9_RVFlag] WHEN 1 THEN a.[UserData9] ELSE b.[UserData9] END),
			a.[UserGuid] = (CASE e.[UserGuid_RVFlag] WHEN 1 THEN a.[UserGuid] ELSE b.[UserGuid] END),
			a.[Zip] = (CASE e.[Zip_RVFlag] WHEN 1 THEN a.[Zip] ELSE b.[Zip] END)			
		FROM tblPersonnel a
		INNER JOIN tblPersonnel b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTargetEntitySite c
		ON c.EntityGuid = a.PersonnelGuid
		INNER JOIN erv.tblTempTargetEntitySite d
		ON d.ParentEntityGuid = b.PersonnelGuid
		INNER JOIN erv.tblTempPersonnelRecordVersioningFlag e
		ON e.PersonnelGuid = a._MasterRecordGuid
		WHERE e._CallingReferenceGuid = @callingRefGuid
		AND c._CallingReferenceGuid = @callingRefGuid
		AND d._CallingReferenceGuid = @callingRefGuid

		DELETE erv.tblTempPersonnelRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRefGuid 

		-- Process those ParentSpecific External fields whose propagation require custom handling.
		DECLARE @tblParentSpecificExternalFields TABLE
		(
			TargetField nvarchar(100)
		)

		/*Process those ParentSpecific External fields whose propagation require custom handling. */

		-- Process [Roles] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Roles') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblPersonnelToRole] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblPersonnelToRole] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)	
			AND b._CallingReferenceGuid = @callingRefGuid
													
			--No characteristics of the PersonnelToRole mappings to update. The mappings are either inserted or deleted.

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblPersonnelToRole]
			(PersonnelGuid, LookupPersonnelRoleIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.LookupPersonnelRoleIndex, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblPersonnelToRole] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblPersonnelToRole] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid		
		END

		-- Process [AccessSchedule] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AccessSchedule') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Enabled = d.Enabled, 
			a.OpeningTime = d.OpeningTime,
			a.ClosingTime = d.ClosingTime,
			a.EndOfDayEnabled = d.EndOfDayEnabled,
			a.EndOfDayTime = d.EndOfDayTime,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN [dbo].[tblSchedulePersonnelAccess] d
			ON d.PersonnelGuid = b.ParentEntityGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [dbo].[tblSchedulePersonnelAccess]
			(PersonnelGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.LookupDayOfWeekIndex, 
			 a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Qualifications] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Qualifications') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the active (non-historical) attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.id = d.id,		
			a.Instructor = d.Instructor,
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonQualificationToPerson] d
			ON d.PersonnelGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord			
			WHERE b._CallingReferenceGuid = @callingRefGuid
			AND a.HistoricalRecord = 0

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationPersonQualificationToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.QualificationGuid
			 ,a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			INNER JOIN tblQualifications c
			ON c.QualificationGuid = a.QualificationGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Training] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Training') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the active (non-historical) attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.id = d.id,		
			a.Instructor = d.Instructor,
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonTrainingToPerson] d
			ON d.PersonnelGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord
			WHERE b._CallingReferenceGuid = @callingRefGuid
			AND a.HistoricalRecord = 0

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationPersonTrainingToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.QualificationGuid
			 ,a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			INNER JOIN tblQualifications c
			ON c.QualificationGuid = a.QualificationGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END


		-- Process [Licenses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Licenses') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the active (non-historical) attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.id = d.id,		
			a.Instructor = d.Instructor,
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonLicenseToPerson] d
			ON d.PersonnelGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord
			WHERE b._CallingReferenceGuid = @callingRefGuid
			AND a.HistoricalRecord = 0

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationPersonLicenseToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.QualificationGuid
			 ,a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			INNER JOIN tblQualifications c
			ON c.QualificationGuid = a.QualificationGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Carrier] External Field
		-- Company is both an External Attribute of Personnel (i.e. Company-To-Personnel mappings are maintained as part of the Personnel entity), and an External Client of Personnel (i.e. Company-To-Personnel mappings are also maintained as part of the Company entity, i.e. outside of the Personnel entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Carrier') = 0)
		BEGIN
			--Only delete the child record version Company mappings that are not supported anymore in the parent Personnel and that are not tied to a local Company or a Company child record version whose mappings to Personnel is VersionSpecific (so that the local Company or the Company child record version does not loose its Personnel mappings when Personnel RecordVersioning is turned off).
			DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Company'
				AND TargetField = 'Drivers'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid			
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.CompanyGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Company or by a Company child record version whose Drivers field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d				
				WHERE d.PersonnelGuid = b.ParentEntityGuid
				AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid	
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.ID = d.ID,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.PersonnelGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN [map].[tblCompanyPersonnelAssignedToCompany] d
			ON d.PersonnelGuid = b.ParentEntityGuid
			AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
			(CompanyGuid, PersonnelGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), 
			 b.EntityGuid, b.SiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.PersonnelGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d
				WHERE d.PersonnelGuid = b.EntityGuid
				AND d.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		DELETE erv.tblTempTargetEntitySite
		WHERE _CallingReferenceGuid = @callingRefGuid

		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateToChildRecordVersions
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateToChildRecordVersions
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
						+ 'Procedure Name: [erv].usp_PropagatePersonnelRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
