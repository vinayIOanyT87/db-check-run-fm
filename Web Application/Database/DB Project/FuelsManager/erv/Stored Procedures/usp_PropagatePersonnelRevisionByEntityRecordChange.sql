/*
	DROP PROCEDURE [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange]

	EXEC [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange] '0DC68ACA-11AD-4F43-AD2B-87609738C453'
*/

CREATE PROCEDURE [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange]
(
	@SourcePersonnelGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagatePersonnelRevisionByEntityRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the current revision of a given Personnel entity record down the site hierarchy, according to the rules established by the Field Level Control configurations.
	-- This Stored Procedure is to be used to propagate the effect of an entity record change down to all its children record versions.
	-- Notes:
	-- 1. @SourcePersonnelGuid: Guid of the Personnel record that needs to be propagated down the site hierarchy. This should correspond to the exact record version that has been 
	--    changed (and not the parent record of the entity record).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Personnel'

		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblPersonnel
		WHERE PersonnelGuid = @SourcePersonnelGuid

		IF ((@masterRecordGuid IS NULL) OR (@ownerSiteGuid IS NULL))
		BEGIN
			RAISERROR('Cannot locate the source record for data propagation.',16,1); 
			RETURN;
		END

		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		
		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourcePersonnelGuid)
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END

		DECLARE @assignedFromSiteGroupGuid uniqueidentifier
		IF (@SourcePersonnelGuid = @masterRecordGuid)
		BEGIN
			SET @assignedFromSiteGroupGuid = @ownerSiteGuid
		END
		ELSE
		BEGIN
			SET @assignedFromSiteGroupGuid = (SELECT [erv].[udf_GetEntityAssignedFromSite] (@EntityTypeId, @SourcePersonnelGuid, Null))
			IF (@assignedFromSiteGroupGuid IS NULL)
			BEGIN
				RAISERROR('Cannot locate the assignment information for the selected child record version.',16,1); 
				RETURN;
			END
		END				

		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose changes are to be propagated
		-- This corresponds to all the child record versions who derives, directly or indirectly, from the given record version.
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, 0
		FROM [erv].[udf_GetPersonnelToSiteHierarchyByRecordVersionGuid](@SourcePersonnelGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid


		--Retrieve the VersionSpecific fields for the owner sitegroup of the entity record whose changes need to be propagated
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, 'VersionSpecific', @callingRef2Guid 

		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName)
		SELECT TargetField, IsExternalAttribute, InternalFieldName FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid


		--Build a table that has one flag column for each column of the tblPersonnel table, and set the flag according to whether the field is VersionSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempPersonnelRecordVersioningFlag
		(PersonnelGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.PersonnelGuid, a.SiteGuid, @callingRef1Guid FROM tblPersonnel a
		INNER JOIN @tblEntityToSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @masterRecordGuid

		DECLARE @tblTargetChildRecordVersions TABLE
		(
			PersonnelGuid uniqueidentifier,
			SiteGuid uniqueidentifier,
			HierarchyLevel int,
			Processed bit
		)

		INSERT INTO @tblTargetChildRecordVersions
		(PersonnelGuid, SiteGuid, HierarchyLevel, Processed)
		SELECT a.PersonnelGuid, b.SiteGuid, c.HierarchyLevel, 0 FROM erv.tblTempPersonnelRecordVersioningFlag a
		INNER JOIN tblPersonnel b
		ON b.PersonnelGuid = a.PersonnelGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON c.SiteGuid = b.SiteGuid
		WHERE b._MasterRecordGuid = @masterRecordGuid
		AND a._CallingReferenceGuid = @callingRef1Guid


		IF (NOT EXISTS (SELECT * FROM erv.tblTempPersonnelRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRef1Guid))
		BEGIN				
			/*	No child record versions to update.	*/
			RETURN;
		END

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  	
		

		-- Update all the internal non-VersionSpecific fields for all applicable child record versions
		UPDATE a
		SET	a.[Address1] = (CASE d.[Address1_RVFlag] WHEN 1 THEN a.[Address1] ELSE b.[Address1] END),
		a.[Address2] = (CASE d.[Address2_RVFlag] WHEN 1 THEN a.[Address2] ELSE b.[Address2] END),
		a.[AssignedEquipmentGuid] = (CASE d.[AssignedEquipmentGuid_RVFlag] WHEN 1 THEN a.[AssignedEquipmentGuid] ELSE b.[AssignedEquipmentGuid] END),
		a.[AssignmentDate] = (CASE d.[AssignmentDate_RVFlag] WHEN 1 THEN a.[AssignmentDate] ELSE b.[AssignmentDate] END),
		a.[BirthDate] = (CASE d.[BirthDate_RVFlag] WHEN 1 THEN a.[BirthDate] ELSE b.[BirthDate] END),
		a.[CardedIn] = (CASE d.[CardedIn_RVFlag] WHEN 1 THEN a.[CardedIn] ELSE b.[CardedIn] END),
		a.[CardNumber] = (CASE d.[CardNumber_RVFlag] WHEN 1 THEN a.[CardNumber] ELSE b.[CardNumber] END),
		a.[City] = (CASE d.[City_RVFlag] WHEN 1 THEN a.[City] ELSE b.[City] END),
		a.[CompanyGuid] = (CASE d.[CompanyGuid_RVFlag] WHEN 1 THEN a.[CompanyGuid] ELSE b.[CompanyGuid] END),
		a.[Country] = (CASE d.[Country_RVFlag] WHEN 1 THEN a.[Country] ELSE b.[Country] END),
		a.[Department] = (CASE d.[Department_RVFlag] WHEN 1 THEN a.[Department] ELSE b.[Department] END),
		a.[Email] = (CASE d.[Email_RVFlag] WHEN 1 THEN a.[Email] ELSE b.[Email] END),
		a.[FirstName] = (CASE d.[FirstName_RVFlag] WHEN 1 THEN a.[FirstName] ELSE b.[FirstName] END),
		a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
		a.[InhibitInactivityLockout] = (CASE d.[InhibitInactivityLockout_RVFlag] WHEN 1 THEN a.[InhibitInactivityLockout] ELSE b.[InhibitInactivityLockout] END),	
		a.[LaborRate1] = (CASE d.[LaborRate1_RVFlag] WHEN 1 THEN a.[LaborRate1] ELSE b.[LaborRate1] END),
		a.[LaborRate2] = (CASE d.[LaborRate2_RVFlag] WHEN 1 THEN a.[LaborRate2] ELSE b.[LaborRate2] END),
		a.[LaborRate3] = (CASE d.[LaborRate3_RVFlag] WHEN 1 THEN a.[LaborRate3] ELSE b.[LaborRate3] END),
		a.[LaborRate4] = (CASE d.[LaborRate4_RVFlag] WHEN 1 THEN a.[LaborRate4] ELSE b.[LaborRate4] END),
		a.[LastActivityDate] = (CASE d.[LastActivityDate_RVFlag] WHEN 1 THEN a.[LastActivityDate] ELSE b.[LastActivityDate] END),
		a.[LastName] = (CASE d.[LastName_RVFlag] WHEN 1 THEN a.[LastName] ELSE b.[LastName] END),
		a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
		a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
		a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
		a.[MiddleName] = (CASE d.[MiddleName_RVFlag] WHEN 1 THEN a.[MiddleName] ELSE b.[MiddleName] END),
		a.[OnFileSignature] = (CASE d.[OnFileSignature_RVFlag] WHEN 1 THEN a.[OnFileSignature] ELSE b.[OnFileSignature] END),
		a.[PayRate] = (CASE d.[PayRate_RVFlag] WHEN 1 THEN a.[PayRate] ELSE b.[PayRate] END),
		a.[PersonID] = (CASE d.[PersonID_RVFlag] WHEN 1 THEN a.[PersonID] ELSE b.[PersonID] END),
		a.[Phone1] = (CASE d.[Phone1_RVFlag] WHEN 1 THEN a.[Phone1] ELSE b.[Phone1] END),
		a.[Phone2] = (CASE d.[Phone2_RVFlag] WHEN 1 THEN a.[Phone2] ELSE b.[Phone2] END),
		a.[PINNumber] = (CASE d.[PINNumber_RVFlag] WHEN 1 THEN a.[PINNumber] ELSE b.[PINNumber] END),
		a.[PINRequired] = (CASE d.[PINRequired_RVFlag] WHEN 1 THEN a.[PINRequired] ELSE b.[PINRequired] END),
		a.[ResponsibleOfficer] = (CASE d.[ResponsibleOfficer_RVFlag] WHEN 1 THEN a.[ResponsibleOfficer] ELSE b.[ResponsibleOfficer] END),
		a.[Shift] = (CASE d.[Shift_RVFlag] WHEN 1 THEN a.[Shift] ELSE b.[Shift] END),
		a.[ShortCardNumber] = (CASE d.[ShortCardNumber_RVFlag] WHEN 1 THEN a.[ShortCardNumber] ELSE b.[ShortCardNumber] END),
		a.[SSAN] = (CASE d.[SSAN_RVFlag] WHEN 1 THEN a.[SSAN] ELSE b.[SSAN] END),
		a.[State] = (CASE d.[State_RVFlag] WHEN 1 THEN a.[State] ELSE b.[State] END),
		a.[Status] = (CASE d.[Status_RVFlag] WHEN 1 THEN a.[Status] ELSE b.[Status] END),
		a.[SupervisionDate] = (CASE d.[SupervisionDate_RVFlag] WHEN 1 THEN a.[SupervisionDate] ELSE b.[SupervisionDate] END),
		a.[SupervisorPersonnelGuid] = (CASE d.[SupervisorPersonnelGuid_RVFlag] WHEN 1 THEN a.[SupervisorPersonnelGuid] ELSE b.[SupervisorPersonnelGuid] END),
		a.[Title] = (CASE d.[Title_RVFlag] WHEN 1 THEN a.[Title] ELSE b.[Title] END),
		a.[UpdatedBy] = b.[UpdatedBy],
		a.[UpdatedDate] = SYSDATETIMEOFFSET(),
		a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
		a.[UserData10] = (CASE d.[UserData10_RVFlag] WHEN 1 THEN a.[UserData10] ELSE b.[UserData10] END),
		a.[UserData11] = (CASE d.[UserData11_RVFlag] WHEN 1 THEN a.[UserData11] ELSE b.[UserData11] END),
		a.[UserData12] = (CASE d.[UserData12_RVFlag] WHEN 1 THEN a.[UserData12] ELSE b.[UserData12] END),
		a.[UserData13] = (CASE d.[UserData13_RVFlag] WHEN 1 THEN a.[UserData13] ELSE b.[UserData13] END),
		a.[UserData14] = (CASE d.[UserData14_RVFlag] WHEN 1 THEN a.[UserData14] ELSE b.[UserData14] END),
		a.[UserData15] = (CASE d.[UserData15_RVFlag] WHEN 1 THEN a.[UserData15] ELSE b.[UserData15] END),
		a.[UserData16] = (CASE d.[UserData16_RVFlag] WHEN 1 THEN a.[UserData16] ELSE b.[UserData16] END),
		a.[UserData17] = (CASE d.[UserData17_RVFlag] WHEN 1 THEN a.[UserData17] ELSE b.[UserData17] END),
		a.[UserData18] = (CASE d.[UserData18_RVFlag] WHEN 1 THEN a.[UserData18] ELSE b.[UserData18] END),
		a.[UserData19] = (CASE d.[UserData19_RVFlag] WHEN 1 THEN a.[UserData19] ELSE b.[UserData19] END),
		a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
		a.[UserData20] = (CASE d.[UserData20_RVFlag] WHEN 1 THEN a.[UserData20] ELSE b.[UserData20] END),
		a.[UserData21] = (CASE d.[UserData21_RVFlag] WHEN 1 THEN a.[UserData21] ELSE b.[UserData21] END),
		a.[UserData22] = (CASE d.[UserData22_RVFlag] WHEN 1 THEN a.[UserData22] ELSE b.[UserData22] END),
		a.[UserData23] = (CASE d.[UserData23_RVFlag] WHEN 1 THEN a.[UserData23] ELSE b.[UserData23] END),
		a.[UserData24] = (CASE d.[UserData24_RVFlag] WHEN 1 THEN a.[UserData24] ELSE b.[UserData24] END),
		a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
		a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
		a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
		a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
		a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
		a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
		a.[UserData9] = (CASE d.[UserData9_RVFlag] WHEN 1 THEN a.[UserData9] ELSE b.[UserData9] END),
		a.[UserGuid] = (CASE d.[UserGuid_RVFlag] WHEN 1 THEN a.[UserGuid] ELSE b.[UserGuid] END),
		a.[Zip] = (CASE d.[Zip_RVFlag] WHEN 1 THEN a.[Zip] ELSE b.[Zip] END)		
		FROM tblPersonnel a
		INNER JOIN tblPersonnel b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON a.SiteGuid = c.SiteGuid
		INNER JOIN erv.tblTempPersonnelRecordVersioningFlag d
		ON d.PersonnelGuid = a.PersonnelGuid
		WHERE b.PersonnelGuid = @SourcePersonnelGuid
		AND d._CallingReferenceGuid = @callingRef1Guid

		DELETE erv.tblTempPersonnelRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 

		
		/*Process those non-VersionSpecific External fields whose propagation require custom handling. */	
		-- Process [Roles] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Roles') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Personnel
			DELETE a FROM [map].[tblPersonnelToRole] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblPersonnelToRole] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)		
													
			--No characteristics of the PersonnelToRole mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblPersonnelToRole]
			(LookupPersonnelRoleIndex, PersonnelGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.LookupPersonnelRoleIndex, b.PersonnelGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblPersonnelToRole] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblPersonnelToRole] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)
		END
		

		-- Process [AccessSchedule] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AccessSchedule') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Personnel
			DELETE a FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)		
								
			--Update the child record version mappings that have been modified in the parent Personnel
			UPDATE d
			SET d.Enabled = a.Enabled, 
			d.OpeningTime = a.OpeningTime,
			d.ClosingTime = a.ClosingTime,
			d.EndOfDayEnabled = a.EndOfDayEnabled,
			d.EndOfDayTime = a.EndOfDayTime,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblSchedulePersonnelAccess] d
			ON d.PersonnelGuid = b.PersonnelGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [dbo].[tblSchedulePersonnelAccess]
			(PersonnelGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.PersonnelGuid, a.LookupDayOfWeekIndex, a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
		END


		-- Process [Qualifications] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Qualifications') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Personnel
			DELETE a FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)		
													
			--Update the active (non-historical) child record version mappings that have been modified in the parent Personnel
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblQualificationPersonQualificationToPerson] d
			ON d.PersonnelGuid = b.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord			
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND d.HistoricalRecord = 0

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblQualificationPersonQualificationToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.PersonnelGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		


		-- Process [Training] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Training') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Personnel
			DELETE a FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)		
													
			--Update the active (non-historical) child record version mappings that have been modified in the parent Personnel
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblQualificationPersonTrainingToPerson] d
			ON d.PersonnelGuid = b.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord			
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND d.HistoricalRecord = 0

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblQualificationPersonTrainingToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.PersonnelGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		


		-- Process [Licenses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Licenses') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Personnel
			DELETE a FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)		
										
			--Update the active (non-historical) child record version mappings that have been modified in the parent Personnel
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblQualificationPersonLicenseToPerson] d
			ON d.PersonnelGuid = b.PersonnelGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord			
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND d.HistoricalRecord = 0

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblQualificationPersonLicenseToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.PersonnelGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		

		-- Process [Carrier] External Field
		-- Company is both an External Attribute of Personnel (i.e. Company-To-Product mappings are maintained as part of the Personnel entity), and an External Client of Personnel (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Personnel entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Carrier') = 0)
		BEGIN
			--Only delete the child record version Company mappings that are not supported anymore in the parent Personnel and that are not tied to a local Company or a Company child record version whose mappings to Personnel is VersionSpecific (so that the local Company or the Company child record version does not loose its Personnel mappings when Personnel RecordVersioning is turned off).
			DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.PersonnelGuid = a.PersonnelGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid= a.CompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid= c._MasterRecordGuid
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same Company record
					c.SiteGuid = b.SiteGuid
					AND c.CompanyGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same Company master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.CompanyGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d				
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid)
			)				
													
			--Update the child record version mappings that have been modified in the parent Personnel
			UPDATE d
			SET d.ID = a.ID,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN [map].[tblCompanyPersonnelAssignedToCompany] d
			ON d.PersonnelGuid = b.PersonnelGuid
			AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
			(CompanyGuid, PersonnelGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), 
			b.PersonnelGuid, b.SiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d
				WHERE d.PersonnelGuid = b.PersonnelGuid
				AND d.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)
			)
		END
	
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
						+ 'Procedure Name: [erv].usp_PropagatePersonnelRevisionByEntityRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
