/*
	DROP PROCEDURE [erv].[usp_ReplicatePersonnelGSChangesOnMaster]

	EXEC [erv].[usp_ReplicatePersonnelGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_ReplicatePersonnelGSChangesOnMaster] '0DC68ACA-11AD-4F43-AD2B-87609738C453'
*/

CREATE PROCEDURE [erv].[usp_ReplicatePersonnelGSChangesOnMaster]
(
	@SourcePersonnelGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ReplicatePersonnelGSChangesOnMaster] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Replicate the Global Specific field values of a Personnel child record version onto the Master Record copy.
	--          By replicating those field values onto the master record, we ensure that when the non-VersionSpecific
	--          fields of the master record are propagated down the site hierarchy, that all the GlobalSpecific changes made onto the
	--          the child record version will get propagated onto all the sitegroups and sites where the master record is assigned.
	-- Notes:
	-- 1. @SourcePersonnelGuid: Guid of the Personnel child record version record whose GlobalSpecific fields needs to be replicated to its local Master Record copy 
	--    (and not the parent record of the entity record).
	-- 2. Whereas RecordVersioning propagation is limited to child record versions, the GlobalSpecific field replication targets the master records and allows
	--    modifications to the master records. This also applies to external attributres that represent a reference to another RecordVersioning entity (e.g. Product).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Personnel'

		DECLARE @masterSiteGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		DECLARE @assignedFromSiteGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid FROM dbo.tblPersonnel
		WHERE PersonnelGuid = @SourcePersonnelGuid
		AND PersonnelGuid <> _MasterRecordGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source child record for data replication.',16,1); 
			RETURN;
		END

		IF ((SELECT COUNT(*) FROM dbo.tblPersonnel WHERE PersonnelGuid = @masterRecordGuid AND _MasterRecordGuid = @masterRecordGuid) = 0)
		BEGIN
			RAISERROR('Cannot locate the target master record for data replication.',16,1); 
			RETURN;
		END

		SELECT @masterSiteGuid = SiteGuid FROM dbo.tblPersonnel
		WHERE PersonnelGuid = @masterRecordGuid
		AND PersonnelGuid = _MasterRecordGuid

		SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityPersonnelToSite 
		WHERE PersonnelGuid = @masterRecordGuid 
		AND SiteGuid = @ownerSiteGuid


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
		

		--Retrieve the GlobalSpecific fields for the AssignedFrom sitegroup of the child record version whose changes need to be replicated
		DECLARE @tblSourceGlobalSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, 'GlobalSpecific', @callingRef2Guid 

		INSERT @tblSourceGlobalSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid

		IF (NOT EXISTS (SELECT * FROM @tblSourceGlobalSpecificFields))
		BEGIN				
			/*	No GlobalSpecific fields to update.	*/
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblPersonnel table, and set the flag according to whether the field is GlobalSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempPersonnelRecordVersioningFlag
		(PersonnelGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.PersonnelGuid, a.SiteGuid, @callingRef1Guid FROM tblPersonnel a
		WHERE a._MasterRecordGuid = @masterRecordGuid
		AND a.PersonnelGuid = a._MasterRecordGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --ReplicateToMasterRecord
            SET @BeginTran = 1   
		END  	
		

		-- Update all the GlobalSpecific fields of the Master record version
		UPDATE a
		SET	a.[Address1] = (CASE d.[Address1_RVFlag] WHEN 1 THEN b.[Address1] ELSE a.[Address1] END),
		a.[Address2] = (CASE d.[Address2_RVFlag] WHEN 1 THEN b.[Address2] ELSE a.[Address2] END),
		a.[AssignedEquipmentGuid] = (CASE d.[AssignedEquipmentGuid_RVFlag] WHEN 1 THEN b.[AssignedEquipmentGuid] ELSE a.[AssignedEquipmentGuid] END),
		a.[AssignmentDate] = (CASE d.[AssignmentDate_RVFlag] WHEN 1 THEN b.[AssignmentDate] ELSE a.[AssignmentDate] END),
		a.[BirthDate] = (CASE d.[BirthDate_RVFlag] WHEN 1 THEN b.[BirthDate] ELSE a.[BirthDate] END),
		a.[CardedIn] = (CASE d.[CardedIn_RVFlag] WHEN 1 THEN b.[CardedIn] ELSE a.[CardedIn] END),
		a.[CardNumber] = (CASE d.[CardNumber_RVFlag] WHEN 1 THEN b.[CardNumber] ELSE a.[CardNumber] END),
		a.[City] = (CASE d.[City_RVFlag] WHEN 1 THEN b.[City] ELSE a.[City] END),
		a.[CompanyGuid] = (CASE d.[CompanyGuid_RVFlag] WHEN 1 THEN b.[CompanyGuid] ELSE a.[CompanyGuid] END),
		a.[Country] = (CASE d.[Country_RVFlag] WHEN 1 THEN b.[Country] ELSE a.[Country] END),
		a.[Department] = (CASE d.[Department_RVFlag] WHEN 1 THEN b.[Department] ELSE a.[Department] END),
		a.[Email] = (CASE d.[Email_RVFlag] WHEN 1 THEN b.[Email] ELSE a.[Email] END),
		a.[FirstName] = (CASE d.[FirstName_RVFlag] WHEN 1 THEN b.[FirstName] ELSE a.[FirstName] END),
		a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN b.[HiddenDate] ELSE a.[HiddenDate] END),
		a.[InhibitInactivityLockout] = (CASE d.[InhibitInactivityLockout_RVFlag] WHEN 1 THEN b.[InhibitInactivityLockout] ELSE a.[InhibitInactivityLockout] END),
		a.[LaborRate1] = (CASE d.[LaborRate1_RVFlag] WHEN 1 THEN b.[LaborRate1] ELSE a.[LaborRate1] END),
		a.[LaborRate2] = (CASE d.[LaborRate2_RVFlag] WHEN 1 THEN b.[LaborRate2] ELSE a.[LaborRate2] END),
		a.[LaborRate3] = (CASE d.[LaborRate3_RVFlag] WHEN 1 THEN b.[LaborRate3] ELSE a.[LaborRate3] END),
		a.[LaborRate4] = (CASE d.[LaborRate4_RVFlag] WHEN 1 THEN b.[LaborRate4] ELSE a.[LaborRate4] END),
		a.[LastActivityDate] = (CASE d.[LastActivityDate_RVFlag] WHEN 1 THEN b.[LastActivityDate] ELSE a.[LastActivityDate] END),
		a.[LastName] = (CASE d.[LastName_RVFlag] WHEN 1 THEN b.[LastName] ELSE a.[LastName] END),
		a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN b.[LockedOut] ELSE a.[LockedOut] END),
		a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN b.[LockedOutDate] ELSE a.[LockedOutDate] END),
		a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN b.[LockedOutReason] ELSE a.[LockedOutReason] END),
		a.[MiddleName] = (CASE d.[MiddleName_RVFlag] WHEN 1 THEN b.[MiddleName] ELSE a.[MiddleName] END),
		a.[OnFileSignature] = (CASE d.[OnFileSignature_RVFlag] WHEN 1 THEN b.[OnFileSignature] ELSE a.[OnFileSignature] END),
		a.[PayRate] = (CASE d.[PayRate_RVFlag] WHEN 1 THEN b.[PayRate] ELSE a.[PayRate] END),
		a.[PersonID] = (CASE d.[PersonID_RVFlag] WHEN 1 THEN b.[PersonID] ELSE a.[PersonID] END),
		a.[Phone1] = (CASE d.[Phone1_RVFlag] WHEN 1 THEN b.[Phone1] ELSE a.[Phone1] END),
		a.[Phone2] = (CASE d.[Phone2_RVFlag] WHEN 1 THEN b.[Phone2] ELSE a.[Phone2] END),
		a.[PINNumber] = (CASE d.[PINNumber_RVFlag] WHEN 1 THEN b.[PINNumber] ELSE a.[PINNumber] END),
		a.[PINRequired] = (CASE d.[PINRequired_RVFlag] WHEN 1 THEN b.[PINRequired] ELSE a.[PINRequired] END),
		a.[ResponsibleOfficer] = (CASE d.[ResponsibleOfficer_RVFlag] WHEN 1 THEN b.[ResponsibleOfficer] ELSE a.[ResponsibleOfficer] END),
		a.[Shift] = (CASE d.[Shift_RVFlag] WHEN 1 THEN b.[Shift] ELSE a.[Shift] END),
		a.[ShortCardNumber] = (CASE d.[ShortCardNumber_RVFlag] WHEN 1 THEN b.[ShortCardNumber] ELSE a.[ShortCardNumber] END),
		a.[SSAN] = (CASE d.[SSAN_RVFlag] WHEN 1 THEN b.[SSAN] ELSE a.[SSAN] END),
		a.[State] = (CASE d.[State_RVFlag] WHEN 1 THEN b.[State] ELSE a.[State] END),
		a.[Status] = (CASE d.[Status_RVFlag] WHEN 1 THEN b.[Status] ELSE a.[Status] END),
		a.[SupervisionDate] = (CASE d.[SupervisionDate_RVFlag] WHEN 1 THEN b.[SupervisionDate] ELSE a.[SupervisionDate] END),
		a.[SupervisorPersonnelGuid] = (CASE d.[SupervisorPersonnelGuid_RVFlag] WHEN 1 THEN b.[SupervisorPersonnelGuid] ELSE a.[SupervisorPersonnelGuid] END),
		a.[Title] = (CASE d.[Title_RVFlag] WHEN 1 THEN b.[Title] ELSE a.[Title] END),
		a.[UpdatedBy] = b.[UpdatedBy],
		a.[UpdatedDate] = SYSDATETIMEOFFSET(),
		a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN b.[UserData1] ELSE a.[UserData1] END),
		a.[UserData10] = (CASE d.[UserData10_RVFlag] WHEN 1 THEN b.[UserData10] ELSE a.[UserData10] END),
		a.[UserData11] = (CASE d.[UserData11_RVFlag] WHEN 1 THEN b.[UserData11] ELSE a.[UserData11] END),
		a.[UserData12] = (CASE d.[UserData12_RVFlag] WHEN 1 THEN b.[UserData12] ELSE a.[UserData12] END),
		a.[UserData13] = (CASE d.[UserData13_RVFlag] WHEN 1 THEN b.[UserData13] ELSE a.[UserData13] END),
		a.[UserData14] = (CASE d.[UserData14_RVFlag] WHEN 1 THEN b.[UserData14] ELSE a.[UserData14] END),
		a.[UserData15] = (CASE d.[UserData15_RVFlag] WHEN 1 THEN b.[UserData15] ELSE a.[UserData15] END),
		a.[UserData16] = (CASE d.[UserData16_RVFlag] WHEN 1 THEN b.[UserData16] ELSE a.[UserData16] END),
		a.[UserData17] = (CASE d.[UserData17_RVFlag] WHEN 1 THEN b.[UserData17] ELSE a.[UserData17] END),
		a.[UserData18] = (CASE d.[UserData18_RVFlag] WHEN 1 THEN b.[UserData18] ELSE a.[UserData18] END),
		a.[UserData19] = (CASE d.[UserData19_RVFlag] WHEN 1 THEN b.[UserData19] ELSE a.[UserData19] END),
		a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN b.[UserData2] ELSE a.[UserData2] END),
		a.[UserData20] = (CASE d.[UserData20_RVFlag] WHEN 1 THEN b.[UserData20] ELSE a.[UserData20] END),
		a.[UserData21] = (CASE d.[UserData21_RVFlag] WHEN 1 THEN b.[UserData21] ELSE a.[UserData21] END),
		a.[UserData22] = (CASE d.[UserData22_RVFlag] WHEN 1 THEN b.[UserData22] ELSE a.[UserData22] END),
		a.[UserData23] = (CASE d.[UserData23_RVFlag] WHEN 1 THEN b.[UserData23] ELSE a.[UserData23] END),
		a.[UserData24] = (CASE d.[UserData24_RVFlag] WHEN 1 THEN b.[UserData24] ELSE a.[UserData24] END),
		a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN b.[UserData3] ELSE a.[UserData3] END),
		a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN b.[UserData4] ELSE a.[UserData4] END),
		a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN b.[UserData5] ELSE a.[UserData5] END),
		a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN b.[UserData6] ELSE a.[UserData6] END),
		a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN b.[UserData7] ELSE a.[UserData7] END),
		a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN b.[UserData8] ELSE a.[UserData8] END),
		a.[UserData9] = (CASE d.[UserData9_RVFlag] WHEN 1 THEN b.[UserData9] ELSE a.[UserData9] END),
		a.[UserGuid] = (CASE d.[UserGuid_RVFlag] WHEN 1 THEN b.[UserGuid] ELSE a.[UserGuid] END),
		a.[Zip] = (CASE d.[Zip_RVFlag] WHEN 1 THEN b.[Zip] ELSE a.[Zip] END)			
		FROM tblPersonnel a
		INNER JOIN tblPersonnel b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempPersonnelRecordVersioningFlag d
		ON d.PersonnelGuid = a.PersonnelGuid
		WHERE b.PersonnelGuid = @SourcePersonnelGuid
		AND d._CallingReferenceGuid = @callingRef1Guid
		AND a.PersonnelGuid = a._MasterRecordGuid

		DELETE erv.tblTempPersonnelRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 

		
		/*Process those ParentSpecific External fields whose propagation require custom handling. */	
		-- Process [Roles] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Roles') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Personnel record
			DELETE a FROM [map].[tblPersonnelToRole] a
			INNER JOIN [dbo].tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE b.PersonnelGuid = @masterRecordGuid
			AND b.PersonnelGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblPersonnelToRole] d
				INNER JOIN [dbo].tblPersonnel e
				ON e.PersonnelGuid = d.PersonnelGuid			
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)			
													
			--No characteristics of the PersonnelToRole mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblPersonnelToRole]
			(LookupPersonnelRoleIndex, PersonnelGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.LookupPersonnelRoleIndex, b._MasterRecordGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblPersonnelToRole] a
			INNER JOIN dbo.tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblPersonnelToRole] d
				WHERE d.PersonnelGuid = b._MasterRecordGuid
				AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
			)
		END
		

		-- Process [AccessSchedule] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Schedule') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Personnel record
			DELETE a FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN [dbo].tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE b.PersonnelGuid = @masterRecordGuid
			AND b.PersonnelGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d
				INNER JOIN [dbo].tblPersonnel e
				ON e.PersonnelGuid = d.PersonnelGuid			
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
								
			--Update the master record version mappings that have been modified in the child Personnel record
			UPDATE d
			SET d.Enabled = a.Enabled, 
			d.OpeningTime = a.OpeningTime,
			d.ClosingTime = a.ClosingTime,
			d.EndOfDayEnabled = a.EndOfDayEnabled,
			d.EndOfDayTime = a.EndOfDayTime,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN [dbo].[tblPersonnel] b
			ON b.PersonnelGuid = a.PersonnelGuid
			INNER JOIN [dbo].[tblSchedulePersonnelAccess] d
			ON d.PersonnelGuid = b._MasterRecordGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblSchedulePersonnelAccess]
			(PersonnelGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b._MasterRecordGuid, a.LookupDayOfWeekIndex, a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblSchedulePersonnelAccess] a
			INNER JOIN dbo.tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblSchedulePersonnelAccess] d
				WHERE d.PersonnelGuid = b._MasterRecordGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
		END


		-- Process [Qualifications] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Qualification') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Personnel record
			DELETE a FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN [dbo].tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE b.PersonnelGuid = @masterRecordGuid
			AND b.PersonnelGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d
				INNER JOIN [dbo].tblPersonnel e
				ON e.PersonnelGuid = d.PersonnelGuid			
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)	
													
			--Update the master record version mappings that have been modified in the child Personnel record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.HistoricalRecord = a.HistoricalRecord,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN [dbo].[tblPersonnel] b
			ON b.PersonnelGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonQualificationToPerson] d
			ON d.PersonnelGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationPersonQualificationToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b._MasterRecordGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonQualificationToPerson] a
			INNER JOIN dbo.tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d
				WHERE d.PersonnelGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		


		-- Process [Training] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Training') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Personnel record
			DELETE a FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN [dbo].tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE b.PersonnelGuid = @masterRecordGuid
			AND b.PersonnelGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d
				INNER JOIN [dbo].tblPersonnel e
				ON e.PersonnelGuid = d.PersonnelGuid			
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)	
				
			--Update the master record version mappings that have been modified in the child Personnel record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.HistoricalRecord = a.HistoricalRecord,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN [dbo].[tblPersonnel] b
			ON b.PersonnelGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonTrainingToPerson] d
			ON d.PersonnelGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationPersonTrainingToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b._MasterRecordGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonTrainingToPerson] a
			INNER JOIN dbo.tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d
				WHERE d.PersonnelGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		


		-- Process [Licenses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'License') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Personnel record
			DELETE a FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN [dbo].tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE b.PersonnelGuid = @masterRecordGuid
			AND b.PersonnelGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d
				INNER JOIN [dbo].tblPersonnel e
				ON e.PersonnelGuid = d.PersonnelGuid			
				WHERE d.PersonnelGuid = @SourcePersonnelGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)		
										
			--Update the master record version mappings that have been modified in the child Personnel record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Instructor = a.Instructor,
			d.Rating = a.Rating,
			d.HistoricalRecord = a.HistoricalRecord,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN [dbo].[tblPersonnel] b
			ON b.PersonnelGuid = a.PersonnelGuid
			INNER JOIN [map].[tblQualificationPersonLicenseToPerson] d
			ON d.PersonnelGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationPersonLicenseToPerson]
			(PersonnelGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b._MasterRecordGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationPersonLicenseToPerson] a
			INNER JOIN dbo.tblPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			WHERE a.PersonnelGuid = @SourcePersonnelGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d
				WHERE d.PersonnelGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END		
	
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --ReplicateToMasterRecord
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --ReplicateToMasterRecord
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
						+ 'Procedure Name: [erv].usp_ReplicatePersonnelGSChangesOnMaster' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END