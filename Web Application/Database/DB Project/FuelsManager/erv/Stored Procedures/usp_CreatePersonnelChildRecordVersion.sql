/*
	DROP PROCEDURE [erv].[usp_CreatePersonnelChildRecordVersion]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreatePersonnelChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreatePersonnelChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364', @dt, 'HB'

	SELECT PersonnelGuid, PersonID, _MasterRecordGuid, SiteGuid, * FROM tblPersonnel WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreatePersonnelChildRecordVersion]
(
	@ParentEntityGuid uniqueidentifier, @TargetSiteIndex uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreatePersonnelChildRecordVersion] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Personnel record version for a target site/sitegroup, off a parent record version 
	-- Notes:
	-- 1. @ParentEntityGuid: Entity Guid of the record to be cloned.
	-- 2. @TargetSiteIndex: Site/SiteGroup for which the new clone needs to be created.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @childRecordVersionGuid uniqueidentifier
		SET @childRecordVersionGuid = NEWID()

		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @sourceSite uniqueidentifier
		SELECT @masterRecGuid = _MasterRecordGuid, @sourceSite = SiteGuid FROM tblPersonnel
		WHERE PersonnelGuid = @ParentEntityGuid

		IF NOT EXISTS
		(
			SELECT * FROM map.tblEntityPersonnelToSite
			WHERE PersonnelGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		IF EXISTS
		(
			SELECT * FROM tblPersonnel
			WHERE _MasterRecordGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		--Create the child record version by cloning the internal fields of the parent record version
		INSERT INTO tblPersonnel
		(PersonnelGuid,PersonID,SiteGuid,_MasterRecordGuid,CardNumber,FirstName,MiddleName,LastName,Title,Department,Address1,Address2,City,State,Zip,Country,Phone1,Phone2,AssignmentDate,SupervisionDate,SSAN,BirthDate,PayRate,LaborRate1,LaborRate2,LaborRate3,LaborRate4,Status,Email,ResponsibleOfficer,Shift,PINNumber,PINRequired,LockedOut,LockedOutReason,LockedOutDate,LastActivityDate,CardedIn,ShortCardNumber,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,OnFileSignature,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,CompanyGuid,SupervisorPersonnelGuid,UserGuid,AssignedEquipmentGuid,HiddenDate,InhibitInactivityLockout)	
		SELECT @childRecordVersionGuid,PersonID,@TargetSiteIndex,_MasterRecordGuid,CardNumber,FirstName,MiddleName,LastName,Title,Department,Address1,Address2,City,State,Zip,Country,Phone1,Phone2,AssignmentDate,SupervisionDate,SSAN,BirthDate,PayRate,LaborRate1,LaborRate2,LaborRate3,LaborRate4,Status,Email,ResponsibleOfficer,Shift,PINNumber,PINRequired,LockedOut,LockedOutReason,LockedOutDate,LastActivityDate,CardedIn,ShortCardNumber,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,OnFileSignature,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,CompanyGuid,SupervisorPersonnelGuid,UserGuid,AssignedEquipmentGuid,HiddenDate,InhibitInactivityLockout
		FROM tblPersonnel
		WHERE PersonnelGuid = @ParentEntityGuid

		Insert into tblSchedulePersonnelAccess (SchedulePersonnelAccessGuid,PersonnelGuid,LookupDayOfWeekIndex,Enabled,OpeningTime,ClosingTime,EndOfDayEnabled,EndOfDayTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),@childRecordVersionGuid,a.LookupDayOfWeekIndex,a.Enabled,a.OpeningTime,a.ClosingTime,a.EndOfDayEnabled,a.EndOfDayTime,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM tblSchedulePersonnelAccess a
		WHERE PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM tblSchedulePersonnelAccess d
			WHERE d.PersonnelGuid = @childRecordVersionGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
		)

		INSERT INTO [map].[tblPersonnelToRole]
		(PersonnelToRoleGuid,PersonnelGuid,LookupPersonnelRoleIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),@childRecordVersionGuid,a.LookupPersonnelRoleIndex,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblPersonnelToRole] a
		WHERE PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblPersonnelToRole] d
			WHERE d.PersonnelGuid = @childRecordVersionGuid
			AND d.LookupPersonnelRoleIndex = a.LookupPersonnelRoleIndex
		)

		UPDATE a 
		SET a.PersonnelGuid = @childRecordVersionGuid
		FROM [map].[tblQualificationPersonQualificationToPerson] a
		INNER JOIN tblPersonnel b
		ON b.PersonnelGuid = a.PersonnelGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.PersonnelGuid = @ParentEntityGuid	

		INSERT INTO [map].[tblQualificationPersonQualificationToPerson]
		(QualificationPersonQualificationToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,@childRecordVersionGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonQualificationToPerson] a
		WHERE PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonQualificationToPerson] d 
			WHERE d.PersonnelGuid = @childRecordVersionGuid
			AND d.QualificationGuid = a.QualificationGuid
		)

		UPDATE a 
		SET a.PersonnelGuid = @childRecordVersionGuid
		FROM [map].[tblQualificationPersonLicenseToPerson] a
		INNER JOIN tblPersonnel b
		ON b.PersonnelGuid = a.PersonnelGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.PersonnelGuid = @ParentEntityGuid	

		INSERT INTO [map].[tblQualificationPersonLicenseToPerson]
		(QualificationPersonLicenseToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,@childRecordVersionGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonLicenseToPerson] a
		WHERE PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonLicenseToPerson] d 
			WHERE d.PersonnelGuid = @childRecordVersionGuid
			AND d.QualificationGuid = a.QualificationGuid
		)

		UPDATE a 
		SET a.PersonnelGuid = @childRecordVersionGuid
		FROM [map].[tblQualificationPersonTrainingToPerson] a
		INNER JOIN tblPersonnel b
		ON b.PersonnelGuid = a.PersonnelGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.PersonnelGuid = @ParentEntityGuid	

		INSERT INTO [map].[tblQualificationPersonTrainingToPerson]
		(QualificationPersonTrainingToPersonGuid,QualificationGuid,PersonnelGuid,Sequence,Instructor,DateCompleted,DateDue,ExpirationDate,ID,Rating,HistoricalRecord,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT NewID(),a.QualificationGuid,@childRecordVersionGuid,a.Sequence,a.Instructor,a.DateCompleted,a.DateDue,a.ExpirationDate,a.ID,a.Rating,a.HistoricalRecord,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationPersonTrainingToPerson] a
		WHERE PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationPersonTrainingToPerson] d 
			WHERE d.PersonnelGuid = @childRecordVersionGuid
			AND d.QualificationGuid = a.QualificationGuid
		)

		
		--Carrier	
		UPDATE a 
		SET a.PersonnelGuid = @childRecordVersionGuid
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.PersonnelGuid = @ParentEntityGuid

		INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
		(PersonnelGuid, CompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b.MasterRecordGuid, @TargetSiteIndex), a.CompanyGuid), --Clone the mapping even if the Company is not assigned to the target site, so that the invalid mapping is available when/if the Company is eventually mapped to the site.
		@TargetSiteIndex, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN [erv].[udf_GetCompanyRecordVersions](@sourceSite) b  --Only clone those Personnel mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Personnel RecordVersioning was Off (and Company RecordVersioning was On).
		ON b.CompanyGuid = a.CompanyGuid
		WHERE a.PersonnelGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany]  c
		  WHERE c.PersonnelGuid = @childRecordVersionGuid
		  AND c.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b.MasterRecordGuid, @TargetSiteIndex), a.CompanyGuid)
		)

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
						+ 'Procedure Name: [erv].usp_CreatePersonnelChildRecordVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
